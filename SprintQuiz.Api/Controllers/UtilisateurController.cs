using Microsoft.AspNetCore.Mvc;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SprintQuiz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilisateurController : ControllerBase
    {
        private readonly IUtilisateurService _utilisateurService;

        public UtilisateurController(IUtilisateurService utilisateurService)
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

        /// <summary>
        /// Inscrire un étudiant à une formation
        /// </summary>
        [HttpPost("inscription")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InscriptionDto>> Inscrire([FromBody] CreateInscriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _utilisateurService.InscrireEtudiantAsync(dto);
                return CreatedAtAction(nameof(GetInscriptionsParFormation), new { formationId = dto.FormationId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Liste des inscriptions pour une formation
        /// </summary>
        [HttpGet("formation/{formationId}")]
        public async Task<ActionResult<IEnumerable<InscriptionDto>>> GetInscriptionsParFormation(Guid formationId)
        {
            try
            {
                var inscriptions = await _utilisateurService.GetInscriptionsParFormationAsync(formationId);
                return Ok(inscriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Désinscrire un étudiant d'une formation (Admin uniquement)
        /// </summary>
        [HttpDelete("utilisateur/{utilisateurId}/formation/{formationId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DesinscrireDeFormation(Guid utilisateurId, Guid formationId)
        {
            try
            {
                var success = await _utilisateurService.DesinscrireEtudiantDeFormationAsync(utilisateurId, formationId);
                if (!success)
                    return NotFound("L'étudiant n'est pas inscrit à cette formation.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Récupère les inscriptions de l'utilisateur connecté
        /// </summary>
        [HttpGet("inscriptions")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<InscriptionDto>>> GetMesInscriptions()
        {
            try
            {
                var utilisateurId = GetUserId();
                var inscriptions = await _utilisateurService.GetInscriptionsUtilisateurAsync(utilisateurId);
                return Ok(inscriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les inscriptions d'un utilisateur spécifique (Admin ou soi-même)
        /// </summary>
        [HttpGet("{id}/inscriptions")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<InscriptionDto>>> GetInscriptionsUtilisateur(Guid id)
        {
            try
            {
                var currentUserId = GetUserId();
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (currentUserId != id && userRole != "Admin")
                    return Forbid("Vous ne pouvez consulter que vos propres inscriptions.");

                var inscriptions = await _utilisateurService.GetInscriptionsUtilisateurAsync(id);
                return Ok(inscriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }

        /// <summary>
        /// Vérifie si l'utilisateur est inscrit à une formation
        /// </summary>
        [HttpGet("inscriptions/{formationId}")]
        [Authorize]
        public async Task<ActionResult<bool>> EstInscritAFormation(Guid formationId)
        {
            try
            {
                var utilisateurId = GetUserId();
                var estInscrit = await _utilisateurService.EstInscritAFormationAsync(utilisateurId, formationId);
                return Ok(estInscrit);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }

    }
        
}

