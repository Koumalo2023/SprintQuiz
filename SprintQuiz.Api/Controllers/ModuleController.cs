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

        /// <summary>
        /// Récupère tous les modules
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAllModules()
        {
            var modules = await _moduleService.GetAllModulesAsync();
            return Ok(modules);
        }

        /// <summary>
        /// Récupère un module par son ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<ModuleDto>> GetModuleById(Guid id)
        {
            var module = await _moduleService.GetModuleByIdAsync(id);
            if (module == null)
                return NotFound($"Module avec l'ID {id} non trouvé");

            return Ok(module);
        }

        /// <summary>
        /// Récupère un module avec ses cours
        /// </summary>
        [HttpGet("{id}/cours")]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<ModuleDto>> GetModuleWithCours(Guid id)
        {
            var module = await _moduleService.GetModuleWithCoursAsync(id);
            if (module == null)
                return NotFound($"Module avec l'ID {id} non trouvé");

            return Ok(module);
        }

        /// <summary>
        /// Récupère les modules d'un sprint
        /// </summary>
        [HttpGet("sprint/{sprintId}")]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModulesBySprintId(Guid sprintId)
        {
            var modules = await _moduleService.GetModulesBySprintIdAsync(sprintId);
            return Ok(modules);
        }

        /// <summary>
        /// Crée un nouveau module
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent créer
        public async Task<ActionResult<ModuleDto>> CreateModule([FromBody] CreateModuleDto createModuleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdModule = await _moduleService.CreateModuleAsync(createModuleDto);
            return CreatedAtAction(nameof(GetModuleById), new { id = createdModule.Id }, createdModule);
        }

        /// <summary>
        /// Met à jour un module existant
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent modifier
        public async Task<ActionResult<ModuleDto>> UpdateModule(Guid id, [FromBody] UpdateModuleDto updateModuleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedModule = await _moduleService.UpdateModuleAsync(id, updateModuleDto);
            if (updatedModule == null)
                return NotFound($"Module avec l'ID {id} non trouvé");

            return Ok(updatedModule);
        }

        /// <summary>
        /// Supprime un module
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent supprimer
        public async Task<ActionResult> DeleteModule(Guid id)
        {
            var deleted = await _moduleService.DeleteModuleAsync(id);
            if (!deleted)
                return NotFound($"Module avec l'ID {id} non trouvé");

            return NoContent();
        }
    }
}

