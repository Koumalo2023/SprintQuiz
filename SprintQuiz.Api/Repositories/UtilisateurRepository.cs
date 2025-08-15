using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Repositories
{
    public class UtilisateurRepository : IUtilisateurRepository
    {
        private readonly SprintQuizDbContext _context;

        public UtilisateurRepository(SprintQuizDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Utilisateur>> GetAllAsync()
        {
            return await _context.Utilisateurs
                .OrderBy(u => u.DateInscription)
                .ToListAsync();
        }

        public async Task<Utilisateur?> GetByIdAsync(Guid id)
        {
            return await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Utilisateur?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Utilisateurs
                .Include(u => u.StatistiquesGlobales)
                .Include(u => u.Progressions)
                .Include(u => u.TentativesQuiz)
                    .ThenInclude(t => t.Quiz)
                .Include(u => u.ConsultationsQA)
                    .ThenInclude(c => c.QAQuestion)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Utilisateur?> GetByEmailAsync(string email)
        {
            return await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Utilisateur> CreateAsync(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();
            return utilisateur;
        }

        public async Task<Utilisateur> UpdateAsync(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Update(utilisateur);
            await _context.SaveChangesAsync();
            return utilisateur;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null) return false;

            _context.Utilisateurs.Remove(utilisateur);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Utilisateurs.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Utilisateurs.AnyAsync(u => u.Email == email);
        }
    }
}

