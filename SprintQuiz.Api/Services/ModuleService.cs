using Microsoft.EntityFrameworkCore;
using AutoMapper;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Repositories;

namespace SprintQuiz.Api.Services
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _repository;
        private readonly IMapper _mapper;
        private readonly SprintQuizDbContext _context;

        public ModuleService(
            IModuleRepository repository,
            IMapper mapper,
            SprintQuizDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<ModuleDto>> GetAllAsync()
        {
            var modules = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<ModuleDto>>(modules);

            foreach (var dto in dtos)
            {
                await EnrichModuleDto(dto, null);
            }

            return dtos;
        }

        public async Task<ModuleDto?> GetByIdAsync(Guid id, Guid? utilisateurId = null)
        {
            var module = await _repository.GetByIdAsync(id);
            if (module == null) return null;

            var dto = _mapper.Map<ModuleDto>(module);
            await EnrichModuleDto(dto, utilisateurId);
            return dto;
        }

        public async Task<ModuleDto?> GetWithCoursAsync(Guid id, Guid? utilisateurId = null)
        {
            var module = await _repository.GetByIdWithCoursAsync(id);
            if (module == null) return null;

            var dto = _mapper.Map<ModuleDto>(module);
            await EnrichModuleDto(dto, utilisateurId);
            return dto;
        }

        public async Task<ModuleDto> CreateAsync(CreateModuleDto createDto)
        {
            var module = _mapper.Map<Module>(createDto);
            var created = await _repository.CreateAsync(module);
            return _mapper.Map<ModuleDto>(created);
        }

        public async Task<ModuleDto?> UpdateAsync(Guid id, UpdateModuleDto updateDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(updateDto, existing);
            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<ModuleDto>(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<IEnumerable<ModuleDto>> GetBySprintIdAsync(Guid sprintId, Guid? utilisateurId = null)
        {
            var modules = await _repository.GetBySprintIdAsync(sprintId);
            var dtos = _mapper.Map<IEnumerable<ModuleDto>>(modules);

            foreach (var dto in dtos)
            {
                await EnrichModuleDto(dto, utilisateurId);
            }

            return dtos;
        }

        private async Task EnrichModuleDto(ModuleDto dto, Guid? utilisateurId)
        {
            dto.NombreQuiz = await _context.Quizzes.CountAsync(q => q.Niveau == NiveauEnum.Module && q.NiveauId == dto.Id);
            dto.NombreFlashcards = await _context.QAQuestions.CountAsync(q => q.Niveau == NiveauEnum.Module && q.NiveauId == dto.Id);
            dto.NombreExercices = await _context.Exercices.CountAsync(e => e.Niveau == NiveauEnum.Module && e.NiveauId == dto.Id);

            dto.DureeEstimee = await CalculateEstimatedTime(NiveauEnum.Module, dto.Id);

            if (utilisateurId.HasValue)
            {
                var progression = await _context.ProgressionsUtilisateur
                    .FirstOrDefaultAsync(p => p.UtilisateurId == utilisateurId.Value
                                           && p.Niveau == NiveauEnum.Module
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

