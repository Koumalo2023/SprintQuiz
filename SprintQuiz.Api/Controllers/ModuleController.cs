using Microsoft.AspNetCore.Mvc;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace SprintQuiz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModuleController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }
        private Guid? GetUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return userIdClaim?.Value != null && Guid.TryParse(userIdClaim.Value, out var userId)
                ? userId
                : null;
        }

        /// <summary>
        /// Récupère tous les modules (Public)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAll()
        {
            var modules = await _moduleService.GetAllAsync();
            return Ok(modules);
        }

        /// <summary>
        /// Récupère un module par ID (Public)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ModuleDto>> GetById(Guid id)
        {
            var utilisateurId = GetUserId();
            var dto = await _moduleService.GetByIdAsync(id, utilisateurId);

            if (dto == null)
                return NotFound($"Module avec l'ID {id} non trouvé.");

            return Ok(dto);
        }

        /// <summary>
        /// Récupère un module avec ses cours (Public)
        /// </summary>
        [HttpGet("{id}/cours")]
        [AllowAnonymous]
        public async Task<ActionResult<ModuleDto>> GetWithCours(Guid id)
        {
            var utilisateurId = GetUserId();
            var dto = await _moduleService.GetWithCoursAsync(id, utilisateurId);

            if (dto == null)
                return NotFound($"Module avec l'ID {id} non trouvé.");

            return Ok(dto);
        }

        /// <summary>
        /// Récupère les modules d’un sprint (Public)
        /// </summary>
        [HttpGet("sprint/{sprintId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetBySprintId(Guid sprintId)
        {
            var utilisateurId = GetUserId();
            var dtos = await _moduleService.GetBySprintIdAsync(sprintId, utilisateurId);
            return Ok(dtos);
        }

        /// <summary>
        /// Crée un nouveau module (Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ModuleDto>> Create([FromBody] CreateModuleDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _moduleService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur lors de la création : {ex.Message}");
            }
        }

        /// <summary>
        /// Met à jour un module (Admin)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ModuleDto>> Update(Guid id, [FromBody] UpdateModuleDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _moduleService.UpdateAsync(id, updateDto);
                if (updated == null)
                    return NotFound($"Module avec l'ID {id} non trouvé.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur lors de la mise à jour : {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime un module (Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _moduleService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}

