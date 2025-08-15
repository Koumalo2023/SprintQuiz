using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Repositories
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetAllAsync();
        Task<Quiz?> GetByIdAsync(Guid id);
        Task<Quiz?> GetByIdWithQuestionsAsync(Guid id);
        Task<IEnumerable<Quiz>> GetByNiveauAsync(NiveauEnum niveau, Guid niveauId);
        Task<Quiz> CreateAsync(Quiz quiz); 
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);

        // --- Gestion des Questions ---
        Task<IEnumerable<QCMQuestion>> GetQuestionsByQuizIdAsync(Guid quizId);
        Task<QCMQuestion?> GetQuestionByIdAsync(Guid id);
        Task<QCMQuestion> CreateQuestionAsync(QCMQuestion question);
        Task<QCMQuestion> UpdateQuestionAsync(QCMQuestion question);
        Task<bool> DeleteQuestionAsync(Guid id);

        // --- Gestion des Options ---
        Task<IEnumerable<QCMOption>> GetOptionsByQuestionIdAsync(Guid questionId);
        Task<QCMOption?> GetOptionByIdAsync(Guid id);
        Task<QCMOption> CreateOptionAsync(QCMOption option);
        Task<QCMOption> UpdateOptionAsync(QCMOption option);
        Task<bool> DeleteOptionAsync(Guid id);
    }
}

