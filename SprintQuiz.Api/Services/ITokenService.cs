using SprintQuiz.Api.Models;

namespace SprintQuiz.Api.Services
{
    public interface ITokenService
    {
        string GenerateToken(Utilisateur utilisateur);
    }
}

