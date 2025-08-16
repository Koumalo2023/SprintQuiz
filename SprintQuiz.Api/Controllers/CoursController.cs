using Microsoft.AspNetCore.Mvc;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace SprintQuiz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursController : ControllerBase
    {
        private readonly ICoursService _coursService;

        public CoursController(ICoursService coursService)
        {
            _coursService = coursService;
        }
        private Guid? GetUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return userIdClaim?.Value != null && Guid.TryParse(userIdClaim.Value, out var userId)
                ? userId
                : null;
        }

        /// <summary>
        /// Récupère tous les cours (Public)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CoursDto>>> GetAll()
        {
            var cours = await _coursService.GetAllAsync();
            return Ok(cours);
        }

        /// <summary>
        /// Récupère un cours par ID (Public)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CoursDto>> GetById(Guid id)
        {
            var utilisateurId = GetUserId();
            var dto = await _coursService.GetByIdAsync(id, utilisateurId);

            if (dto == null)
                return NotFound($"Cours avec l'ID {id} non trouvé.");

            return Ok(dto);
        }

        /// <summary>
        /// Récupère les cours d’un module (Public)
        /// </summary>
        [HttpGet("module/{moduleId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CoursDto>>> GetByModuleId(Guid moduleId)
        {
            var utilisateurId = GetUserId();
            var dtos = await _coursService.GetByModuleIdAsync(moduleId, utilisateurId);
            return Ok(dtos);
        }

        /// <summary>
        /// Crée un nouveau cours (Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CoursDto>> Create([FromBody] CreateCoursDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _coursService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur lors de la création : {ex.Message}");
            }
        }

        /// <summary>
        /// Met à jour un cours (Admin)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CoursDto>> Update(Guid id, [FromBody] UpdateCoursDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _coursService.UpdateAsync(id, updateDto);
                if (updated == null)
                    return NotFound($"Cours avec l'ID {id} non trouvé.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur lors de la mise à jour : {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime un cours (Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _coursService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}

