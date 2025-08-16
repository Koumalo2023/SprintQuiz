using SprintQuiz.Api.DTOs;

namespace SprintQuiz.Api.Services
{
    public interface IFormationService
    {
        Task<IEnumerable<FormationDto>> GetAllAsync();
        Task<FormationDto?> GetByIdAsync(Guid id, Guid? utilisateurId = null);
        Task<FormationDto> CreateAsync(CreateFormationDto createDto);
        Task<FormationDto> UpdateAsync(Guid id, UpdateFormationDto updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
