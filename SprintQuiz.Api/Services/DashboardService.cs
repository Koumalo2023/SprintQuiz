using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models; 
using System.Globalization;

namespace SprintQuiz.Services
{
    public class DashboardService
    {
        private readonly SprintQuizDbContext _context;

        public DashboardService(SprintQuizDbContext context)
        {
            _context = context;
        }

        // Dasboard personnalisé pour l'utilisateur connecté
        public async Task<DashboardDto> GetDashboardAsync(Guid utilisateurId)
        {
            var dto = new DashboardDto();

            // Dernière session
            var derniereActivite = await _context.ProgressionsUtilisateur
                .Where(p => p.UtilisateurId == utilisateurId)
                .OrderByDescending(p => p.DerniereActivite)
                .Select(p => p.DerniereActivite) // C'est déjà un DateTime?
                .FirstOrDefaultAsync();

            dto.DerniereSession = FormatDateRelative(derniereActivite);

            // Temps de révision aujourd'hui
            var aujourd = DateTime.Today;
            var demain = aujourd.AddDays(1);
            dto.TempsRevisionAujourdhui = await CalculateTempsRevisionJour(utilisateurId, aujourd, demain);

            // Streak
            dto.Streak = await CalculateStreak(utilisateurId);

            // Récupérer l'utilisateur pour ses objectifs
            var utilisateur = await _context.Utilisateurs.FindAsync(utilisateurId);
            if (utilisateur == null)
                throw new KeyNotFoundException("Utilisateur non trouvé.");

            // Objectifs
            dto.ObjectifHebdomadaireQuiz.Objectif = utilisateur.ObjectifHebdomadaireQuiz;
            dto.ObjectifHebdomadaireQuiz.Realise = await _context.TentativesQuiz
                .CountAsync(t => t.UtilisateurId == utilisateurId && t.Date >= DateTime.Today.AddDays(-7));

            dto.ObjectifHebdomadaireFlashcards.Objectif = utilisateur.ObjectifHebdomadaireFlashcards;
            dto.ObjectifHebdomadaireFlashcards.Realise = await _context.ConsultationsQA
                .CountAsync(c => c.UtilisateurId == utilisateurId && c.DateConsultation >= DateTime.Today.AddDays(-7));

            dto.ObjectifHebdomadaireExercices.Objectif = utilisateur.ObjectifHebdomadaireExercices;
            dto.ObjectifHebdomadaireExercices.Realise = await _context.ConsultationsExercice
                .CountAsync(c => c.UtilisateurId == utilisateurId && c.DateConsultation >= DateTime.Today.AddDays(-7));

            dto.ObjectifTempsRevision.Objectif = utilisateur.ObjectifTempsRevision;
            dto.ObjectifTempsRevision.Realise = await GetTempsRevisionDerniereSemaine(utilisateurId);

            // Prochaines révisions
            dto.ProchainesRevisions = await GetProchainesRevisions(utilisateurId);

            // Historique 7 jours
            dto.HistoriqueRevision = await GetHistorique7Jours(utilisateurId);

            // Progression globale
            var progressions = await _context.ProgressionsUtilisateur
                .Where(p => p.UtilisateurId == utilisateurId)
                .Select(p => p.PourcentageComplet)
                .ToListAsync();

            dto.ProgressionGlobale = progressions.Any() ? progressions.Average() : 0;

            return dto;
        }


        /// <summary>
        /// Formate une date en "Aujourd'hui", "Hier", ou "dd MMM"
        /// </summary>
        private string FormatDateRelative(DateTime? date)
        {
            if (!date.HasValue) return "Jamais";

            var now = DateTime.UtcNow;
            var ts = now - date.Value;

            return ts.TotalHours switch
            {
                <= 24 => "Aujourd'hui",
                <= 48 => "Hier",
                _ => date.Value.ToString("dd MMM", CultureInfo.CurrentUICulture)
            };
        }

        /// <summary>
        /// Calcule le temps de révision en minutes pour une plage horaire
        /// </summary>
        private async Task<int> CalculateTempsRevisionJour(Guid utilisateurId, DateTime start, DateTime end)
        {
            // 1. Temps passé dans les quiz
            var quizTime = await _context.TentativesQuiz
                .Where(t => t.UtilisateurId == utilisateurId && t.Date >= start && t.Date < end)
                .SumAsync(t => (int)t.TempsPasse.TotalMinutes);

            // 2. Temps flashcards (1 min par consultation)
            var qaCount = await _context.ConsultationsQA
                .Where(c => c.UtilisateurId == utilisateurId && c.DateConsultation >= start && c.DateConsultation < end)
                .CountAsync();
            var qaTime = qaCount * 1;

            // 3. Temps exercices (estimé par type)
            var exerciceTime = await (
                from c in _context.ConsultationsExercice
                join e in _context.Exercices on c.ExerciceId equals e.Id
                where c.UtilisateurId == utilisateurId
                      && c.DateConsultation >= start
                      && c.DateConsultation < end
                select GetEstimatedTimeForExercice(e.Type)
            ).SumAsync();

            return quizTime + qaTime + exerciceTime;
        }

        /// <summary>
        /// Temps estimé par type d'exercice
        /// </summary>
        private int GetEstimatedTimeForExercice(TypeExercice type)
        {
            return type switch
            {
                TypeExercice.Basique => 2,
                TypeExercice.Applique => 4,
                TypeExercice.Analyse or TypeExercice.Cas => 7,
                TypeExercice.Defi => 10,
                _ => 2
            };
        }

        /// <summary>
        /// Calcule le nombre de jours consécutifs d'activité
        /// </summary>
        private async Task<int> CalculateStreak(Guid utilisateurId)
        {
            var activityDates = await (
                from p in _context.ProgressionsUtilisateur
                where p.UtilisateurId == utilisateurId && p.DerniereActivite.HasValue
                select p.DerniereActivite!.Value.Date
            ).Distinct().OrderByDescending(d => d).ToListAsync();

            if (!activityDates.Any()) return 0;

            var today = DateTime.Today;
            var current = today;
            int streak = 0;

            foreach (var date in activityDates)
            {
                if (date == current)
                {
                    streak++;
                    current = current.AddDays(-1);
                }
                else if (date < current)
                {
                    break; // Trou dans la séquence
                }
            }

            return streak;
        }

        /// <summary>
        /// Temps total de révision sur les 7 derniers jours
        /// </summary>
        private async Task<TimeSpan> GetTempsRevisionDerniereSemaine(Guid utilisateurId)
        {
            var semaine = DateTime.Today.AddDays(-7);

            var quizMinutes = await _context.TentativesQuiz
                .Where(t => t.UtilisateurId == utilisateurId && t.Date >= semaine)
                .SumAsync(t => (int)t.TempsPasse.TotalMinutes);

            var qaCount = await _context.ConsultationsQA
                .Where(c => c.UtilisateurId == utilisateurId && c.DateConsultation >= semaine)
                .CountAsync();
            var qaMinutes = qaCount * 1;

            var exerciceMinutes = await (
                from c in _context.ConsultationsExercice
                join e in _context.Exercices on c.ExerciceId equals e.Id
                where c.UtilisateurId == utilisateurId && c.DateConsultation >= semaine
                select GetEstimatedTimeForExercice(e.Type)
            ).SumAsync();

            var totalMinutes = quizMinutes + qaMinutes + exerciceMinutes;
            return TimeSpan.FromMinutes(totalMinutes);
        }

        /// <summary>
        /// Prochaines révisions : flashcards non comprises
        /// </summary>
        private async Task<List<DashboardRevisionItemDto>> GetProchainesRevisions(Guid utilisateurId)
        {
            var nonComprises = await _context.ConsultationsQA
                .Where(c => c.UtilisateurId == utilisateurId && c.MarqueeComprise == false)
                .Select(c => c.QAQuestionId)
                .ToListAsync();

            var questions = await _context.QAQuestions
                .Where(q => nonComprises.Contains(q.Id))
                .Take(3)
                .Select(q => new DashboardRevisionItemDto
                {
                    Type = "flashcard",
                    Nom = q.Question.Length > 50 ? q.Question.Substring(0, 47) + "..." : q.Question,
                    Niveau = q.Niveau.ToString(),
                    Priorite = "haute"
                })
                .ToListAsync();

            return questions;
        }

        /// <summary>
        /// Historique des 7 derniers jours
        /// </summary>
        private async Task<List<DashboardJourDto>> GetHistorique7Jours(Guid utilisateurId)
        {
            var jours = new List<DashboardJourDto>();
            var date = DateTime.Today.AddDays(-6);

            for (int i = 0; i < 7; i++)
            {
                var start = date;
                var end = start.AddDays(1);
                var minutes = await CalculateTempsRevisionJour(utilisateurId, start, end);
                jours.Add(new DashboardJourDto { Date = start, Minutes = minutes });
                date = date.AddDays(1);
            }

            return jours;
        }
    }
}