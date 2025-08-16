// Repositories/FormationRepository.cs
using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Repositories; 

namespace SprintQuiz.Data.Repositories
{
    public class FormationRepository : IFormationRepository
    {
        private readonly SprintQuizDbContext _context;

        public FormationRepository(SprintQuizDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Formation>> GetAllAsync()
        {
            return await _context.Formations
                .Include(f => f.Sprints)
                    .ThenInclude(s => s.Modules)
                        .ThenInclude(m => m.Cours)
                .OrderBy(f => f.Ordre)
                .ToListAsync();
        }

        public async Task<Formation?> GetByIdAsync(Guid id)
        {
            return await _context.Formations
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Formation?> GetByIdWithSprintsAsync(Guid id)
        {
            return await _context.Formations
                .Include(f => f.Sprints)
                    .ThenInclude(s => s.Modules)
                        .ThenInclude(m => m.Cours)
                .Include(f => f.Sprints)
                    .ThenInclude(s => s.Quizzes)
                .Include(f => f.Sprints)
                    .ThenInclude(s => s.QAQuestions)
                .Include(f => f.Sprints)
                    .ThenInclude(s => s.Exercices)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Formation> CreateAsync(Formation formation)
        {
            formation.Id = formation.Id == Guid.Empty ? Guid.NewGuid() : formation.Id;
            formation.DateCreation = DateTime.UtcNow;

            _context.Formations.Add(formation);
            await _context.SaveChangesAsync();
            return formation;
        }

        public async Task<Formation> UpdateAsync(Formation formation)
        {
            formation.DerniereModification = DateTime.UtcNow;
            _context.Formations.Update(formation);
            await _context.SaveChangesAsync();
            return formation;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var formation = await _context.Formations.FindAsync(id);
            if (formation == null) return false;

            _context.Formations.Remove(formation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Formations.AnyAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<Formation>> GetByOrdreAsync()
        {
            return await _context.Formations
                .OrderBy(f => f.Ordre)
                .ToListAsync();
        }
    }
}