using Microsoft.AspNetCore.Mvc;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace SprintQuiz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SprintController : ControllerBase
    {
        private readonly ISprintService _sprintService;

        public SprintController(ISprintService sprintService)
        {
            _sprintService = sprintService;
        }
        private Guid? GetUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return userIdClaim?.Value != null && Guid.TryParse(userIdClaim.Value, out var userId)
                ? userId
                : null;
        }

        /// <summary>
        /// Récupère tous les sprints (Public)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SprintDto>>> GetAll()
        {
            var sprints = await _sprintService.GetAllAsync();
            return Ok(sprints);
        }

        /// <summary>
        /// Récupère un sprint par ID (Public)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<SprintDto>> GetById(Guid id)
        {
            var utilisateurId = GetUserId();
            var dto = await _sprintService.GetByIdAsync(id, utilisateurId);

            if (dto == null)
                return NotFound($"Sprint avec l'ID {id} non trouvé.");

            return Ok(dto);
        }

        /// <summary>
        /// Récupère un sprint avec ses modules (Public)
        /// </summary>
        [HttpGet("{id}/modules")]
        [AllowAnonymous]
        public async Task<ActionResult<SprintDto>> GetWithModules(Guid id)
        {
            var utilisateurId = GetUserId();
            var dto = await _sprintService.GetWithModulesAsync(id, utilisateurId);

            if (dto == null)
                return NotFound($"Sprint avec l'ID {id} non trouvé.");

            return Ok(dto);
        }

        /// <summary>
        /// Crée un nouveau sprint (Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SprintDto>> Create([FromBody] CreateSprintDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _sprintService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur lors de la création : {ex.Message}");
            }
        }

        /// <summary>
        /// Met à jour un sprint existant (Admin)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SprintDto>> Update(Guid id, [FromBody] UpdateSprintDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _sprintService.UpdateAsync(id, updateDto);
                if (updated == null)
                    return NotFound($"Sprint avec l'ID {id} non trouvé.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur lors de la mise à jour : {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime un sprint (Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _sprintService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}

