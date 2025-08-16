using Microsoft.EntityFrameworkCore;
using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Repositories;
using SprintQuiz.Api.Data;

namespace SprintQuiz.Api.Services
{
    public class CoursService : ICoursService
    {

        private readonly ICoursRepository _repository;
        private readonly IMapper _mapper;
        private readonly SprintQuizDbContext _context;

        public CoursService(
            ICoursRepository repository,
            IMapper mapper,
            SprintQuizDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<CoursDto>> GetAllAsync()
        {
            var cours = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<CoursDto>>(cours);

            foreach (var dto in dtos)
            {
                await EnrichCoursDto(dto, null);
            }

            return dtos;
        }

        public async Task<CoursDto?> GetByIdAsync(Guid id, Guid? utilisateurId = null)
        {
            var cours = await _repository.GetByIdAsync(id);
            if (cours == null) return null;

            var dto = _mapper.Map<CoursDto>(cours);
            await EnrichCoursDto(dto, utilisateurId);
            return dto;
        }

        public async Task<CoursDto> CreateAsync(CreateCoursDto createDto)
        {
            var cours = _mapper.Map<Cours>(createDto);
            var created = await _repository.CreateAsync(cours);
            return _mapper.Map<CoursDto>(created);
        }

        public async Task<CoursDto?> UpdateAsync(Guid id, UpdateCoursDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(updateDto, existing);
            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<CoursDto>(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<IEnumerable<CoursDto>> GetByModuleIdAsync(Guid moduleId, Guid? utilisateurId = null)
        {
            var cours = await _repository.GetByModuleIdAsync(moduleId);
            var dtos = _mapper.Map<IEnumerable<CoursDto>>(cours);

            foreach (var dto in dtos)
            {
                await EnrichCoursDto(dto, utilisateurId);
            }

            return dtos;
        }

        private async Task EnrichCoursDto(CoursDto dto, Guid? utilisateurId)
        {
            dto.NombreQuiz = await _context.Quizzes.CountAsync(q => q.Niveau == NiveauEnum.Cours && q.NiveauId == dto.Id);
            dto.NombreFlashcards = await _context.QAQuestions.CountAsync(q => q.Niveau == NiveauEnum.Cours && q.NiveauId == dto.Id);
            dto.NombreExercices = await _context.Exercices.CountAsync(e => e.Niveau == NiveauEnum.Cours && e.NiveauId == dto.Id);

            dto.DureeEstimee = await CalculateEstimatedTime(NiveauEnum.Cours, dto.Id);

            if (utilisateurId.HasValue)
            {
                var progression = await _context.ProgressionsUtilisateur
                    .FirstOrDefaultAsync(p => p.UtilisateurId == utilisateurId.Value
                                           && p.Niveau == NiveauEnum.Cours
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

