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

        /// <summary>
        /// Récupère tous les cours
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<IEnumerable<CoursDto>>> GetAllCours()
        {
            var cours = await _coursService.GetAllCoursAsync();
            return Ok(cours);
        }

        /// <summary>
        /// Récupère un cours par son ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<CoursDto>> GetCoursById(Guid id)
        {
            var cours = await _coursService.GetCoursByIdAsync(id);
            if (cours == null)
                return NotFound($"Cours avec l'ID {id} non trouvé");

            return Ok(cours);
        }

        /// <summary>
        /// Récupère les cours d'un module
        /// </summary>
        [HttpGet("module/{moduleId}")]
        [AllowAnonymous] // Accessible par tous
        public async Task<ActionResult<IEnumerable<CoursDto>>> GetCoursByModuleId(Guid moduleId)
        {
            var cours = await _coursService.GetCoursByModuleIdAsync(moduleId);
            return Ok(cours);
        }

        /// <summary>
        /// Crée un nouveau cours
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent créer
        public async Task<ActionResult<CoursDto>> CreateCours([FromBody] CreateCoursDto createCoursDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCours = await _coursService.CreateCoursAsync(createCoursDto);
            return CreatedAtAction(nameof(GetCoursById), new { id = createdCours.Id }, createdCours);
        }

        /// <summary>
        /// Met à jour un cours existant
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent modifier
        public async Task<ActionResult<CoursDto>> UpdateCours(Guid id, [FromBody] UpdateCoursDto updateCoursDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedCours = await _coursService.UpdateCoursAsync(id, updateCoursDto);
            if (updatedCours == null)
                return NotFound($"Cours avec l'ID {id} non trouvé");

            return Ok(updatedCours);
        }

        /// <summary>
        /// Supprime un cours
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent supprimer
        public async Task<ActionResult> DeleteCours(Guid id)
        {
            var deleted = await _coursService.DeleteCoursAsync(id);
            if (!deleted)
                return NotFound($"Cours avec l'ID {id} non trouvé");

            return NoContent();
        }
    }
}

