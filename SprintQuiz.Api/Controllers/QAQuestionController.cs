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
    public class QAQuestionController : ControllerBase
    {
        private readonly IQAQuestionService _qaQuestionService;

        public QAQuestionController(IQAQuestionService qaQuestionService)
        {
            _qaQuestionService = qaQuestionService;
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
        /// Récupère toutes les questions-réponses
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<IEnumerable<QAQuestionDto>>> GetAllQAQuestions()
        {
            var questions = await _qaQuestionService.GetAllQAQuestionsAsync();
            return Ok(questions);
        }

        /// <summary>
        /// Récupère une question-réponse par son ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<QAQuestionDto>> GetQAQuestionById(Guid id)
        {
            var question = await _qaQuestionService.GetQAQuestionByIdAsync(id);
            if (question == null)
                return NotFound($"Question-réponse avec l'ID {id} non trouvée");

            return Ok(question);
        }

        /// <summary>
        /// Récupère les questions-réponses par niveau (cours, module, sprint)
        /// </summary>
        [HttpGet("niveau/{niveau}/{niveauId}")]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<IEnumerable<QAQuestionDto>>> GetQAQuestionsByNiveau(NiveauEnum niveau, Guid niveauId)
        {
            var questions = await _qaQuestionService.GetQAQuestionsByNiveauAsync(niveau, niveauId);
            return Ok(questions);
        }

        /// <summary>
        /// Récupère les questions-réponses pour révision d'un utilisateur
        /// </summary>
        [HttpGet("revision/{utilisateurId}/niveau/{niveau}/{niveauId}")]
        [Authorize] // Nécessite une authentification
        public async Task<ActionResult<IEnumerable<QAQuestionDto>>> GetQAQuestionsForRevision(
            Guid utilisateurId, NiveauEnum niveau, Guid niveauId)
        {
            // Vérifier que l'utilisateur demande ses propres questions ou qu'il est admin
            var currentUserId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (currentUserId != utilisateurId && userRole != "Admin")
            {
                return Forbid("Vous ne pouvez consulter que vos propres questions de révision.");
            }

            var questions = await _qaQuestionService.GetQAQuestionsForRevisionAsync(utilisateurId, niveau, niveauId);
            return Ok(questions);
        }

        /// <summary>
        /// Récupère les questions-réponses pour révision de l'utilisateur connecté
        /// </summary>
        [HttpGet("ma-revision/niveau/{niveau}/{niveauId}")]
        [Authorize] // Nécessite une authentification
        public async Task<ActionResult<IEnumerable<QAQuestionDto>>> GetMyQAQuestionsForRevision(
            NiveauEnum niveau, Guid niveauId)
        {
            try
            {
                var utilisateurId = GetUserId();
                var questions = await _qaQuestionService.GetQAQuestionsForRevisionAsync(utilisateurId, niveau, niveauId);
                return Ok(questions);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Crée une nouvelle question-réponse
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent créer des questions
        public async Task<ActionResult<QAQuestionDto>> CreateQAQuestion([FromBody] CreateQAQuestionDto createQAQuestionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdQuestion = await _qaQuestionService.CreateQAQuestionAsync(createQAQuestionDto);
            return CreatedAtAction(nameof(GetQAQuestionById), new { id = createdQuestion.Id }, createdQuestion);
        }

        /// <summary>
        /// Met à jour une question-réponse existante
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent modifier des questions
        public async Task<ActionResult<QAQuestionDto>> UpdateQAQuestion(Guid id, [FromBody] UpdateQAQuestionDto updateQAQuestionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedQuestion = await _qaQuestionService.UpdateQAQuestionAsync(id, updateQAQuestionDto);
            if (updatedQuestion == null)
                return NotFound($"Question-réponse avec l'ID {id} non trouvée");

            return Ok(updatedQuestion);
        }

        /// <summary>
        /// Supprime une question-réponse
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent supprimer des questions
        public async Task<ActionResult> DeleteQAQuestion(Guid id)
        {
            var deleted = await _qaQuestionService.DeleteQAQuestionAsync(id);
            if (!deleted)
                return NotFound($"Question-réponse avec l'ID {id} non trouvée");

            return NoContent();
        }

        /// <summary>
        /// Enregistre une consultation de question-réponse (flashcard)
        /// </summary>
        [HttpPost("consult")]
        [Authorize] // Nécessite une authentification pour consulter
        public async Task<ActionResult<ConsultationQADto>> ConsultQAQuestion([FromBody] CreateConsultationQADto consultationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var utilisateurId = GetUserId();
                var consultation = await _qaQuestionService.ConsultQAQuestionAsync(utilisateurId, consultationDto);
                return Ok(consultation);
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
        /// Récupère les consultations d'un utilisateur
        /// </summary>
        [HttpGet("utilisateur/{utilisateurId}/consultations")]
        [Authorize] // Nécessite une authentification
        public async Task<ActionResult<IEnumerable<ConsultationQADto>>> GetUserConsultations(Guid utilisateurId)
        {
            // Vérifier que l'utilisateur demande ses propres consultations ou qu'il est admin
            var currentUserId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (currentUserId != utilisateurId && userRole != "Admin")
            {
                return Forbid("Vous ne pouvez consulter que vos propres consultations.");
            }

            var consultations = await _qaQuestionService.GetUserConsultationsAsync(utilisateurId);
            return Ok(consultations);
        }

        /// <summary>
        /// Récupère les consultations de l'utilisateur connecté
        /// </summary>
        [HttpGet("mes-consultations")]
        [Authorize] // Nécessite une authentification
        public async Task<ActionResult<IEnumerable<ConsultationQADto>>> GetMyConsultations()
        {
            try
            {
                var utilisateurId = GetUserId();
                var consultations = await _qaQuestionService.GetUserConsultationsAsync(utilisateurId);
                return Ok(consultations);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}

