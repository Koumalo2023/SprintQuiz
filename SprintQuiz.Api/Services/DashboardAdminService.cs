// Services/DashboardAdminService.cs
using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Services;

namespace SprintQuiz.Services
{
    public class DashboardAdminService : IDashboardAdminService
    {
        private readonly SprintQuizDbContext _context;

        public DashboardAdminService(SprintQuizDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardOverviewDto> GetOverviewAsync()
        {
            var today = DateTime.Today;
            var weekAgo = today.AddDays(-7);

            var totalUtilisateurs = await _context.Utilisateurs.CountAsync();
            var actifsCetteSemaine = await _context.ProgressionsUtilisateur
                .Where(p => p.DerniereActivite >= weekAgo)
                .Select(p => p.UtilisateurId)
                .Distinct()
                .CountAsync();

            var progressionMoyenne = await _context.ProgressionsUtilisateur
                .AverageAsync(p => (double)p.PourcentageComplet);

            var tempsRevisionQuery = from p in _context.ProgressionsUtilisateur
                                     where p.DerniereActivite >= weekAgo
                                     group p by p.UtilisateurId into g
                                     select new { Minutes = g.Count() * 5 }; // Estimation

            var tempsTotal = await tempsRevisionQuery.SumAsync(t => t.Minutes);
            var tempsMoyen = totalUtilisateurs > 0 ? tempsTotal / totalUtilisateurs : 0;

            var nouveaux = await _context.Utilisateurs
                .CountAsync(u => u.DateInscription >= weekAgo);

            return new DashboardOverviewDto
            {
                TotalUtilisateurs = totalUtilisateurs,
                ActifsCetteSemaine = actifsCetteSemaine,
                TauxEngagement = totalUtilisateurs > 0 ? (double)actifsCetteSemaine / totalUtilisateurs * 100 : 0,
                ProgressionMoyenneGlobale = (float)progressionMoyenne,
                TempsMoyenRevisionHebdo = tempsMoyen,
                NouveauxUtilisateurs = nouveaux
            };
        }

        public async Task<DashboardProgressionDto> GetProgressionParNiveauAsync(NiveauEnum niveau, Guid niveauId)
        {
            var nom = await GetNomNiveau(niveau, niveauId);
            var progressions = await _context.ProgressionsUtilisateur
                .Where(p => p.Niveau == niveau && p.NiveauId == niveauId)
                .ToListAsync();

            var completions = progressions.Count(p => p.PourcentageComplet >= 100);
            var enCours = progressions.Count(p => p.PourcentageComplet > 0 && p.PourcentageComplet < 100);
            var nonCommences = progressions.Count(p => p.PourcentageComplet == 0);

            var progressionMoyenne = progressions.Any() ? progressions.Average(p => p.PourcentageComplet) : 0;

            return new DashboardProgressionDto
            {
                Nom = nom,
                ProgressionMoyenne = (float)progressionMoyenne,
                Completions = completions,
                EnCours = enCours,
                NonCommences = nonCommences
            };
        }

        private async Task<string> GetNomNiveau(NiveauEnum niveau, Guid niveauId)
        {
            return niveau switch
            {
                NiveauEnum.Formation => await _context.Formations.Where(f => f.Id == niveauId).Select(f => f.Nom).FirstOrDefaultAsync() ?? "Inconnu",
                NiveauEnum.Sprint => await _context.Sprints.Where(s => s.Id == niveauId).Select(s => s.Nom).FirstOrDefaultAsync() ?? "Inconnu",
                NiveauEnum.Module => await _context.Modules.Where(m => m.Id == niveauId).Select(m => m.Nom).FirstOrDefaultAsync() ?? "Inconnu",
                NiveauEnum.Cours => await _context.Cours.Where(c => c.Id == niveauId).Select(c => c.Nom).FirstOrDefaultAsync() ?? "Inconnu",
                _ => "Inconnu"
            };
        }

        public async Task<QuizAnalyseDto> GetAnalyseQuizAsync(Guid quizId)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null) throw new KeyNotFoundException("Quiz non trouvé");

            var tentatives = await _context.TentativesQuiz
                .Where(t => t.QuizId == quizId)
                .Include(t => t.Reponses)
                .ThenInclude(r => r.Question)
                .ToListAsync();

            if (!tentatives.Any())
                return new QuizAnalyseDto { QuizTitre = quiz.Titre, TauxReussiteGlobal = 0, MoyenneScore = 0 };

            var moyenneScore = tentatives.Average(t => t.Score);
            var tauxReussite = tentatives.Count(t => t.Reussi) / (double)tentatives.Count();

            var questions = await _context.QCMQuestions
                .Where(q => q.QuizId == quizId)
                .Include(q => q.Options)
                .ToDictionaryAsync(q => q.Id);

            var questionsRatees = questions.Keys
                .Select(qId =>
                {
                    var erreurCount = tentatives.Count(t =>
                        t.Reponses.All(r => r.QuestionId != qId) || // Pas de réponse
                        t.Reponses.Where(r => r.QuestionId == qId)
                                 .Select(r => r.OptionId)
                                 .Join(questions[qId].Options.Where(o => o.EstCorrecte),
                                       id => id, o => o.Id, (id, o) => o)
                                 .Any() == false); // Aucune bonne réponse
                    return new
                    {
                        Intitule = questions[qId].Intitule,
                        TauxErreur = tentatives.Count > 0 ? (double)erreurCount / tentatives.Count : 0
                    };
                })
                .OrderByDescending(q => q.TauxErreur)
                .Take(5)
                .Select(q => new QuestionAnalyseDto
                {
                    Intitule = q.Intitule.Length > 60 ? q.Intitule.Substring(0, 57) + "..." : q.Intitule,
                    TauxErreur = (int)(q.TauxErreur * 100)
                })
                .ToList();

            var distribution = new int[5]; // 0-20, 20-40, 40-60, 60-80, 80-100
            foreach (var t in tentatives)
            {
                var index = Math.Min((int)(t.Score * 100 / 20), 4);
                distribution[index]++;
            }

            return new QuizAnalyseDto
            {
                QuizTitre = quiz.Titre,
                TauxReussiteGlobal = (float)(tauxReussite * 100),
                MoyenneScore = (float)moyenneScore,
                NombreTentatives = tentatives.Count(),
                QuestionsLesPlusRatees = questionsRatees,
                DistributionScores = distribution
            };
        }
        public async Task<AlerteInactifsDto> GetAlertesInactifsAsync(int jours)
        {
            var seuil = DateTime.UtcNow.AddDays(-jours);
            var inactifs = await _context.ProgressionsUtilisateur
                .Where(p => p.DerniereActivite < seuil)
                .GroupBy(p => p.UtilisateurId)
                .Select(g => new
                {
                    Id = g.Key,
                    Derniere = g.Max(p => p.DerniereActivite)
                })
                .Join(_context.Utilisateurs,
                      i => i.Id,
                      u => u.Id,
                      (i, u) => new UtilisateurDashboardDto
                      {
                          Id = u.Id,
                          Nom = u.Nom,
                          DerniereActivite = i.Derniere,
                          ProgressionGlobale = 0 // À compléter si nécessaire
                      })
                .ToListAsync();

            // Ajouter la progression globale
            foreach (var user in inactifs)
            {
                var progression = await _context.ProgressionsUtilisateur
                    .Where(p => p.UtilisateurId == user.Id)
                    .AverageAsync(p => (double?)p.PourcentageComplet);
                user.ProgressionGlobale = (float)(progression ?? 0);
            }

            return new AlerteInactifsDto
            {
                InactifsDepuisPlusDeNJours = inactifs.OrderByDescending(u => u.DerniereActivite).ToList()
            };
        }

        public async Task<AlerteDifficulteDto> GetUtilisateursEnDifficulteAsync(float seuil)
        {
            var utilisateurs = await _context.ProgressionsUtilisateur
                .GroupBy(p => p.UtilisateurId)
                .Select(g => new
                {
                    Id = g.Key,
                    Progression = g.Average(p => p.PourcentageComplet)
                })
                .Where(u => u.Progression < seuil)
                .Join(_context.Utilisateurs,
                      u => u.Id,
                      usr => usr.Id,
                      (u, usr) => new UtilisateurDashboardDto
                      {
                          Id = usr.Id,
                          Nom = usr.Nom,
                          ProgressionGlobale = (float)u.Progression,
                          DerniereActivite = null
                      })
                .ToListAsync();

            // Ajouter la dernière activité
            foreach (var user in utilisateurs)
            {
                var derniere = await _context.ProgressionsUtilisateur
                    .Where(p => p.UtilisateurId == user.Id)
                    .MaxAsync(p => (DateTime?)p.DerniereActivite);
                user.DerniereActivite = derniere;
            }

            return new AlerteDifficulteDto
            {
                UtilisateursFaibleProgression = utilisateurs.OrderByDescending(u => u.ProgressionGlobale).ToList()
            };
        }

        public async Task<HistoriqueActiviteDto> GetHistoriqueAsync(int periode)
        {
            var dates = Enumerable.Range(0, periode)
                .Select(i => DateTime.Today.AddDays(-i))
                .Reverse()
                .ToList();

            var data = new List<HistoriqueJourDto>();

            foreach (var date in dates)
            {
                var start = date;
                var end = date.AddDays(1);

                var actifs = await _context.ProgressionsUtilisateur
                    .Where(p => p.DerniereActivite >= start && p.DerniereActivite < end)
                    .Select(p => p.UtilisateurId)
                    .Distinct()
                    .CountAsync();

                var tentatives = await _context.TentativesQuiz
                    .CountAsync(t => t.Date >= start && t.Date < end);

                data.Add(new HistoriqueJourDto
                {
                    Date = date,
                    UtilisateursActifs = actifs,
                    TentativesQuiz = tentatives
                });
            }

            return new HistoriqueActiviteDto
            {
                ActiviteJournaliere = data
            };
        }

        public async Task<TopFlopDto> GetTopFlopAsync()
        {
            // Top quiz
            var quizScores = await _context.TentativesQuiz
                .GroupBy(t => t.QuizId)
                .Select(g => new
                {
                    QuizId = g.Key,
                    Moyenne = g.Average(t => t.Score),
                    Count = g.Count()
                })
                .Where(q => q.Count >= 5) // Seuil de fiabilité
                .ToListAsync();

            var topQuiz = quizScores
                .OrderByDescending(q => q.Moyenne)
                .Take(1)
                .Select(q => new TopFlopItemDto
                {
                    Titre = _context.Quizzes.Where(qz => qz.Id == q.QuizId).Select(qz => qz.Titre).FirstOrDefault() ?? "Inconnu",
                    Valeur = (int)(q.Moyenne * 100)
                })
                .FirstOrDefault();

            var flopQuiz = quizScores
                .OrderBy(q => q.Moyenne)
                .Take(1)
                .Select(q => new TopFlopItemDto
                {
                    Titre = _context.Quizzes.Where(qz => qz.Id == q.QuizId).Select(qz => qz.Titre).FirstOrDefault() ?? "Inconnu",
                    Valeur = (int)(q.Moyenne * 100)
                })
                .FirstOrDefault();

            // Top module
            var moduleProgression = await _context.ProgressionsUtilisateur
                .Where(p => p.Niveau == NiveauEnum.Module)
                .GroupBy(p => p.NiveauId)
                .Select(g => new
                {
                    ModuleId = g.Key,
                    Moyenne = g.Average(p => p.PourcentageComplet)
                })
                .ToListAsync();

            var topModule = moduleProgression
                .OrderByDescending(m => m.Moyenne)
                .Take(1)
                .Select(m => new TopFlopItemDto
                {
                    Titre = _context.Modules.Where(mod => mod.Id == m.ModuleId).Select(mod => mod.Nom).FirstOrDefault() ?? "Inconnu",
                    Valeur = (int)m.Moyenne
                })
                .FirstOrDefault();

            var flopModule = moduleProgression
                .OrderBy(m => m.Moyenne)
                .Take(1)
                .Select(m => new TopFlopItemDto
                {
                    Titre = _context.Modules.Where(mod => mod.Id == m.ModuleId).Select(mod => mod.Nom).FirstOrDefault() ?? "Inconnu",
                    Valeur = (int)m.Moyenne
                })
                .FirstOrDefault();

            return new TopFlopDto
            {
                Top = new TopSectionDto
                {
                    QuizPlusReussi = topQuiz,
                    ModulePlusConsulte = null // À implémenter si besoin
                },
                Flop = new FlopSectionDto
                {
                    QuizPlusRate = flopQuiz,
                    ModuleMoinsComplet = flopModule
                }
            };
        }
    }
}