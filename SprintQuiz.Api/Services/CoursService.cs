using AutoMapper;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Repositories;

namespace SprintQuiz.Api.Services
{
    public class CoursService : ICoursService
    {
        private readonly ICoursRepository _coursRepository;
        private readonly IMapper _mapper;

        public CoursService(ICoursRepository coursRepository, IMapper mapper)
        {
            _coursRepository = coursRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CoursDto>> GetAllCoursAsync()
        {
            var cours = await _coursRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CoursDto>>(cours);
        }

        public async Task<CoursDto?> GetCoursByIdAsync(Guid id)
        {
            var cours = await _coursRepository.GetByIdAsync(id);
            return cours == null ? null : _mapper.Map<CoursDto>(cours);
        }

        public async Task<IEnumerable<CoursDto>> GetCoursByModuleIdAsync(Guid moduleId)
        {
            var cours = await _coursRepository.GetByModuleIdAsync(moduleId);
            return _mapper.Map<IEnumerable<CoursDto>>(cours);
        }

        public async Task<CoursDto> CreateCoursAsync(CreateCoursDto createCoursDto)
        {
            var cours = _mapper.Map<Cours>(createCoursDto);
            var createdCours = await _coursRepository.CreateAsync(cours);
            return _mapper.Map<CoursDto>(createdCours);
        }

        public async Task<CoursDto?> UpdateCoursAsync(Guid id, UpdateCoursDto updateCoursDto)
        {
            var existingCours = await _coursRepository.GetByIdAsync(id);
            if (existingCours == null) return null;

            _mapper.Map(updateCoursDto, existingCours);
            var updatedCours = await _coursRepository.UpdateAsync(existingCours);
            return _mapper.Map<CoursDto>(updatedCours);
        }

        public async Task<bool> DeleteCoursAsync(Guid id)
        {
            return await _coursRepository.DeleteAsync(id);
        }
    }
}

