using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Services
{
    public interface IExerciceService
    {
        Task<IEnumerable<ExerciceDto>> GetAllExercicesAsync();
        Task<ExerciceDto?> GetExerciceByIdAsync(Guid id, Guid? utilisateurId = null);
        Task<IEnumerable<ExerciceDto>> GetExercicesByNiveauAsync(NiveauEnum niveau, Guid niveauId);
        Task<ExerciceDto> CreateExerciceAsync(CreateExerciceDto createDto);
        Task<ExerciceDto?> UpdateExerciceAsync(Guid id, UpdateExerciceDto updateDto);
        Task<bool> DeleteExerciceAsync(Guid id);

        Task<ConsultationExerciceDto> ConsultExerciceAsync(Guid utilisateurId, CreateConsultationExerciceDto consultationDto);
        Task<IEnumerable<ConsultationExerciceDto>> GetUserConsultationsAsync(Guid utilisateurId);
        Task<IEnumerable<ExerciceDto>> GetExercicesForRevisionAsync(Guid utilisateurId, NiveauEnum niveau, Guid niveauId);

        // --- Gestion des Indices ---
        Task<IEnumerable<IndiceDto>> GetIndicesByExerciceIdAsync(Guid exerciceId);
        Task<IndiceDto?> GetIndiceByIdAsync(Guid id);
        Task<IndiceDto> CreateIndiceAsync(CreateIndiceDto createDto);
        Task<IndiceDto?> UpdateIndiceAsync(Guid id, UpdateIndiceDto updateDto);
        Task<bool> DeleteIndiceAsync(Guid id);

        // --- Gestion des Étapes de Résolution ---
        Task<IEnumerable<EtapeResolutionDto>> GetEtapesByExerciceIdAsync(Guid exerciceId);
        Task<EtapeResolutionDto?> GetEtapeByIdAsync(Guid id);
        Task<EtapeResolutionDto> CreateEtapeAsync(CreateEtapeResolutionDto createDto);
        Task<EtapeResolutionDto?> UpdateEtapeAsync(Guid id, UpdateEtapeResolutionDto updateDto);
        Task<bool> DeleteEtapeAsync(Guid id);
    }
}
