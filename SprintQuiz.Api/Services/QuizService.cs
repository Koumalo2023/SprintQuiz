using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SprintQuiz.Api.Data;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Exceptions;
using SprintQuiz.Api.Middleware;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Repositories;
using System.Data;

namespace SprintQuiz.Api.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly SprintQuizDbContext _context;
        private readonly IMapper _mapper;

        public QuizService(IQuizRepository quizRepository, SprintQuizDbContext context, IMapper mapper)
        {
            _quizRepository = quizRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QuizDto>> GetAllQuizzesAsync()
        {
            var quizzes = await _quizRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<QuizDto>>(quizzes);
        }

        public async Task<QuizDto?> GetQuizByIdAsync(Guid id)
        {
            var quiz = await _quizRepository.GetByIdAsync(id);
            return quiz == null ? null : _mapper.Map<QuizDto>(quiz);
        }

        public async Task<QuizDto?> GetQuizWithQuestionsAsync(Guid id)
        {
            var quiz = await _quizRepository.GetByIdWithQuestionsAsync(id);
            return quiz == null ? null : _mapper.Map<QuizDto>(quiz);
        }

        public async Task<IEnumerable<QuizDto>> GetQuizzesByNiveauAsync(NiveauEnum niveau, Guid niveauId)
        {
            var quizzes = await _quizRepository.GetByNiveauAsync(niveau, niveauId);
            return _mapper.Map<IEnumerable<QuizDto>>(quizzes);
        }

        public async Task<QuizDto> CreateQuizAsync(CreateQuizDto createQuizDto)
        {
            if (!createQuizDto.Questions.Any())
                throw new ArgumentException("Un quiz doit avoir au moins une question.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Valider les questions avant toute opération
                foreach (var questionDto in createQuizDto.Questions)
                {
                    if (!questionDto.Options.Any())
                        throw new ArgumentException($"La question '{questionDto.Intitule}' doit avoir au moins une option.");

                    if (!questionDto.Options.Any(o => o.EstCorrecte))
                        throw new ArgumentException($"La question '{questionDto.Intitule}' doit avoir au moins une bonne réponse.");
                }

                // Créer le quiz
                var quiz = _mapper.Map<Quiz>(createQuizDto);
                quiz.Id = Guid.NewGuid();
                quiz.DateCreation = DateTime.UtcNow;

                // Préparer les questions avec leurs options
                var questions = createQuizDto.Questions.Select(questionDto =>
                {
                    var question = _mapper.Map<QCMQuestion>(questionDto);
                    question.Id = Guid.NewGuid();
                    question.QuizId = quiz.Id;

                    question.Options = questionDto.Options.Select(optionDto =>
                    {
                        var option = _mapper.Map<QCMOption>(optionDto);
                        option.Id = Guid.NewGuid();
                        option.QuestionId = question.Id;
                        return option;
                    }).ToList();

                    return question;
                }).ToList();

                // Ajouter tout en une seule opération
                quiz.Questions = questions;
                _context.Quizzes.Add(quiz);

                // Un seul appel à SaveChanges
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Recharger le quiz avec les relations pour le retour
                var createdQuiz = await _context.Quizzes
                    .Include(q => q.Questions)
                        .ThenInclude(q => q.Options)
                    .FirstOrDefaultAsync(q => q.Id == quiz.Id);

                return _mapper.Map<QuizDto>(createdQuiz);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("Erreur lors de la création du quiz", ex);
            }
        }


        public async Task<QuizDto?> UpdateQuizAsync(Guid id, UpdateQuizDto updateQuizDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //  Charger le quiz sans le suivre (pour éviter le suivi d'état)
                var quiz = await _context.Quizzes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quiz == null) return null;

                //  Validation du DTO
                if (!updateQuizDto.Questions?.Any() ?? true)
                    throw new ArgumentException("Un quiz doit avoir au moins une question.");

                foreach (var q in updateQuizDto.Questions)
                {
                    if (!q.Options.Any()) throw new ArgumentException($"La question '{q.Intitule}' doit avoir des options.");
                    if (!q.Options.Any(o => o.EstCorrecte)) throw new ArgumentException($"La question '{q.Intitule}' doit avoir une bonne réponse.");
                }

                //  Supprimer les anciennes questions
                _context.QCMQuestions.RemoveRange(_context.QCMQuestions.Where(q => q.QuizId == id));

                // ➕ Créer les nouvelles
                var nouvellesQuestions = new List<QCMQuestion>();
                foreach (var questionDto in updateQuizDto.Questions)
                {
                    var question = _mapper.Map<QCMQuestion>(questionDto);
                    question.Id = Guid.NewGuid();
                    question.QuizId = id;

                    question.Options = questionDto.Options.Select(o =>
                    {
                        var option = _mapper.Map<QCMOption>(o);
                        option.Id = Guid.NewGuid();
                        option.QuestionId = question.Id;
                        return option;
                    }).ToList();

                    nouvellesQuestions.Add(question);
                }

                _context.QCMQuestions.AddRange(nouvellesQuestions);

                //  Sauvegarder → si Version a changé, échec (concurrent update)
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                //  Recharger pour retourner
                var updatedQuiz = await _context.Quizzes
                    .Include(q => q.Questions).ThenInclude(q => q.Options)
                    .FirstOrDefaultAsync(q => q.Id == id);

                return _mapper.Map<QuizDto>(updatedQuiz);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                if (!await _context.Quizzes.AnyAsync(q => q.Id == id))
                    throw new ArgumentException("Le quiz a été supprimé.");
                else
                    throw new InvalidOperationException("Le quiz a été modifié par un autre utilisateur. Veuillez recharger.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> DeleteQuizAsync(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Vérifier l'existence du quiz sans le tracker
                var quizExists = await _context.Quizzes
                    .AsNoTracking()
                    .AnyAsync(q => q.Id == id);

                if (!quizExists) return false;

                // 2. Suppression en cascade optimisée (EF Core 7+)
                // D'abord les options
                await _context.QCMOptions
                    .Where(o => _context.QCMQuestions
                        .Where(q => q.QuizId == id)
                        .Select(q => q.Id)
                        .Contains(o.QuestionId))
                    .ExecuteDeleteAsync();

                // Ensuite les questions
                await _context.QCMQuestions
                    .Where(q => q.QuizId == id)
                    .ExecuteDeleteAsync();

                // Puis les tentatives
                await _context.TentativesQuiz
                    .Where(t => t.QuizId == id)
                    .ExecuteDeleteAsync();

                // Enfin le quiz lui-même
                var rowsAffected = await _context.Quizzes
                    .Where(q => q.Id == id)
                    .ExecuteDeleteAsync();

                await transaction.CommitAsync();

                // Si aucune ligne affectée, le quiz a été supprimé entre-temps
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("Erreur lors de la suppression du quiz", ex);
            }
        }

        public async Task<QuizResultDto> SubmitQuizAsync(Guid utilisateurId, CreateTentativeQuizDto tentativeDto)
        {
            var quiz = await _quizRepository.GetByIdWithQuestionsAsync(tentativeDto.QuizId);
            if (quiz == null) throw new ArgumentException("Quiz non trouvé");

            var tentative = new TentativeQuiz
            {
                Id = Guid.NewGuid(),
                UtilisateurId = utilisateurId,
                QuizId = tentativeDto.QuizId,
                Date = DateTime.UtcNow,
                TempsPasse = tentativeDto.TempsPasse
            };

            var reponses = new List<ReponseUtilisateurQCM>();
            var reponsesDto = new List<ReponseQuestionDto>();
            int correctAnswers = 0;

            foreach (var reponseDto in tentativeDto.Reponses)
            {
                var question = quiz.Questions.FirstOrDefault(q => q.Id == reponseDto.QuestionId);
                var option = question?.Options.FirstOrDefault(o => o.Id == reponseDto.OptionId);
                
                if (question != null && option != null)
                {
                    var reponse = new ReponseUtilisateurQCM
                    {
                        Id = Guid.NewGuid(),
                        TentativeId = tentative.Id,
                        QuestionId = reponseDto.QuestionId,
                        OptionId = reponseDto.OptionId,
                        EstCorrecte = option.EstCorrecte
                    };

                    reponses.Add(reponse);

                    reponsesDto.Add(new ReponseQuestionDto
                    {
                        QuestionId = question.Id,
                        QuestionIntitule = question.Intitule,
                        OptionChoisieId = option.Id,
                        OptionChoisieTexte = option.Texte,
                        EstCorrecte = option.EstCorrecte,
                        Explication = question.Explication
                    });

                    if (option.EstCorrecte) correctAnswers++;
                }
            }

            tentative.Score = quiz.Questions.Count > 0 ? (float)correctAnswers / quiz.Questions.Count : 0;
            tentative.Reussi = tentative.Score >= 0.5f; // 50% pour réussir

            _context.TentativesQuiz.Add(tentative);
            _context.ReponsesUtilisateurQCM.AddRange(reponses);
            await _context.SaveChangesAsync();

            return new QuizResultDto
            {
                QuizId = quiz.Id,
                QuizTitre = quiz.Titre,
                Score = tentative.Score,
                Reussi = tentative.Reussi,
                TempsPasse = tentative.TempsPasse,
                Date = tentative.Date,
                Reponses = reponsesDto
            };
        }

        

        public async Task<IEnumerable<TentativeQuizDto>> GetUserQuizAttemptsAsync(Guid utilisateurId)
        {
            var tentatives = await _context.TentativesQuiz
                .Include(t => t.Quiz)
                .Where(t => t.UtilisateurId == utilisateurId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TentativeQuizDto>>(tentatives);
        }

        // --- Questions ---
        public async Task<IEnumerable<QCMQuestionDto>> GetQuestionsByQuizIdAsync(Guid quizId)
        {
            var questions = await _quizRepository.GetQuestionsByQuizIdAsync(quizId);
            return _mapper.Map<IEnumerable<QCMQuestionDto>>(questions);
        }

        public async Task<QCMQuestionDto?> GetQuestionByIdAsync(Guid id)
        {
            var question = await _quizRepository.GetQuestionByIdAsync(id);
            return question == null ? null : _mapper.Map<QCMQuestionDto>(question);
        }

        public async Task<QCMQuestionDto> CreateQuestionAsync(CreateQCMQuestionDto createDto)
        {
            var question = _mapper.Map<QCMQuestion>(createDto);
            var created = await _quizRepository.CreateQuestionAsync(question);
            return _mapper.Map<QCMQuestionDto>(created);
        }

        public async Task<QCMQuestionDto?> UpdateQuestionAsync(Guid id, UpdateQCMQuestionDto updateDto)
        {
            var question = await _quizRepository.GetQuestionByIdAsync(id);
            if (question == null) return null;
            _mapper.Map(updateDto, question);
            var updated = await _quizRepository.UpdateQuestionAsync(question);
            return _mapper.Map<QCMQuestionDto>(updated);
        }

        public async Task<bool> DeleteQuestionAsync(Guid id)
        {
            return await _quizRepository.DeleteQuestionAsync(id);
        }

        public async Task<QCMOptionDto?> GetOptionByIdAsync(Guid id)
        {
            var option = await _quizRepository.GetOptionByIdAsync(id);
            if (option == null) return null;
            return _mapper.Map<QCMOptionDto>(option);
        }

        // --- Options ---
        public async Task<IEnumerable<QCMOptionDto>> GetOptionsByQuestionIdAsync(Guid questionId)
        {
            var options = await _quizRepository.GetOptionsByQuestionIdAsync(questionId);
            return _mapper.Map<IEnumerable<QCMOptionDto>>(options);
        }

        public async Task<QCMOptionDto> CreateOptionAsync(CreateQCMOptionDto createDto)
        {
            var option = _mapper.Map<QCMOption>(createDto);
            var created = await _quizRepository.CreateOptionAsync(option);
            return _mapper.Map<QCMOptionDto>(created);
        }

        public async Task<QCMOptionDto?> UpdateOptionAsync(Guid id, UpdateQCMOptionDto updateDto)
        {
            var option = await _quizRepository.GetOptionByIdAsync(id);
            if (option == null) return null;
            _mapper.Map(updateDto, option);
            var updated = await _quizRepository.UpdateOptionAsync(option);
            return _mapper.Map<QCMOptionDto>(updated);
        }

        public async Task<bool> DeleteOptionAsync(Guid id)
        {
            return await _quizRepository.DeleteOptionAsync(id);
        }
    }
}

