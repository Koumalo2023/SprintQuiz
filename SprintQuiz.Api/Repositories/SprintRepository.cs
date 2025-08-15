using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Repositories
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
                .OrderBy(s => s.Ordre)
                .ToListAsync();
        }

        public async Task<Sprint?> GetByIdAsync(Guid id)
        {
            return await _context.Sprints
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sprint?> GetByIdWithModulesAsync(Guid id)
        {
            return await _context.Sprints
                .Include(s => s.Modules.OrderBy(m => m.Ordre))
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sprint> CreateAsync(Sprint sprint)
        {
            _context.Sprints.Add(sprint);
            await _context.SaveChangesAsync();
            return sprint;
        }

        public async Task<Sprint> UpdateAsync(Sprint sprint)
        {
            _context.Sprints.Update(sprint);
            await _context.SaveChangesAsync();
            return sprint;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sprint = await _context.Sprints.FindAsync(id);
            if (sprint == null) return false;

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

