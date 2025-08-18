using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Services
{
    public class QAQuestionService : IQAQuestionService
    {
        private readonly SprintQuizDbContext _context;
        private readonly IMapper _mapper;

        public QAQuestionService(SprintQuizDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QAQuestionDto>> GetAllQAQuestionsAsync()
        {
            var questions = await _context.QAQuestions
                .OrderBy(q => q.DateCreation)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QAQuestionDto>>(questions);
        }

        public async Task<QAQuestionDto?> GetQAQuestionByIdAsync(Guid id, Guid? utilisateurId = null)
        {
            var question = await _context.QAQuestions.FindAsync(id);
            if (question == null) return null;

            var dto = _mapper.Map<QAQuestionDto>(question);

            if (utilisateurId.HasValue)
            {
                var progression = await _context.ProgressionsUtilisateur
                    .FirstOrDefaultAsync(p => p.UtilisateurId == utilisateurId.Value
                                           && p.Niveau == NiveauEnum.QAQuestion
                                           && p.NiveauId == id);
                dto.DerniereActivite = progression?.DerniereActivite;

                //  Ajout : Statut de maîtrise
                var derniereConsultation = await _context.ConsultationsQA
                    .Where(c => c.UtilisateurId == utilisateurId.Value && c.QAQuestionId == id)
                    .OrderByDescending(c => c.DateConsultation)
                    .FirstOrDefaultAsync();

                dto.EstCompris = derniereConsultation?.MarqueeComprise;
            }

            return dto;
        }
        public async Task<IEnumerable<QAQuestionDto>> GetQAQuestionsByNiveauAsync(NiveauEnum niveau, Guid niveauId)
        {
            var questions = await _context.QAQuestions
                .Where(q => q.Niveau == niveau && q.NiveauId == niveauId)
                .OrderBy(q => q.DateCreation)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QAQuestionDto>>(questions);
        }

        public async Task<QAQuestionDto> CreateQAQuestionAsync(CreateQAQuestionDto createDto)
        {
            var question = _mapper.Map<QAQuestion>(createDto);
            question.Id = Guid.NewGuid();
            question.DateCreation = DateTime.UtcNow;
            question.DureeEstimee = CalculateEstimatedTimeForQA();

            _context.QAQuestions.Add(question);
            await _context.SaveChangesAsync();
            return _mapper.Map<QAQuestionDto>(question);
        }

        // --- Dans UpdateQAQuestionAsync ---
        public async Task<QAQuestionDto?> UpdateQAQuestionAsync(Guid id, UpdateQAQuestionDto updateDto)
        {
            var question = await _context.QAQuestions.FindAsync(id);
            if (question == null) return null;

            _mapper.Map(updateDto, question);
            question.DerniereModification = DateTime.UtcNow;
            question.DureeEstimee = CalculateEstimatedTimeForQA();

            _context.QAQuestions.Update(question);
            await _context.SaveChangesAsync();
            return _mapper.Map<QAQuestionDto>(question);
        }

        public async Task<bool> DeleteQAQuestionAsync(Guid id)
        {
            var question = await _context.QAQuestions.FindAsync(id);
            if (question == null) return false;

            _context.QAQuestions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ConsultationQADto> ConsultQAQuestionAsync(Guid utilisateurId, CreateConsultationQADto consultationDto)
        {
            var consultation = new ConsultationQA
            {
                Id = Guid.NewGuid(),
                UtilisateurId = utilisateurId,
                QAQuestionId = consultationDto.QAQuestionId,
                DateConsultation = DateTime.UtcNow,
                MarqueeComprise = consultationDto.MarqueeComprise
            };

            _context.ConsultationsQA.Add(consultation);
            await _context.SaveChangesAsync();

            // Mettre à jour la progression de l'utilisateur
            await UpdateUserProgressionAsync(utilisateurId, consultationDto.QAQuestionId);

            return _mapper.Map<ConsultationQADto>(consultation);
        }

        public async Task<IEnumerable<ConsultationQADto>> GetUserConsultationsAsync(Guid utilisateurId)
        {
            var consultations = await _context.ConsultationsQA
                .Where(c => c.UtilisateurId == utilisateurId)
                .OrderByDescending(c => c.DateConsultation)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ConsultationQADto>>(consultations);
        }

        public async Task<IEnumerable<QAQuestionDto>> GetQAQuestionsForRevisionAsync(
    Guid utilisateurId,
    NiveauEnum niveau,
    Guid niveauId,
    int limit = 10)
        {
            // Récupérer les questions du niveau
            var questions = await _context.QAQuestions
                .Where(q => q.Niveau == niveau && q.NiveauId == niveauId)
                .ToListAsync();

            // Récupérer les consultations de l'utilisateur
            var questionIds = questions.Select(q => q.Id).ToList();

            var consultations = await _context.ConsultationsQA
                .Where(c => questionIds.Contains(c.QAQuestionId))
                .ToListAsync();

            // Associer chaque question à sa dernière consultation
            var questionConsultMap = consultations
                .GroupBy(c => c.QAQuestionId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(c => c.DateConsultation).First());

            // Tri intelligent :
            // 1. Non consultées
            // 2. Consultées mais non comprises
            // 3. Consultées et comprises → triées par ancienneté (spaced repetition)
            var questionsForRevision = questions
                .Select(q =>
                {
                    questionConsultMap.TryGetValue(q.Id, out var consult);
                    return new
                    {
                        Question = q,
                        Consult = consult
                    };
                })
                .OrderBy(x => x.Consult == null ? 0 : (x.Consult.MarqueeComprise == true ? 2 : 1)) // Non consultée < non comprise < comprise
                .ThenBy(x => x.Consult?.DateConsultation) // Anciennes en premier
                .Take(limit) //  Séries de révision
                .Select(x => _mapper.Map<QAQuestionDto>(x.Question))
                .ToList();

            // Ajouter EstCompris
            foreach (var dto in questionsForRevision)
            {
                var consult = questionConsultMap.GetValueOrDefault(dto.Id);
                dto.EstCompris = consult?.MarqueeComprise;
            }

            return questionsForRevision;
        }

        private async Task UpdateUserProgressionAsync(Guid utilisateurId, Guid qaQuestionId)
        {
            var question = await _context.QAQuestions.FindAsync(qaQuestionId);
            if (question == null) return;

            // Vérifier si une progression existe déjà pour ce niveau
            var progression = await _context.ProgressionsUtilisateur
                .FirstOrDefaultAsync(p => p.UtilisateurId == utilisateurId &&
                                         p.Niveau == question.Niveau &&
                                         p.NiveauId == question.NiveauId);

            if (progression == null)
            {
                // Créer une nouvelle progression
                progression = new ProgressionUtilisateur
                {
                    Id = Guid.NewGuid(),
                    UtilisateurId = utilisateurId,
                    Niveau = question.Niveau,
                    NiveauId = question.NiveauId,
                    PourcentageComplet = 0,
                    DerniereActivite = DateTime.UtcNow
                };
                _context.ProgressionsUtilisateur.Add(progression);
            }

            // Calculer le nouveau pourcentage basé sur les consultations
            var totalQuestions = await _context.QAQuestions
                .CountAsync(q => q.Niveau == question.Niveau && q.NiveauId == question.NiveauId);

            var consultedQuestions = await _context.ConsultationsQA
                .Where(c => c.UtilisateurId == utilisateurId)
                .Join(_context.QAQuestions,
                      c => c.QAQuestionId,
                      q => q.Id,
                      (c, q) => new { c, q })
                .Where(cq => cq.q.Niveau == question.Niveau && cq.q.NiveauId == question.NiveauId)
                .CountAsync();

            progression.PourcentageComplet = totalQuestions > 0 ? (float)consultedQuestions / totalQuestions : 0;
            progression.DerniereActivite = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // --- Méthode privée : Calcul de la durée estimée ---
        private int CalculateEstimatedTimeForQA()
        {
            return 1; // 1 minute par flashcard (peut être ajusté)
        }
    }
}

