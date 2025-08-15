using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Services
{
    public interface IQuizService
    {
        Task<IEnumerable<QuizDto>> GetAllQuizzesAsync();
        Task<QuizDto?> GetQuizByIdAsync(Guid id);
        Task<QuizDto?> GetQuizWithQuestionsAsync(Guid id);
        Task<IEnumerable<QuizDto>> GetQuizzesByNiveauAsync(NiveauEnum niveau, Guid niveauId);
        Task<QuizDto> CreateQuizAsync(CreateQuizDto createQuizDto);
        Task<QuizDto?> UpdateQuizAsync(Guid id, UpdateQuizDto updateQuizDto);
        Task<bool> DeleteQuizAsync(Guid id);
        Task<QuizResultDto> SubmitQuizAsync(Guid utilisateurId, CreateTentativeQuizDto tentativeDto);
        Task<IEnumerable<TentativeQuizDto>> GetUserQuizAttemptsAsync(Guid utilisateurId);
        

        // --- Gestion des Questions ---
        Task<IEnumerable<QCMQuestionDto>> GetQuestionsByQuizIdAsync(Guid quizId);
        Task<QCMQuestionDto?> GetQuestionByIdAsync(Guid id);
        Task<QCMQuestionDto> CreateQuestionAsync(CreateQCMQuestionDto createDto);
        Task<QCMQuestionDto?> UpdateQuestionAsync(Guid id, UpdateQCMQuestionDto updateDto);
        Task<bool> DeleteQuestionAsync(Guid id);

        // --- Gestion des Options ---
        Task<IEnumerable<QCMOptionDto>> GetOptionsByQuestionIdAsync(Guid questionId);
        Task<QCMOptionDto> CreateOptionAsync(CreateQCMOptionDto createDto);
        Task<QCMOptionDto?> UpdateOptionAsync(Guid id, UpdateQCMOptionDto updateDto);
        Task<bool> DeleteOptionAsync(Guid id);

        //  Ajout manquant : Récupérer une option par ID
        Task<QCMOptionDto?> GetOptionByIdAsync(Guid id);
    }
}

