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
                .Include(m => m.Sprint)
                .Include(m => m.Cours.OrderBy(c => c.Ordre))
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
            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            return module;
        }

        public async Task<Module> UpdateAsync(Module module)
        {
            _context.Modules.Update(module);
            await _context.SaveChangesAsync();
            return module;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null) return false;

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

