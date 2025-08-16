using SprintQuiz.Api.DTOs;

namespace SprintQuiz.Api.Services
{
    public interface ISprintService
    {
        Task<IEnumerable<SprintDto>> GetAllAsync();
        Task<SprintDto?> GetByIdAsync(Guid id, Guid? utilisateurId = null);
        Task<SprintDto?> GetWithModulesAsync(Guid id, Guid? utilisateurId = null);
        Task<SprintDto> CreateAsync(CreateSprintDto createDto);
        Task<SprintDto?> UpdateAsync(Guid id, UpdateSprintDto updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}

