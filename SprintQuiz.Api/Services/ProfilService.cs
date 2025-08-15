using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace SprintQuiz.Api.Services
{
    public class ProfilService : IProfilService
    {
        private readonly IUtilisateurRepository _utilisateurRepository;
        private readonly SprintQuizDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProfilService> _logger;

        public ProfilService(
            IUtilisateurRepository utilisateurRepository,
            SprintQuizDbContext context,
            IMapper mapper,
            IWebHostEnvironment environment,
            ILogger<ProfilService> logger)
        {
            _utilisateurRepository = utilisateurRepository;
            _context = context;
            _mapper = mapper;
            _environment = environment;
            _logger = logger;
        }

        public async Task<ProfilDto?> GetProfilAsync(Guid utilisateurId)
        {
            var utilisateur = await _utilisateurRepository.GetByIdWithDetailsAsync(utilisateurId);
            if (utilisateur == null) return null;

            var profil = _mapper.Map<ProfilDto>(utilisateur);
            
            // Calculer la progression globale
            profil.ProgressionGlobale = await CalculateProgressionGlobaleAsync(utilisateurId);
            
            // Récupérer les dernières activités
            profil.DernieresActivites = await GetDernieresActivitesAsync(utilisateurId);

            return profil;
        }

        public async Task<ProfilDto?> UpdateProfilAsync(Guid utilisateurId, UpdateProfilDto updateProfilDto)
        {
            var utilisateur = await _utilisateurRepository.GetByIdAsync(utilisateurId);
            if (utilisateur == null) return null;

            // Vérifier si l'email existe déjà (si changé)
            if (!string.IsNullOrEmpty(updateProfilDto.Email) && 
                updateProfilDto.Email != utilisateur.Email &&
                await _utilisateurRepository.EmailExistsAsync(updateProfilDto.Email))
            {
                throw new ArgumentException("Un utilisateur avec cet email existe déjà");
            }

            _mapper.Map(updateProfilDto, utilisateur);
            await _utilisateurRepository.UpdateAsync(utilisateur);

            return await GetProfilAsync(utilisateurId);
        }

        public async Task<string?> UploadPhotoAsync(Guid utilisateurId, UploadPhotoDto uploadPhotoDto)
        {
            var utilisateur = await _utilisateurRepository.GetByIdAsync(utilisateurId);
            if (utilisateur == null) return null;

            try
            {
                // Créer le dossier uploads s'il n'existe pas
                var uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "profiles");
                Directory.CreateDirectory(uploadsPath);

                // Générer un nom de fichier unique
                var fileExtension = Path.GetExtension(uploadPhotoDto.FileName);
                var fileName = $"{utilisateurId}_{DateTime.UtcNow.Ticks}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Convertir base64 en bytes et sauvegarder
                var imageBytes = Convert.FromBase64String(uploadPhotoDto.PhotoBase64);
                await File.WriteAllBytesAsync(filePath, imageBytes);

                // Mettre à jour l'URL de la photo
                var photoUrl = $"/uploads/profiles/{fileName}";
                utilisateur.PhotoUrl = photoUrl;
                await _utilisateurRepository.UpdateAsync(utilisateur);

                _logger.LogInformation($"Photo de profil uploadée pour l'utilisateur {utilisateurId}: {photoUrl}");
                return photoUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de l'upload de la photo pour l'utilisateur {utilisateurId}");
                throw new InvalidOperationException("Erreur lors de l'upload de la photo");
            }
        }

        public async Task<bool> ChangePasswordAsync(Guid utilisateurId, ChangePasswordDto changePasswordDto)
        {
            var utilisateur = await _utilisateurRepository.GetByIdAsync(utilisateurId);
            if (utilisateur == null) return false;

            // Vérifier l'ancien mot de passe
            if (!VerifyPassword(changePasswordDto.CurrentPassword, utilisateur.MotDePasse))
            {
                throw new ArgumentException("Mot de passe actuel incorrect");
            }

            // Vérifier que les nouveaux mots de passe correspondent
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                throw new ArgumentException("Les nouveaux mots de passe ne correspondent pas");
            }

            // Hasher et sauvegarder le nouveau mot de passe
            utilisateur.MotDePasse = HashPassword(changePasswordDto.NewPassword);
            await _utilisateurRepository.UpdateAsync(utilisateur);

            _logger.LogInformation($"Mot de passe changé pour l'utilisateur {utilisateurId}");
            return true;
        }

        public async Task<DernieresActivitesDto> GetDernieresActivitesAsync(Guid utilisateurId)
        {
            var derniereTentative = await _context.TentativesQuiz
                .Include(t => t.Quiz)
                .Where(t => t.UtilisateurId == utilisateurId)
                .OrderByDescending(t => t.Date)
                .FirstOrDefaultAsync();

            var derniereConsultation = await _context.ConsultationsQA
                .Include(c => c.QAQuestion)
                .Where(c => c.UtilisateurId == utilisateurId)
                .OrderByDescending(c => c.DateConsultation)
                .FirstOrDefaultAsync();

            var derniereProgression = await _context.ProgressionsUtilisateur
                .Where(p => p.UtilisateurId == utilisateurId)
                .OrderByDescending(p => p.DerniereActivite)
                .FirstOrDefaultAsync();

            return new DernieresActivitesDto
            {
                DerniereTentativeQuiz = derniereTentative?.Date,
                DernierQuizTitre = derniereTentative?.Quiz.Titre,
                DerniereConsultationQA = derniereConsultation?.DateConsultation,
                DerniereQuestionQA = derniereConsultation?.QAQuestion.Question,
                DerniereActiviteGlobale = new[] { 
                    derniereTentative?.Date, 
                    derniereConsultation?.DateConsultation, 
                    derniereProgression?.DerniereActivite 
                }.Where(d => d.HasValue).Max()
            };
        }

        public async Task<float> CalculateProgressionGlobaleAsync(Guid utilisateurId)
        {
            var progressions = await _context.ProgressionsUtilisateur
                .Where(p => p.UtilisateurId == utilisateurId)
                .ToListAsync();

            if (!progressions.Any()) return 0f;

            return progressions.Average(p => p.PourcentageComplet);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }
}

