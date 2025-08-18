using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Api.Models;
using SprintQuiz.Api.Services; 

namespace SprintQuiz.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardAdminController : ControllerBase
    {
        private readonly IDashboardAdminService _adminService;

        public DashboardAdminController(IDashboardAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Récupère la vue d'ensemble de la promotion
        /// </summary>
        [HttpGet("overview")]
        public async Task<ActionResult<DashboardOverviewDto>> GetOverview()
        {
            try
            {
                var data = await _adminService.GetOverviewAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère la progression moyenne par niveau (Sprint, Module, etc.)
        /// </summary>
        [HttpGet("progression")]
        public async Task<ActionResult<DashboardProgressionDto>> GetProgressionParNiveau(
            [FromQuery] NiveauEnum niveau,
            [FromQuery] Guid niveauId)
        {
            if (!Enum.IsDefined(typeof(NiveauEnum), niveau))
                return BadRequest("Niveau invalide.");

            try
            {
                var data = await _adminService.GetProgressionParNiveauAsync(niveau, niveauId);
                return Ok(data);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Niveau non trouvé.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }

        /// <summary>
        /// Analyse détaillée d'un quiz (taux de réussite, questions les plus ratées)
        /// </summary>
        [HttpGet("quiz/{quizId}/analyse")]
        public async Task<ActionResult<QuizAnalyseDto>> GetAnalyseQuiz(Guid quizId)
        {
            try
            {
                var data = await _adminService.GetAnalyseQuizAsync(quizId);
                return Ok(data);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Quiz non trouvé.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }

        /// <summary>
        /// Liste les utilisateurs inactifs depuis N jours
        /// </summary>
        [HttpGet("alertes/inactifs")]
        public async Task<ActionResult<AlerteInactifsDto>> GetAlertesInactifs(
            [FromQuery] int jours = 7)
        {
            if (jours <= 0) return BadRequest("Le nombre de jours doit être positif.");

            try
            {
                var data = await _adminService.GetAlertesInactifsAsync(jours);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }

        /// <summary>
        /// Liste les utilisateurs en difficulté (progression < seuil)
        /// </summary>
        [HttpGet("alertes/difficulte")]
        public async Task<ActionResult<AlerteDifficulteDto>> GetUtilisateursEnDifficulte(
            [FromQuery] float seuil = 50.0f)
        {
            if (seuil <= 0 || seuil > 100)
                return BadRequest("Le seuil doit être entre 0 et 100.");

            try
            {
                var data = await _adminService.GetUtilisateursEnDifficulteAsync(seuil);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }

        /// <summary>
        /// Historique d'activité collective (utilisateurs actifs, tentatives par jour)
        /// </summary>
        [HttpGet("historique")]
        public async Task<ActionResult<HistoriqueActiviteDto>> GetHistorique(
            [FromQuery] int periode = 30)
        {
            if (periode <= 0 || periode > 365)
                return BadRequest("La période doit être entre 1 et 365 jours.");

            try
            {
                var data = await _adminService.GetHistoriqueAsync(periode);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }

        /// <summary>
        /// Top et flop des contenus (quiz, modules)
        /// </summary>
        [HttpGet("top-flop")]
        public async Task<ActionResult<TopFlopDto>> GetTopFlop()
        {
            try
            {
                var data = await _adminService.GetTopFlopAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }

        
    }
}