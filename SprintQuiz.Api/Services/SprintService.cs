using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Repositories;

namespace SprintQuiz.Api.Services
{
    public class SprintService : ISprintService
    {
        private readonly ISprintRepository _sprintRepository;
        private readonly IMapper _mapper;

        public SprintService(ISprintRepository sprintRepository, IMapper mapper)
        {
            _sprintRepository = sprintRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SprintDto>> GetAllSprintsAsync()
        {
            var sprints = await _sprintRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SprintDto>>(sprints);
        }

        public async Task<SprintDto?> GetSprintByIdAsync(Guid id)
        {
            var sprint = await _sprintRepository.GetByIdAsync(id);
            return sprint == null ? null : _mapper.Map<SprintDto>(sprint);
        }

        public async Task<SprintDto?> GetSprintWithModulesAsync(Guid id)
        {
            var sprint = await _sprintRepository.GetByIdWithModulesAsync(id);
            return sprint == null ? null : _mapper.Map<SprintDto>(sprint);
        }

        public async Task<SprintDto> CreateSprintAsync(CreateSprintDto createSprintDto)
        {
            var sprint = _mapper.Map<Sprint>(createSprintDto);
            var createdSprint = await _sprintRepository.CreateAsync(sprint);
            return _mapper.Map<SprintDto>(createdSprint);
        }

        public async Task<SprintDto?> UpdateSprintAsync(Guid id, UpdateSprintDto updateSprintDto)
        {
            var existingSprint = await _sprintRepository.GetByIdAsync(id);
            if (existingSprint == null) return null;

            _mapper.Map(updateSprintDto, existingSprint);
            var updatedSprint = await _sprintRepository.UpdateAsync(existingSprint);
            return _mapper.Map<SprintDto>(updatedSprint);
        }

        public async Task<bool> DeleteSprintAsync(Guid id)
        {
            return await _sprintRepository.DeleteAsync(id);
        }
    }
}

