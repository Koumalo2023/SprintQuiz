using Microsoft.EntityFrameworkCore;
using AutoMapper;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Repositories;

namespace SprintQuiz.Api.Services
{
    public class SprintService : ISprintService
    {
        private readonly ISprintRepository _repository;
        private readonly IMapper _mapper;
        private readonly SprintQuizDbContext _context;

        public SprintService(
            ISprintRepository repository,
            IMapper mapper,
            SprintQuizDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<SprintDto>> GetAllAsync()
        {
            var sprints = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<SprintDto>>(sprints);

            foreach(var dto in dtos)
            {
                await EnrichSprintDto(dto, null);
            }
            return dtos;
        }

        public async Task<SprintDto?> GetByIdAsync(Guid id, Guid? utilisateurId = null)
        {
            var sprint = await _repository.GetByIdAsync(id);
            if (sprint == null) return null;

            var dto = _mapper.Map<SprintDto>(sprint);

            await EnrichSprintDto(dto, utilisateurId);
            return dto;
        }

        public async Task<SprintDto?> GetWithModulesAsync(Guid id, Guid? utilisateurId = null)
        {
            var sprint = await _repository.GetByIdWithModulesAsync(id);
            if (sprint == null) return null;

            var dto = _mapper.Map<SprintDto>(sprint);

            await EnrichSprintDto(dto, utilisateurId);
            return dto;
        }

        public async Task<SprintDto> CreateAsync(CreateSprintDto createDto)
        {
            var sprint = _mapper.Map<Sprint>(createDto);
            var created = await _repository.CreateAsync(sprint);
            return _mapper.Map<SprintDto>(created);
        }

        public async Task<SprintDto?> UpdateAsync(Guid id, UpdateSprintDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(updateDto, existing);
            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<SprintDto>(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }

        private async Task EnrichSprintDto(SprintDto dto, Guid? utilisateurId)
        {
            // Compteurs
            dto.NombreQuiz = await _context.Quizzes.CountAsync(q => q.Niveau == NiveauEnum.Sprint && q.NiveauId == dto.Id);
            dto.NombreFlashcards = await _context.QAQuestions.CountAsync(q => q.Niveau == NiveauEnum.Sprint && q.NiveauId == dto.Id);
            dto.NombreExercices = await _context.Exercices.CountAsync(e => e.Niveau == NiveauEnum.Sprint && e.NiveauId == dto.Id);

            // Durée estimée
            dto.DureeEstimee = await CalculateEstimatedTime(NiveauEnum.Sprint, dto.Id);

            // Dernière activité
            if (utilisateurId.HasValue)
            {
                var progression = await _context.ProgressionsUtilisateur
                    .FirstOrDefaultAsync(p => p.UtilisateurId == utilisateurId.Value
                                           && p.Niveau == NiveauEnum.Sprint
                                           && p.NiveauId == dto.Id);
                dto.DerniereActivite = progression?.DerniereActivite;
            }
        }

        private async Task<int> CalculateEstimatedTime(NiveauEnum niveau, Guid niveauId)
        {
            int time = 0;

            var quizQuestions = await _context.Quizzes
                .Where(q => q.Niveau == niveau && q.NiveauId == niveauId)
                .SelectMany(q => q.Questions)
                .CountAsync();
            time += (quizQuestions * 90) / 60;

            var qaCount = await _context.QAQuestions.CountAsync(q => q.Niveau == niveau && q.NiveauId == niveauId);
            time += (qaCount * 45) / 60;

            var exercices = await _context.Exercices
                .Where(e => e.Niveau == niveau && e.NiveauId == niveauId)
                .ToListAsync();
            time += exercices.Sum(e => e.Type switch
            {
                TypeExercice.Basique => 2,
                TypeExercice.Applique => 4,
                TypeExercice.Analyse or TypeExercice.Cas => 7,
                TypeExercice.Defi => 10,
                _ => 2
            });

            return time;
        }
    }
}

