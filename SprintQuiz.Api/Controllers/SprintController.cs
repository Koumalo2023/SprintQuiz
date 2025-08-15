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

        /// <summary>
        /// Récupère tous les sprints
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<IEnumerable<SprintDto>>> GetAllSprints()
        {
            var sprints = await _sprintService.GetAllSprintsAsync();
            return Ok(sprints);
        }

        /// <summary>
        /// Récupère un sprint par son ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<SprintDto>> GetSprintById(Guid id)
        {
            var sprint = await _sprintService.GetSprintByIdAsync(id);
            if (sprint == null)
                return NotFound($"Sprint avec l'ID {id} non trouvé");

            return Ok(sprint);
        }

        /// <summary>
        /// Récupère un sprint avec ses modules
        /// </summary>
        [HttpGet("{id}/modules")]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<SprintDto>> GetSprintWithModules(Guid id)
        {
            var sprint = await _sprintService.GetSprintWithModulesAsync(id);
            if (sprint == null)
                return NotFound($"Sprint avec l'ID {id} non trouvé");

            return Ok(sprint);
        }

        /// <summary>
        /// Crée un nouveau sprint
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent créer
        public async Task<ActionResult<SprintDto>> CreateSprint([FromBody] CreateSprintDto createSprintDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdSprint = await _sprintService.CreateSprintAsync(createSprintDto);
            return CreatedAtAction(nameof(GetSprintById), new { id = createdSprint.Id }, createdSprint);
        }

        /// <summary>
        /// Met à jour un sprint existant
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent modifier
        public async Task<ActionResult<SprintDto>> UpdateSprint(Guid id, [FromBody] UpdateSprintDto updateSprintDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedSprint = await _sprintService.UpdateSprintAsync(id, updateSprintDto);
            if (updatedSprint == null)
                return NotFound($"Sprint avec l'ID {id} non trouvé");

            return Ok(updatedSprint);
        }

        /// <summary>
        /// Supprime un sprint
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent supprimer
        public async Task<ActionResult> DeleteSprint(Guid id)
        {
            var deleted = await _sprintService.DeleteSprintAsync(id);
            if (!deleted)
                return NotFound($"Sprint avec l'ID {id} non trouvé");

            return NoContent();
        }
    }
}

