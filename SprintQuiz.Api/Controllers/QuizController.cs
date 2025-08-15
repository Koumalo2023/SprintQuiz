using Microsoft.AspNetCore.Mvc;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SprintQuiz.Api.Controllers
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

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("ID utilisateur non trouvé ou invalide dans le token.");
            }
            return userId;
        }

        /// <summary>
        /// Récupère tous les quiz
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetAllQuizzes()
        {
            var quizzes = await _quizService.GetAllQuizzesAsync();
            return Ok(quizzes);
        }

        /// <summary>
        /// Récupère un quiz par son ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<QuizDto>> GetQuizById(Guid id)
        {
            var quiz = await _quizService.GetQuizByIdAsync(id);
            if (quiz == null)
                return NotFound($"Quiz avec l'ID {id} non trouvé");

            return Ok(quiz);
        }

        /// <summary>
        /// Récupère un quiz avec ses questions
        /// </summary>
        [HttpGet("{id}/questions")]
        [Authorize] // Nécessite une authentification pour voir les questions
        public async Task<ActionResult<QuizDto>> GetQuizWithQuestions(Guid id)
        {
            var quiz = await _quizService.GetQuizWithQuestionsAsync(id);
            if (quiz == null)
                return NotFound($"Quiz avec l'ID {id} non trouvé");

            return Ok(quiz);
        }

        
        /// <summary>
        /// Récupère les quiz par niveau (cours, module, sprint)
        /// </summary>
        [HttpGet("niveau/{niveau}/{niveauId}")]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetQuizzesByNiveau(NiveauEnum niveau, Guid niveauId)
        {
            var quizzes = await _quizService.GetQuizzesByNiveauAsync(niveau, niveauId);
            return Ok(quizzes);
        }

        /// <summary>
        /// Crée un nouveau quiz avec ses questions 
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
        /// Met à jour un quiz existant
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
                    return NotFound($"Quiz avec l'ID {id} non trouvé");
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
        /// Supprime un quiz
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent supprimer des quiz
        public async Task<ActionResult> DeleteQuiz(Guid id)
        {
            var deleted = await _quizService.DeleteQuizAsync(id);
            if (!deleted)
                return NotFound($"Quiz avec l'ID {id} non trouvé");

            return NoContent();
        }

        /// <summary>
        /// Récupère toutes les questions d'un quiz
        /// </summary>
        [HttpGet("quiz/{quizId}/questions")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<QCMQuestionDto>>> GetQuestionsByQuiz(Guid quizId)
        {
            var questions = await _quizService.GetQuestionsByQuizIdAsync(quizId);
            return Ok(questions);
        }

        /// <summary>
        /// Récupère une question par ID
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
        /// Met à jour une question
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
        /// Supprime une question
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

        // --- Options ---

        /// <summary>
        /// Récupère les options d'une question
        /// </summary>
        [HttpGet("question/{questionId}/options")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<QCMOptionDto>>> GetOptionsByQuestion(Guid questionId)
        {
            var options = await _quizService.GetOptionsByQuestionIdAsync(questionId);
            return Ok(options);
        }

        /// <summary>
        /// Crée une option de réponse
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
        /// Met à jour une option
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
        /// Supprime une option
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

        // Ajouter une méthode utilitaire si besoin
        [HttpGet("option/{id}")]
        [Authorize]
        public async Task<ActionResult<QCMOptionDto>> GetOptionById(Guid id)
        {
            var option = await _quizService.GetOptionByIdAsync(id);
            if (option == null)
                return NotFound($"Option avec l'ID {id} non trouvée.");
            return Ok(option);
        }

        /// <summary>
        /// Soumet une tentative de quiz
        /// </summary>
        [HttpPost("submit")]
        [Authorize] // Nécessite une authentification pour soumettre un quiz
        public async Task<ActionResult<QuizResultDto>> SubmitQuiz([FromBody] CreateTentativeQuizDto tentativeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var utilisateurId = GetUserId();
                var result = await _quizService.SubmitQuizAsync(utilisateurId, tentativeDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Récupère les tentatives de quiz d'un utilisateur
        /// </summary>
        [HttpGet("utilisateur/{utilisateurId}/tentatives")]
        [Authorize] // Nécessite une authentification
        public async Task<ActionResult<IEnumerable<TentativeQuizDto>>> GetUserQuizAttempts(Guid utilisateurId)
        {
            // Vérifier que l'utilisateur demande ses propres tentatives ou qu'il est admin
            var currentUserId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserId != utilisateurId && userRole != "Admin")
            {
                return Forbid("Vous ne pouvez consulter que vos propres tentatives.");
            }

            var tentatives = await _quizService.GetUserQuizAttemptsAsync(utilisateurId);
            return Ok(tentatives);
        }

        /// <summary>
        /// Récupère les tentatives de quiz de l'utilisateur connecté
        /// </summary>
        [HttpGet("mes-tentatives")]
        [Authorize] // Nécessite une authentification
        public async Task<ActionResult<IEnumerable<TentativeQuizDto>>> GetMyQuizAttempts()
        {
            try
            {
                var utilisateurId = GetUserId();
                var tentatives = await _quizService.GetUserQuizAttemptsAsync(utilisateurId);
                return Ok(tentatives);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}

