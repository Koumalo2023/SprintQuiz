using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Services
{
    public interface IDashboardAdminService
    {
        Task<DashboardOverviewDto> GetOverviewAsync();
        Task<DashboardProgressionDto> GetProgressionParNiveauAsync(NiveauEnum niveau, Guid niveauId);
        Task<QuizAnalyseDto> GetAnalyseQuizAsync(Guid quizId);
        Task<AlerteInactifsDto> GetAlertesInactifsAsync(int jours);
        Task<AlerteDifficulteDto> GetUtilisateursEnDifficulteAsync(float seuil);
        Task<HistoriqueActiviteDto> GetHistoriqueAsync(int periode);
        Task<TopFlopDto> GetTopFlopAsync();
    }
}
