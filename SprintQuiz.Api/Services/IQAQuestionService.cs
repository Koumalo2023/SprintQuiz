using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Services
{
    public interface IQAQuestionService
    {
        Task<IEnumerable<QAQuestionDto>> GetAllQAQuestionsAsync();
        Task<QAQuestionDto?> GetQAQuestionByIdAsync(Guid id, Guid? utilisateurId = null);
        Task<IEnumerable<QAQuestionDto>> GetQAQuestionsByNiveauAsync(NiveauEnum niveau, Guid niveauId);
        Task<QAQuestionDto> CreateQAQuestionAsync(CreateQAQuestionDto createQAQuestionDto);
        Task<QAQuestionDto?> UpdateQAQuestionAsync(Guid id, UpdateQAQuestionDto updateQAQuestionDto);
        Task<bool> DeleteQAQuestionAsync(Guid id);
        Task<ConsultationQADto> ConsultQAQuestionAsync(Guid utilisateurId, CreateConsultationQADto consultationDto);
        Task<IEnumerable<ConsultationQADto>> GetUserConsultationsAsync(Guid utilisateurId);
        Task<IEnumerable<QAQuestionDto>> GetQAQuestionsForRevisionAsync(Guid utilisateurId, NiveauEnum niveau, Guid niveauId);
    }
}

