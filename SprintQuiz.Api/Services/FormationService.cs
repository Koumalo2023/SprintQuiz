// Services/FormationService.cs
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Repositories;
using SprintQuiz.Api.Services;

namespace SprintQuiz.Services
{
    public class FormationService : IFormationService
    {
        private readonly IFormationRepository _repository;
        private readonly IMapper _mapper;
        private readonly SprintQuizDbContext _context;

        public FormationService(
            IFormationRepository repository,
            IMapper mapper,
            SprintQuizDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<FormationDto>> GetAllAsync()
        {
            var formations = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<FormationDto>>(formations);

            // Ajouter la dernière activité pour chaque formation (si besoin, géré au niveau global)
            return dtos;
        }

        public async Task<FormationDto?> GetByIdAsync(Guid id, Guid? utilisateurId = null)
        {
            var formation = await _repository.GetByIdWithSprintsAsync(id);
            if (formation == null) return null;

            var dto = _mapper.Map<FormationDto>(formation);

            // Calculer la progression et dernière activité si utilisateur connecté
            if (utilisateurId.HasValue)
            {
                var progression = await _context.ProgressionsUtilisateur
                    .FirstOrDefaultAsync(p => p.UtilisateurId == utilisateurId.Value
                                           && p.Niveau == NiveauEnum.Formation
                                           && p.NiveauId == id);

                dto.DerniereActivite = progression?.DerniereActivite;

                // Calculer le % de complétion (optionnel ici, peut être délégué au frontend ou à un service dédié)
            }

            // Calculer les compteurs et durée estimée
            dto.NombreQuiz = await _context.Quizzes.CountAsync(q => q.Niveau == NiveauEnum.Formation && q.NiveauId == id);
            dto.NombreFlashcards = await _context.QAQuestions.CountAsync(q => q.Niveau == NiveauEnum.Formation && q.NiveauId == id);
            dto.NombreExercices = await _context.Exercices.CountAsync(e => e.Niveau == NiveauEnum.Formation && e.NiveauId == id);

            dto.DureeEstimee = await CalculateEstimatedTime(id);

            return dto;
        }

        public async Task<FormationDto> CreateAsync(CreateFormationDto createDto)
        {
            var formation = _mapper.Map<Formation>(createDto);
            var created = await _repository.CreateAsync(formation);
            return _mapper.Map<FormationDto>(created);
        }

        public async Task<FormationDto> UpdateAsync(Guid id, UpdateFormationDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException($"Formation avec l'ID {id} non trouvée.");

            _mapper.Map(updateDto, existing);
            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<FormationDto>(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }

        // Méthode privée de calcul du temps estimé
        private async Task<int> CalculateEstimatedTime(Guid formationId)
        {
            int time = 0;

            // Quiz
            var quizQuestions = await _context.Quizzes
                .Where(q => q.Niveau == NiveauEnum.Formation && q.NiveauId == formationId)
                .SelectMany(q => q.Questions)
                .CountAsync();
            time += (quizQuestions * 90) / 60; // 90s par question

            // Flashcards
            var qaCount = await _context.QAQuestions
                .CountAsync(q => q.Niveau == NiveauEnum.Formation && q.NiveauId == formationId);
            time += (qaCount * 45) / 60; // 45s par flashcard

            // Exercices
            var exercices = await _context.Exercices
                .Where(e => e.Niveau == NiveauEnum.Formation && e.NiveauId == formationId)
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