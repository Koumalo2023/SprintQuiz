using SprintQuiz.Api.DTOs;

namespace SprintQuiz.Api.Services
{
    public interface ICoursService
    {
        Task<IEnumerable<CoursDto>> GetAllAsync();
        Task<CoursDto?> GetByIdAsync(Guid id, Guid? utilisateurId = null);
        Task<CoursDto> CreateAsync(CreateCoursDto createDto);
        Task<CoursDto?> UpdateAsync(Guid id, UpdateCoursDto updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<CoursDto>> GetByModuleIdAsync(Guid moduleId, Guid? utilisateurId = null);
    }
}

