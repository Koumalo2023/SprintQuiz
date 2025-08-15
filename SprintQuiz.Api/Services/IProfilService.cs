using SprintQuiz.Api.DTOs;

namespace SprintQuiz.Api.Services
{
    public interface IProfilService
    {
        Task<ProfilDto?> GetProfilAsync(Guid utilisateurId);
        Task<ProfilDto?> UpdateProfilAsync(Guid utilisateurId, UpdateProfilDto updateProfilDto);
        Task<string?> UploadPhotoAsync(Guid utilisateurId, UploadPhotoDto uploadPhotoDto);
        Task<bool> ChangePasswordAsync(Guid utilisateurId, ChangePasswordDto changePasswordDto);
        Task<DernieresActivitesDto> GetDernieresActivitesAsync(Guid utilisateurId);
        Task<float> CalculateProgressionGlobaleAsync(Guid utilisateurId);
    }
}

