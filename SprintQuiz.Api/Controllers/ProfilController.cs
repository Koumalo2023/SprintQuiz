using Microsoft.AspNetCore.Mvc;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SprintQuiz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Tous les endpoints de ce contrôleur nécessitent une authentification
    public class ProfilController : ControllerBase
    {
        private readonly IProfilService _profilService;
        private readonly ILogger<ProfilController> _logger;

        public ProfilController(IProfilService profilService, ILogger<ProfilController> logger)
        {
            _profilService = profilService;
            _logger = logger;
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
        /// Récupère les informations du profil de l'utilisateur connecté.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ProfilDto>> GetProfil()
        {
            try
            {
                var userId = GetUserId();
                var profil = await _profilService.GetProfilAsync(userId);
                if (profil == null)
                {
                    return NotFound("Profil utilisateur non trouvé.");
                }
                return Ok(profil);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Accès non autorisé au profil.");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du profil.");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }

        /// <summary>
        /// Met à jour les informations du profil de l'utilisateur connecté.
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<ProfilDto>> UpdateProfil([FromBody] UpdateProfilDto updateProfilDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetUserId();
                var updatedProfil = await _profilService.UpdateProfilAsync(userId, updateProfilDto);
                if (updatedProfil == null)
                {
                    return NotFound("Profil utilisateur non trouvé.");
                }
                return Ok(updatedProfil);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Accès non autorisé pour la mise à jour du profil.");
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du profil.");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }

        /// <summary>
        /// Upload une photo de profil pour l'utilisateur connecté.
        /// </summary>
        [HttpPost("photo")]
        public async Task<ActionResult<string>> UploadPhoto([FromBody] UploadPhotoDto uploadPhotoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetUserId();
                var photoUrl = await _profilService.UploadPhotoAsync(userId, uploadPhotoDto);
                if (photoUrl == null)
                {
                    return NotFound("Profil utilisateur non trouvé.");
                }
                return Ok(new { PhotoUrl = photoUrl });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Accès non autorisé pour l'upload de photo.");
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erreur lors de l'upload de la photo.");
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue lors de l'upload de la photo.");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }

        /// <summary>
        /// Change le mot de passe de l'utilisateur connecté.
        /// </summary>
        [HttpPut("password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetUserId();
                var success = await _profilService.ChangePasswordAsync(userId, changePasswordDto);
                if (!success)
                {
                    return NotFound("Utilisateur non trouvé.");
                }
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Accès non autorisé pour le changement de mot de passe.");
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du changement de mot de passe.");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }
    }
}

