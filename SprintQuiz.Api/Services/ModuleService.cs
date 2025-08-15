using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Repositories;

namespace SprintQuiz.Api.Services
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IMapper _mapper;

        public ModuleService(IModuleRepository moduleRepository, IMapper mapper)
        {
            _moduleRepository = moduleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
        {
            var modules = await _moduleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ModuleDto>>(modules);
        }

        public async Task<ModuleDto?> GetModuleByIdAsync(Guid id)
        {
            var module = await _moduleRepository.GetByIdAsync(id);
            return module == null ? null : _mapper.Map<ModuleDto>(module);
        }

        public async Task<ModuleDto?> GetModuleWithCoursAsync(Guid id)
        {
            var module = await _moduleRepository.GetByIdWithCoursAsync(id);
            return module == null ? null : _mapper.Map<ModuleDto>(module);
        }

        public async Task<IEnumerable<ModuleDto>> GetModulesBySprintIdAsync(Guid sprintId)
        {
            var modules = await _moduleRepository.GetBySprintIdAsync(sprintId);
            return _mapper.Map<IEnumerable<ModuleDto>>(modules);
        }

        public async Task<ModuleDto> CreateModuleAsync(CreateModuleDto createModuleDto)
        {
            var module = _mapper.Map<Module>(createModuleDto);
            var createdModule = await _moduleRepository.CreateAsync(module);
            return _mapper.Map<ModuleDto>(createdModule);
        }

        public async Task<ModuleDto?> UpdateModuleAsync(Guid id, UpdateModuleDto updateModuleDto)
        {
            var existingModule = await _moduleRepository.GetByIdAsync(id);
            if (existingModule == null) return null;

            _mapper.Map(updateModuleDto, existingModule);
            var updatedModule = await _moduleRepository.UpdateAsync(existingModule);
            return _mapper.Map<ModuleDto>(updatedModule);
        }

        public async Task<bool> DeleteModuleAsync(Guid id)
        {
            return await _moduleRepository.DeleteAsync(id);
        }
    }
}

