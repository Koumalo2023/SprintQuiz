using SprintQuiz.Api.DTOs;

namespace SprintQuiz.Api.Services
{
    public interface IUtilisateurService
    {
        Task<IEnumerable<UtilisateurDto>> GetAllUtilisateursAsync();
        Task<UtilisateurDto?> GetUtilisateurByIdAsync(Guid id);
        Task<UtilisateurDto> CreateUtilisateurAsync(CreateUtilisateurDto createUtilisateurDto);
        Task<UtilisateurDto?> UpdateUtilisateurAsync(Guid id, UpdateUtilisateurDto updateUtilisateurDto);
        Task<bool> DeleteUtilisateurAsync(Guid id);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<StatistiquesGlobalesDto?> GetUserStatisticsAsync(Guid utilisateurId);
        Task<IEnumerable<ProgressionUtilisateurDto>> GetUserProgressionAsync(Guid utilisateurId);

        //Gestion des inscription
        Task<InscriptionDto> InscrireEtudiantAsync(CreateInscriptionDto dto);
        Task<IEnumerable<InscriptionDto>> GetInscriptionsParFormationAsync(Guid formationId);
        Task<bool> DesinscrireEtudiantDeFormationAsync(Guid utilisateurId, Guid formationId);
        Task<IEnumerable<InscriptionDto>> GetInscriptionsUtilisateurAsync(Guid utilisateurId);
        Task<bool> EstInscritAFormationAsync(Guid utilisateurId, Guid formationId);
    }
}

