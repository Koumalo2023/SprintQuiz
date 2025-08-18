using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SprintQuiz.Api.DTOs;
using SprintQuiz.Services;
using System.Security.Claims;

namespace SprintQuiz.Api.Controllers
{
    // Controllers/DashboardController.cs
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Récupère le tableau de bord de l'utilisateur connecté
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<DashboardDto>> GetDashboard()
        {
            var utilisateurId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(utilisateurId, out var id))
                return Unauthorized();

            try
            {
                var dashboard = await _dashboardService.GetDashboardAsync(id);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
