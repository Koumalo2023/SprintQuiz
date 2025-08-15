using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly SprintQuizDbContext _context;

        public QuizRepository(SprintQuizDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quiz>> GetAllAsync()
        {
            return await _context.Quizzes
                .OrderBy(q => q.DateCreation)
                .ToListAsync();
        }

        public async Task<Quiz?> GetByIdAsync(Guid id)
        {
            return await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Quiz?> GetByIdWithQuestionsAsync(Guid id)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Quiz>> GetByNiveauAsync(NiveauEnum niveau, Guid niveauId)
        {
            return await _context.Quizzes
                .Where(q => q.Niveau == niveau && q.NiveauId == niveauId)
                .OrderBy(q => q.DateCreation)
                .ToListAsync();
        }

        public async Task<Quiz> CreateAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return false;

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Quizzes.AnyAsync(q => q.Id == id);
        }


        // --- Gestion des Questions ---
        public async Task<IEnumerable<QCMQuestion>> GetQuestionsByQuizIdAsync(Guid quizId)
        {
            return await _context.QCMQuestions
                .Where(q => q.QuizId == quizId)
                .Include(q => q.Options)
                .OrderBy(q => q.Id)
                .ToListAsync();
        }

        public async Task<QCMQuestion?> GetQuestionByIdAsync(Guid id)
        {
            return await _context.QCMQuestions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<QCMQuestion> CreateQuestionAsync(QCMQuestion question)
        {
            _context.QCMQuestions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<QCMQuestion> UpdateQuestionAsync(QCMQuestion question)
        {
            _context.QCMQuestions.Update(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<bool> DeleteQuestionAsync(Guid id)
        {
            var question = await _context.QCMQuestions.FindAsync(id);
            if (question == null) return false;
            _context.QCMQuestions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Gestion des Options ---
        public async Task<IEnumerable<QCMOption>> GetOptionsByQuestionIdAsync(Guid questionId)
        {
            return await _context.QCMOptions
                .Where(o => o.QuestionId == questionId)
                .ToListAsync();
        }

        public async Task<QCMOption?> GetOptionByIdAsync(Guid id)
        {
            return await _context.QCMOptions.FindAsync(id);
        }

        public async Task<QCMOption> CreateOptionAsync(QCMOption option)
        {
            _context.QCMOptions.Add(option);
            await _context.SaveChangesAsync();
            return option;
        }

        public async Task<QCMOption> UpdateOptionAsync(QCMOption option)
        {
            _context.QCMOptions.Update(option);
            await _context.SaveChangesAsync();
            return option;
        }

        public async Task<bool> DeleteOptionAsync(Guid id)
        {
            var option = await _context.QCMOptions.FindAsync(id);
            if (option == null) return false;
            _context.QCMOptions.Remove(option);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

