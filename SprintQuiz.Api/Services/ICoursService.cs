using SprintQuiz.Api.DTOs;

namespace SprintQuiz.Api.Services
{
    public interface ICoursService
    {
        Task<IEnumerable<CoursDto>> GetAllCoursAsync();
        Task<CoursDto?> GetCoursByIdAsync(Guid id);
        Task<IEnumerable<CoursDto>> GetCoursByModuleIdAsync(Guid moduleId);
        Task<CoursDto> CreateCoursAsync(CreateCoursDto createCoursDto);
        Task<CoursDto?> UpdateCoursAsync(Guid id, UpdateCoursDto updateCoursDto);
        Task<bool> DeleteCoursAsync(Guid id);
    }
}

