using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Repositories
{
    public class CoursRepository : ICoursRepository
    {
        private readonly SprintQuizDbContext _context;

        public CoursRepository(SprintQuizDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cours>> GetAllAsync()
        {
            return await _context.Cours
                .Include(c => c.Module)
                .ThenInclude(m => m.Sprint)
                .OrderBy(c => c.Module.Sprint.Ordre)
                .ThenBy(c => c.Module.Ordre)
                .ThenBy(c => c.Ordre)
                .ToListAsync();
        }

        public async Task<Cours?> GetByIdAsync(Guid id)
        {
            return await _context.Cours
                .Include(c => c.Module)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Cours>> GetByModuleIdAsync(Guid moduleId)
        {
            return await _context.Cours
                .Include(c => c.Module)
                .Where(c => c.ModuleId == moduleId)
                .OrderBy(c => c.Ordre)
                .ToListAsync();
        }

        public async Task<Cours> CreateAsync(Cours cours)
        {
            _context.Cours.Add(cours);
            await _context.SaveChangesAsync();
            return cours;
        }

        public async Task<Cours> UpdateAsync(Cours cours)
        {
            _context.Cours.Update(cours);
            await _context.SaveChangesAsync();
            return cours;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var cours = await _context.Cours.FindAsync(id);
            if (cours == null) return false;

            _context.Cours.Remove(cours);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Cours.AnyAsync(c => c.Id == id);
        }
    }
}

