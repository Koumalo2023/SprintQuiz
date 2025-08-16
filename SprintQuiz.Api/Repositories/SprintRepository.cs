// Repositories/SprintRepository.cs
using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Repositories; 

namespace SprintQuiz.Data.Repositories
{
    public class SprintRepository : ISprintRepository
    {
        private readonly SprintQuizDbContext _context;

        public SprintRepository(SprintQuizDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sprint>> GetAllAsync()
        {
            return await _context.Sprints
                .Include(s => s.Modules)
                    .ThenInclude(m => m.Cours)
                .OrderBy(s => s.Ordre)
                .ToListAsync();
        }

        public async Task<Sprint?> GetByIdAsync(Guid id)
        {
            return await _context.Sprints.FindAsync(id);
        }

        public async Task<Sprint?> GetByIdWithModulesAsync(Guid id)
        {
            return await _context.Sprints
                .Include(s => s.Modules)
                    .ThenInclude(m => m.Cours)
                .Include(s => s.Quizzes)
                .Include(s => s.QAQuestions)
                .Include(s => s.Exercices)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sprint> CreateAsync(Sprint sprint)
        {
            sprint.Id = sprint.Id == Guid.Empty ? Guid.NewGuid() : sprint.Id;
            sprint.DateCreation = DateTime.UtcNow;

            _context.Sprints.Add(sprint);
            await _context.SaveChangesAsync();
            return sprint;
        }

        public async Task<Sprint> UpdateAsync(Sprint sprint)
        {
            sprint.DerniereModification = DateTime.UtcNow;
            _context.Sprints.Update(sprint);
            await _context.SaveChangesAsync();
            return sprint;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sprint = await _context.Sprints.FindAsync(id);
            if (sprint == null) return false;

            // Suppression en cascade des contenus polymorphes liés
            var quizToDelete = _context.Quizzes.Where(q => q.Niveau == NiveauEnum.Sprint && q.NiveauId == id);
            var qaQuestionsToDelete = _context.QAQuestions.Where(q => q.Niveau == NiveauEnum.Sprint && q.NiveauId == id);
            var exercicesToDelete = _context.Exercices.Where(e => e.Niveau == NiveauEnum.Sprint && e.NiveauId == id);

            _context.Quizzes.RemoveRange(quizToDelete);
            _context.QAQuestions.RemoveRange(qaQuestionsToDelete);
            _context.Exercices.RemoveRange(exercicesToDelete);

            _context.Sprints.Remove(sprint);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Sprints.AnyAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Sprint>> GetByOrdreAsync()
        {
            return await _context.Sprints
                .OrderBy(s => s.Ordre)
                .ToListAsync();
        }
    }
}