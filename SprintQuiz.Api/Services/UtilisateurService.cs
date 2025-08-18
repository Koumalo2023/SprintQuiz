using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace SprintQuiz.Api.Services
{
    public class UtilisateurService : IUtilisateurService
    {
        private readonly IUtilisateurRepository _utilisateurRepository;
        private readonly SprintQuizDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public UtilisateurService(
            IUtilisateurRepository utilisateurRepository, 
            SprintQuizDbContext context, 
            IMapper mapper,
            ITokenService tokenService,
            IConfiguration configuration)
        {
            _utilisateurRepository = utilisateurRepository;
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<IEnumerable<UtilisateurDto>> GetAllUtilisateursAsync()
        {
            var utilisateurs = await _utilisateurRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UtilisateurDto>>(utilisateurs);
        }

        public async Task<UtilisateurDto?> GetUtilisateurByIdAsync(Guid id)
        {
            var utilisateur = await _utilisateurRepository.GetByIdAsync(id);
            return utilisateur == null ? null : _mapper.Map<UtilisateurDto>(utilisateur);
        }

        public async Task<UtilisateurDto> CreateUtilisateurAsync(CreateUtilisateurDto createUtilisateurDto)
        {
            if (await _utilisateurRepository.EmailExistsAsync(createUtilisateurDto.Email))
            {
                throw new ArgumentException("Un utilisateur avec cet email existe déjà");
            }

            var utilisateur = _mapper.Map<Utilisateur>(createUtilisateurDto);
            utilisateur.MotDePasse = HashPassword(createUtilisateurDto.MotDePasse);
            
            var createdUtilisateur = await _utilisateurRepository.CreateAsync(utilisateur);
            return _mapper.Map<UtilisateurDto>(createdUtilisateur);
        }

        public async Task<UtilisateurDto?> UpdateUtilisateurAsync(Guid id, UpdateUtilisateurDto updateUtilisateurDto)
        {
            var existingUtilisateur = await _utilisateurRepository.GetByIdAsync(id);
            if (existingUtilisateur == null) return null;

            if (!string.IsNullOrEmpty(updateUtilisateurDto.Email) && 
                updateUtilisateurDto.Email != existingUtilisateur.Email &&
                await _utilisateurRepository.EmailExistsAsync(updateUtilisateurDto.Email))
            {
                throw new ArgumentException("Un utilisateur avec cet email existe déjà");
            }

            _mapper.Map(updateUtilisateurDto, existingUtilisateur);
            var updatedUtilisateur = await _utilisateurRepository.UpdateAsync(existingUtilisateur);
            return _mapper.Map<UtilisateurDto>(updatedUtilisateur);
        }

        public async Task<bool> DeleteUtilisateurAsync(Guid id)
        {
            return await _utilisateurRepository.DeleteAsync(id);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var utilisateur = await _utilisateurRepository.GetByEmailAsync(loginDto.Email);
            if (utilisateur == null || !VerifyPassword(loginDto.MotDePasse, utilisateur.MotDePasse))
            {
                return null;
            }

            var token = _tokenService.GenerateToken(utilisateur);
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

            return new AuthResponseDto
            {
                Token = token,
                Utilisateur = _mapper.Map<UtilisateurDto>(utilisateur),
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };
        }
        public async Task<StatistiquesGlobalesDto?> GetUserStatisticsAsync(Guid utilisateurId)
        {
            var stats = await _context.StatistiquesGlobales
                .FirstOrDefaultAsync(s => s.UtilisateurId == utilisateurId);

            return stats == null ? null : _mapper.Map<StatistiquesGlobalesDto>(stats);
        }
        private async Task CreateProgressionForFormation(Guid utilisateurId, Guid formationId)
        {
            var progression = new ProgressionUtilisateur
            {
                Id = Guid.NewGuid(),
                UtilisateurId = utilisateurId,
                Niveau = NiveauEnum.Formation,
                NiveauId = formationId,
                PourcentageComplet = 0,
                DerniereActivite = DateTime.UtcNow,
                DateCreation = DateTime.UtcNow
            };

            _context.ProgressionsUtilisateur.Add(progression);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProgressionUtilisateurDto>> GetUserProgressionAsync(Guid utilisateurId)
        {
            var progressions = await _context.ProgressionsUtilisateur
                .Include(p => p.Sprint)
                .Include(p => p.Module)
                .Include(p => p.Cours)
                .Where(p => p.UtilisateurId == utilisateurId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProgressionUtilisateurDto>>(progressions);
        }

        // Gestion de l'inscription des utulisateur aux formation
        public async Task<InscriptionDto> InscrireEtudiantAsync(CreateInscriptionDto dto)
        {
            // Vérifier que l'utilisateur existe et est un étudiant
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Id == dto.UtilisateurId);
            if (utilisateur == null)
                throw new KeyNotFoundException("Utilisateur non trouvé.");

            if (utilisateur.Role != RoleUtilisateur.Etudiant)
                throw new InvalidOperationException("Seuls les étudiants peuvent être inscrits à une formation.");

            // Vérifier que la formation existe
            var formation = await _context.Formations
                .FirstOrDefaultAsync(f => f.Id == dto.FormationId);
            if (formation == null)
                throw new KeyNotFoundException("Formation non trouvée.");

            // Vérifier que l'inscription n'existe pas déjà
            var existe = await _context.InscriptionFormations
                .AnyAsync(i => i.UtilisateurId == dto.UtilisateurId && i.FormationId == dto.FormationId);
            if (existe)
                throw new InvalidOperationException("Cet étudiant est déjà inscrit à cette formation.");

            // Créer l'inscription
            var inscription = new InscriptionFormation
            {
                Id = Guid.NewGuid(),
                UtilisateurId = dto.UtilisateurId,
                FormationId = dto.FormationId,
                DateInscription = DateTime.UtcNow
            };

            _context.InscriptionFormations.Add(inscription);
            await _context.SaveChangesAsync();

            // Créer la progression initiale pour la formation
            await CreateProgressionForFormation(inscription.UtilisateurId, inscription.FormationId);

            // Charger pour le DTO
            var dtoResult = _mapper.Map<InscriptionDto>(inscription);
            dtoResult.NomUtilisateur = utilisateur.Nom;
            dtoResult.NomFormation = formation.Nom;

            return dtoResult;
        }

        public async Task<IEnumerable<InscriptionDto>> GetInscriptionsParFormationAsync(Guid formationId)
        {
            var inscriptions = await _context.InscriptionFormations
                .Include(i => i.Utilisateur)
                .Include(i => i.Formation)
                .Where(i => i.FormationId == formationId)
                .ToListAsync();

            return inscriptions.Select(i => new InscriptionDto
            {
                Id = i.Id,
                UtilisateurId = i.UtilisateurId,
                NomUtilisateur = i.Utilisateur.Nom,
                FormationId = i.FormationId,
                NomFormation = i.Formation.Nom,
                DateInscription = i.DateInscription
            }).ToList();
        }

        public async Task<bool> DesinscrireEtudiantDeFormationAsync(Guid utilisateurId, Guid formationId)
        {
            var inscription = await _context.InscriptionFormations
                .FirstOrDefaultAsync(i => i.UtilisateurId == utilisateurId && i.FormationId == formationId);

            if (inscription == null) return false;

            _context.InscriptionFormations.Remove(inscription);
            await _context.SaveChangesAsync();
            return true;
        }


        // Services/UtilisateurService.cs
        public async Task<IEnumerable<InscriptionDto>> GetInscriptionsUtilisateurAsync(Guid utilisateurId)
        {
            var inscriptions = await _context.InscriptionFormations
                .Include(i => i.Formation)
                .Where(i => i.UtilisateurId == utilisateurId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<InscriptionDto>>(inscriptions);
        }

        public async Task<bool> EstInscritAFormationAsync(Guid utilisateurId, Guid formationId)
        {
            return await _context.InscriptionFormations
                .AnyAsync(i => i.UtilisateurId == utilisateurId && i.FormationId == formationId);
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

