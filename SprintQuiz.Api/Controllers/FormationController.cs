using Microsoft.AspNetCore.Mvc;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SprintQuiz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormationController : ControllerBase
    {
        private readonly IUtilisateurService _utilisateurService;

        public FormationController(IUtilisateurService utilisateurService)
        {
            _utilisateurService = utilisateurService;
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
        /// Récupère tous les utilisateurs
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent voir tous les utilisateurs
        public async Task<ActionResult<IEnumerable<UtilisateurDto>>> GetAllUtilisateurs()
        {
            var utilisateurs = await _utilisateurService.GetAllUtilisateursAsync();
            return Ok(utilisateurs);
        }

        /// <summary>
        /// Récupère un utilisateur par son ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize] // Nécessite une authentification
        public async Task<ActionResult<UtilisateurDto>> GetUtilisateurById(Guid id)
        {
            // Vérifier que l'utilisateur demande ses propres informations ou qu'il est admin
            var currentUserId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (currentUserId != id && userRole != "Admin")
            {
                return Forbid("Vous ne pouvez consulter que vos propres informations.");
            }

            var utilisateur = await _utilisateurService.GetUtilisateurByIdAsync(id);
            if (utilisateur == null)
                return NotFound($"Utilisateur avec l'ID {id} non trouvé");

            return Ok(utilisateur);
        }

        /// <summary>
        /// Crée un nouvel utilisateur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]  //Seuls les administrateurs peuvent créer des utilisateurs
        public async Task<ActionResult<UtilisateurDto>> CreateUtilisateur([FromBody] CreateUtilisateurDto createUtilisateurDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdUtilisateur = await _utilisateurService.CreateUtilisateurAsync(createUtilisateurDto);
                return CreatedAtAction(nameof(GetUtilisateurById), new { id = createdUtilisateur.Id }, createdUtilisateur);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Met à jour un utilisateur existant
        /// </summary>
        [HttpPut("{id}")]
        [Authorize] // Nécessite une authentification
        public async Task<ActionResult<UtilisateurDto>> UpdateUtilisateur(Guid id, [FromBody] UpdateUtilisateurDto updateUtilisateurDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Vérifier que l'utilisateur modifie ses propres informations ou qu'il est admin
            var currentUserId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (currentUserId != id && userRole != "Admin")
            {
                return Forbid("Vous ne pouvez modifier que vos propres informations.");
            }

            // Les étudiants ne peuvent pas changer leur rôle
            if (userRole != "Admin" && updateUtilisateurDto.Role.HasValue)
            {
                return Forbid("Vous ne pouvez pas modifier votre rôle.");
            }

            try
            {
                var updatedUtilisateur = await _utilisateurService.UpdateUtilisateurAsync(id, updateUtilisateurDto);
                if (updatedUtilisateur == null)
                    return NotFound($"Utilisateur avec l'ID {id} non trouvé");

                return Ok(updatedUtilisateur);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Supprime un utilisateur
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Seuls les administrateurs peuvent supprimer des utilisateurs
        public async Task<ActionResult> DeleteUtilisateur(Guid id)
        {
            var deleted = await _utilisateurService.DeleteUtilisateurAsync(id);
            if (!deleted)
                return NotFound($"Utilisateur avec l'ID {id} non trouvé");

            return NoContent();
        }

        /// <summary>
        /// Authentifie un utilisateur
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous] // Accessible sans authentification
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authResponse = await _utilisateurService.LoginAsync(loginDto);
            if (authResponse == null)
                return Unauthorized("Email ou mot de passe incorrect");

            return Ok(authResponse);
        }

        /// <summary>
        /// Récupère les statistiques d'un utilisateur
        /// </summary>
        [HttpGet("{id}/statistiques")]
        [Authorize] // Nécessite une authentification
        public async Task<ActionResult<StatistiquesGlobalesDto>> GetUserStatistics(Guid id)
        {
            // Vérifier que l'utilisateur demande ses propres statistiques ou qu'il est admin
            var currentUserId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (currentUserId != id && userRole != "Admin")
            {
                return Forbid("Vous ne pouvez consulter que vos propres statistiques.");
            }

            var stats = await _utilisateurService.GetUserStatisticsAsync(id);
            if (stats == null)
                return NotFound($"Statistiques pour l'utilisateur {id} non trouvées");

            return Ok(stats);
        }

        /// <summary>
        /// Récupère la progression d'un utilisateur
        /// </summary>
        [HttpGet("{id}/progression")]
        [Authorize] // Nécessite une authentification
        public async Task<ActionResult<IEnumerable<ProgressionUtilisateurDto>>> GetUserProgression(Guid id)
        {
            // Vérifier que l'utilisateur demande sa propre progression ou qu'il est admin
            var currentUserId = GetUserId();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (currentUserId != id && userRole != "Admin")
            {
                return Forbid("Vous ne pouvez consulter que votre propre progression.");
            }

            var progressions = await _utilisateurService.GetUserProgressionAsync(id);
            return Ok(progressions);
        }

        /// <summary>
        /// Récupère les statistiques de l'utilisateur connecté
        /// </summary>
        [HttpGet("mes-statistiques")]
        [Authorize] // Nécessite une authentification
        public async Task<ActionResult<StatistiquesGlobalesDto>> GetMyStatistics()
        {
            try
            {
                var utilisateurId = GetUserId();
                var stats = await _utilisateurService.GetUserStatisticsAsync(utilisateurId);
                if (stats == null)
                    return NotFound("Statistiques non trouvées");

                return Ok(stats);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Récupère la progression de l'utilisateur connecté
        /// </summary>
        [HttpGet("ma-progression")]
        [Authorize] // Nécessite une authentification
        public async Task<ActionResult<IEnumerable<ProgressionUtilisateurDto>>> GetMyProgression()
        {
            try
            {
                var utilisateurId = GetUserId();
                var progressions = await _utilisateurService.GetUserProgressionAsync(utilisateurId);
                return Ok(progressions);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}

