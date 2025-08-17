using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Services;
using System.Security.Claims;

namespace SprintQuiz.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciceController : ControllerBase
    {
        private readonly IExerciceService _exerciceService;

        public ExerciceController(IExerciceService exerciceService)
        {
            _exerciceService = exerciceService;
        }

        private Guid? GetUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return userIdClaim?.Value != null && Guid.TryParse(userIdClaim.Value, out var userId)
                ? userId
                : null;
        }

        // --- CRUD Exercices ---
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExerciceDto>>> GetAll()
        {
            var exercices = await _exerciceService.GetAllExercicesAsync();
            return Ok(exercices);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ExerciceDto>> GetExerciceById(Guid id)
        {
            var utilisateurId = GetUserId();
            var dto = await _exerciceService.GetExerciceByIdAsync(id, utilisateurId);

            if (dto == null)
                return NotFound($"Exercice avec l'ID {id} non trouvé.");

            return Ok(dto);
        }

        [HttpGet("niveau/{niveau}/{niveauId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExerciceDto>>> GetByNiveau(NiveauEnum niveau, Guid niveauId)
        {
            var exercices = await _exerciceService.GetExercicesByNiveauAsync(niveau, niveauId);
            return Ok(exercices);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ExerciceDto>> Create([FromBody] CreateExerciceDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _exerciceService.CreateExerciceAsync(createDto);
            return CreatedAtAction(nameof(GetExerciceById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ExerciceDto>> Update(Guid id, [FromBody] UpdateExerciceDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _exerciceService.UpdateExerciceAsync(id, updateDto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _exerciceService.DeleteExerciceAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // --- Révision ---
        [HttpGet("ma-revision/niveau/{niveau}/{niveauId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ExerciceDto>>> GetMyExercicesForRevision(NiveauEnum niveau, Guid niveauId)
        {
            try
            {
                var utilisateurId = GetUserId();
                var exercices = await _exerciceService.GetExercicesForRevisionAsync(utilisateurId.Value, niveau, niveauId);
                return Ok(exercices);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("revision/{userId}/niveau/{niveau}/{niveauId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ExerciceDto>>> GetExercicesForRevision(Guid userId, NiveauEnum niveau, Guid niveauId)
        {
            var currentUserId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentUserId != userId && userRole != "Admin")
                return Forbid("Accès refusé.");

            var exercices = await _exerciceService.GetExercicesForRevisionAsync(userId, niveau, niveauId);
            return Ok(exercices);
        }

        // --- Consultation ---
        [HttpPost("consult")]
        [Authorize]
        public async Task<ActionResult<ConsultationExerciceDto>> Consult([FromBody] CreateConsultationExerciceDto consultationDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var userId = GetUserId();
                var result = await _exerciceService.ConsultExerciceAsync(userId.Value, consultationDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("mes-consultations")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ConsultationExerciceDto>>> GetMyConsultations()
        {
            try
            {
                var userId = GetUserId();
                var consultations = await _exerciceService.GetUserConsultationsAsync(userId.Value);
                return Ok(consultations);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("utilisateur/{userId}/consultations")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ConsultationExerciceDto>>> GetUserConsultations(Guid userId)
        {
            var currentUserId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentUserId != userId && userRole != "Admin")
                return Forbid("Accès refusé.");

            var consultations = await _exerciceService.GetUserConsultationsAsync(userId);
            return Ok(consultations);
        }

        // --- Indices ---
        [HttpGet("{exerciceId}/indices")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<IndiceDto>>> GetIndices(Guid exerciceId)
        {
            var indices = await _exerciceService.GetIndicesByExerciceIdAsync(exerciceId);
            return Ok(indices);
        }

        [HttpGet("indice/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IndiceDto>> GetIndiceById(Guid id)
        {
            var indice = await _exerciceService.GetIndiceByIdAsync(id);
            if (indice == null) return NotFound();
            return Ok(indice);
        }

        [HttpPost("indice")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IndiceDto>> CreateIndice([FromBody] CreateIndiceDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _exerciceService.CreateIndiceAsync(createDto);
            return CreatedAtAction(nameof(GetIndiceById), new { id = created.Id }, created);
        }

        [HttpPut("indice/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IndiceDto>> UpdateIndice(Guid id, [FromBody] UpdateIndiceDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _exerciceService.UpdateIndiceAsync(id, updateDto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("indice/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteIndice(Guid id)
        {
            var deleted = await _exerciceService.DeleteIndiceAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // --- Étapes de Résolution ---
        [HttpGet("{exerciceId}/etapes")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EtapeResolutionDto>>> GetEtapes(Guid exerciceId)
        {
            var etapes = await _exerciceService.GetEtapesByExerciceIdAsync(exerciceId);
            return Ok(etapes);
        }

        [HttpGet("etape/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EtapeResolutionDto>> GetEtapeById(Guid id)
        {
            var etape = await _exerciceService.GetEtapeByIdAsync(id);
            if (etape == null) return NotFound();
            return Ok(etape);
        }

        [HttpPost("etape")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EtapeResolutionDto>> CreateEtape([FromBody] CreateEtapeResolutionDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _exerciceService.CreateEtapeAsync(createDto);
            return CreatedAtAction(nameof(GetEtapeById), new { id = created.Id }, created);
        }

        [HttpPut("etape/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EtapeResolutionDto>> UpdateEtape(Guid id, [FromBody] UpdateEtapeResolutionDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _exerciceService.UpdateEtapeAsync(id, updateDto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("etape/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteEtape(Guid id)
        {
            var deleted = await _exerciceService.DeleteEtapeAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
