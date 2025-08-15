using AutoMapper;
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

