// Controllers/FormationController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Services;

namespace SprintQuiz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormationController : ControllerBase
    {
        private readonly IFormationService _formationService;

        public FormationController(IFormationService formationService)
        {
            _formationService = formationService;
        }

        private Guid? GetUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return userIdClaim?.Value != null && Guid.TryParse(userIdClaim.Value, out var userId)
                ? userId
                : null;
        }

        /// <summary>
        /// Récupère toutes les formations (Public)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<FormationDto>>> GetAll()
        {
            var formations = await _formationService.GetAllAsync();
            return Ok(formations);
        }

        /// <summary>
        /// Récupère une formation par ID (Public)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<FormationDto>> GetById(Guid id)
        {
            var utilisateurId = GetUserId();
            var dto = await _formationService.GetByIdAsync(id, utilisateurId);

            if (dto == null)
                return NotFound($"Formation avec l'ID {id} non trouvée.");

            return Ok(dto);
        }

        /// <summary>
        /// Crée une nouvelle formation (Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FormationDto>> Create([FromBody] CreateFormationDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _formationService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur lors de la création : {ex.Message}");
            }
        }

        /// <summary>
        /// Met à jour une formation (Admin)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FormationDto>> Update(Guid id, [FromBody] UpdateFormationDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _formationService.UpdateAsync(id, updateDto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Formation avec l'ID {id} non trouvée.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur lors de la mise à jour : {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime une formation (Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _formationService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}