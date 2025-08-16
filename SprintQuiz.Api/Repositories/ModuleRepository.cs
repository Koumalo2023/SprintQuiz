using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Repositories
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly SprintQuizDbContext _context;

        public ModuleRepository(SprintQuizDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Module>> GetAllAsync()
        {
            return await _context.Modules
                .Include(m => m.Sprint)
                .OrderBy(m => m.Sprint.Ordre)
                .ThenBy(m => m.Ordre)
                .ToListAsync();
        }

        public async Task<Module?> GetByIdAsync(Guid id)
        {
            return await _context.Modules
                .Include(m => m.Sprint)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Module?> GetByIdWithCoursAsync(Guid id)
        {
            return await _context.Modules
                .Include(m => m.Cours)
                .Include(m => m.Quizzes)
                .Include(m => m.QAQuestions)
                .Include(m => m.Exercices)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Module>> GetBySprintIdAsync(Guid sprintId)
        {
            return await _context.Modules
                .Include(m => m.Sprint)
                .Where(m => m.SprintId == sprintId)
                .OrderBy(m => m.Ordre)
                .ToListAsync();
        }

        public async Task<Module> CreateAsync(Module module)
        {
            module.Id = module.Id == Guid.Empty ? Guid.NewGuid() : module.Id;
            module.DateCreation = DateTime.UtcNow;

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            return module;
        }

        public async Task<Module> UpdateAsync(Module module)
        {
            module.DerniereModification = DateTime.UtcNow;
            _context.Modules.Update(module);
            await _context.SaveChangesAsync();
            return module;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null) return false;

            // Suppression en cascade des contenus polymorphes liés
            var quizToDelete = _context.Quizzes.Where(q => q.Niveau == NiveauEnum.Module && q.NiveauId == id);
            var qaQuestionsToDelete = _context.QAQuestions.Where(q => q.Niveau == NiveauEnum.Module && q.NiveauId == id);
            var exercicesToDelete = _context.Exercices.Where(e => e.Niveau == NiveauEnum.Module && e.NiveauId == id);

            _context.Quizzes.RemoveRange(quizToDelete);
            _context.QAQuestions.RemoveRange(qaQuestionsToDelete);
            _context.Exercices.RemoveRange(exercicesToDelete);

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Modules.AnyAsync(m => m.Id == id);
        }

         
    }
}

