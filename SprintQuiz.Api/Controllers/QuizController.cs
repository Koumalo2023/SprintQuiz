using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Services; 
using System.Security.Claims;

namespace SprintQuiz.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        private Guid? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value != null && Guid.TryParse(userIdClaim.Value, out var userId)
                ? userId
                : null;
        }

        /// <summary>
        /// Récupère tous les quiz (Public)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetAllQuizzes()
        {
            var quizzes = await _quizService.GetAllQuizzesAsync();
            return Ok(quizzes);
        }

        /// <summary>
        /// Récupère un quiz par ID (Public)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<QuizDto>> GetQuizById(Guid id)
        {
            var quiz = await _quizService.GetQuizByIdAsync(id);
            if (quiz == null)
                return NotFound($"Quiz avec l'ID {id} non trouvé.");
            return Ok(quiz);
        }

        /// <summary>
        /// Récupère un quiz avec ses questions (Authentifié)
        /// Optionnel : ?revision=true pour le mode révision
        /// </summary>
        [HttpGet("{id}/questions")]
        [Authorize]
        public async Task<ActionResult<QuizDto>> GetQuizWithQuestions(Guid id, [FromQuery] bool revision = false)
        {
            var utilisateurId = GetUserId();
            var dto = await _quizService.GetQuizWithQuestionsAsync(id, utilisateurId);

            if (dto == null)
                return NotFound($"Quiz avec l'ID {id} non trouvé.");

            // Mode révision : filtrer les questions non maîtrisées
            if (revision && utilisateurId.HasValue)
            {
                var tentatives = await _quizService.GetUserQuizAttemptsAsync(utilisateurId.Value);
                var questionsRatees = tentatives
                    .SelectMany(t => t.Reponses)
                    .Where(r => !r.EstCorrecte)
                    .Select(r => r.QuestionId)
                    .Distinct();

                dto.Questions = dto.Questions.Where(q => questionsRatees.Contains(q.Id)).ToList();
            }

            // Mélanger les questions si activé
            if (dto.MelangerQuestions)
            {
                var random = new Random();
                dto.Questions = dto.Questions.OrderBy(q => random.Next()).ToList();
            }

            return Ok(dto);
        }

        /// <summary>
        /// Récupère les quiz par niveau (cours, module, sprint) (Public)
        /// </summary>
        [HttpGet("niveau/{niveau}/{niveauId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetQuizzesByNiveau(NiveauEnum niveau, Guid niveauId)
        {
            var quizzes = await _quizService.GetQuizzesByNiveauAsync(niveau, niveauId);
            return Ok(quizzes);
        }

        /// <summary>
        /// Crée un nouveau quiz (Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QuizDto>> CreateQuiz([FromBody] CreateQuizDto createQuizDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdQuiz = await _quizService.CreateQuizAsync(createQuizDto);
                return CreatedAtAction(nameof(GetQuizWithQuestions), new { id = createdQuiz.Id }, createdQuiz);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Met à jour un quiz (Admin)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QuizDto>> UpdateQuiz(Guid id, [FromBody] UpdateQuizDto updateQuizDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedQuiz = await _quizService.UpdateQuizAsync(id, updateQuizDto);
                if (updatedQuiz == null)
                    return NotFound($"Quiz avec l'ID {id} non trouvé.");
                return Ok(updatedQuiz);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Une erreur s'est produite lors de la mise à jour du quiz.");
            }
        }

        /// <summary>
        /// Supprime un quiz (Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteQuiz(Guid id)
        {
            var deleted = await _quizService.DeleteQuizAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Soumet une tentative de quiz (Authentifié)
        /// </summary>
        [HttpPost("submit")]
        [Authorize]
        public async Task<ActionResult<QuizResultDto>> SubmitQuiz([FromBody] CreateTentativeQuizDto tentativeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var utilisateurId = GetUserId();
                if (!utilisateurId.HasValue)
                    return Unauthorized("Utilisateur non authentifié.");

                var result = await _quizService.SubmitQuizAsync(utilisateurId.Value, tentativeDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Récupère les tentatives de quiz de l'utilisateur connecté (Authentifié)
        /// </summary>
        [HttpGet("mes-tentatives")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TentativeQuizDto>>> GetMyQuizAttempts()
        {
            try
            {
                var utilisateurId = GetUserId();
                if (!utilisateurId.HasValue)
                    return Unauthorized("Utilisateur non authentifié.");

                var tentatives = await _quizService.GetUserQuizAttemptsAsync(utilisateurId.Value);
                return Ok(tentatives);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erreur lors de la récupération de vos tentatives.");
            }
        }

        /// <summary>
        /// Récupère les tentatives d'un utilisateur (Admin ou soi-même)
        /// </summary>
        [HttpGet("utilisateur/{utilisateurId}/tentatives")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TentativeQuizDto>>> GetUserQuizAttempts(Guid utilisateurId)
        {
            try
            {
                var currentUserId = GetUserId();
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (!currentUserId.HasValue)
                    return Unauthorized("Utilisateur non authentifié.");

                if (currentUserId.Value != utilisateurId && userRole != "Admin")
                    return Forbid("Vous ne pouvez consulter que vos propres tentatives.");

                var tentatives = await _quizService.GetUserQuizAttemptsAsync(utilisateurId);
                return Ok(tentatives);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erreur lors de la récupération des tentatives.");
            }
        }

        // -----------------------------
        // Gestion des Questions & Options (Admin uniquement)
        // -----------------------------

        /// <summary>
        /// Récupère les questions d'un quiz (Authentifié)
        /// </summary>
        [HttpGet("questions/quiz/{quizId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<QCMQuestionDto>>> GetQuestionsByQuiz(Guid quizId)
        {
            var questions = await _quizService.GetQuestionsByQuizIdAsync(quizId);
            return Ok(questions);
        }

        /// <summary>
        /// Récupère une question par ID (Authentifié)
        /// </summary>
        [HttpGet("question/{id}")]
        [Authorize]
        public async Task<ActionResult<QCMQuestionDto>> GetQuestionById(Guid id)
        {
            var question = await _quizService.GetQuestionByIdAsync(id);
            if (question == null)
                return NotFound($"Question avec l'ID {id} non trouvée.");
            return Ok(question);
        }

        /// <summary>
        /// Met à jour une question (Admin)
        /// </summary>
        [HttpPut("question/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QCMQuestionDto>> UpdateQuestion(Guid id, [FromBody] UpdateQCMQuestionDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _quizService.UpdateQuestionAsync(id, updateDto);
            if (updated == null)
                return NotFound($"Question avec l'ID {id} non trouvée.");
            return Ok(updated);
        }

        /// <summary>
        /// Supprime une question (Admin)
        /// </summary>
        [HttpDelete("question/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteQuestion(Guid id)
        {
            var deleted = await _quizService.DeleteQuestionAsync(id);
            if (!deleted)
                return NotFound($"Question avec l'ID {id} non trouvée.");
            return NoContent();
        }

        /// <summary>
        /// Récupère les options d'une question (Authentifié)
        /// </summary>
        [HttpGet("question/{questionId}/options")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<QCMOptionDto>>> GetOptionsByQuestion(Guid questionId)
        {
            var options = await _quizService.GetOptionsByQuestionIdAsync(questionId);
            return Ok(options);
        }

        /// <summary>
        /// Crée une option de réponse (Admin)
        /// </summary>
        [HttpPost("option")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QCMOptionDto>> CreateOption([FromBody] CreateQCMOptionDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _quizService.CreateOptionAsync(createDto);
            return CreatedAtAction(nameof(GetOptionById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Met à jour une option (Admin)
        /// </summary>
        [HttpPut("option/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QCMOptionDto>> UpdateOption(Guid id, [FromBody] UpdateQCMOptionDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _quizService.UpdateOptionAsync(id, updateDto);
            if (updated == null)
                return NotFound($"Option avec l'ID {id} non trouvée.");
            return Ok(updated);
        }

        /// <summary>
        /// Supprime une option (Admin)
        /// </summary>
        [HttpDelete("option/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteOption(Guid id)
        {
            var deleted = await _quizService.DeleteOptionAsync(id);
            if (!deleted)
                return NotFound($"Option avec l'ID {id} non trouvée.");
            return NoContent();
        }

        /// <summary>
        /// Récupère une option par ID (Authentifié)
        /// </summary>
        [HttpGet("option/{id}")]
        [Authorize]
        public async Task<ActionResult<QCMOptionDto>> GetOptionById(Guid id)
        {
            var option = await _quizService.GetOptionByIdAsync(id);
            if (option == null)
                return NotFound($"Option avec l'ID {id} non trouvée.");
            return Ok(option);
        }
    }
}