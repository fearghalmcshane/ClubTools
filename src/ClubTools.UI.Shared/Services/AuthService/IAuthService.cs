using ClubTools.Shared;
using ClubTools.Shared.Models;

namespace ClubTools.UI.Shared.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<bool>> Register(RegistrationModel registrationModel);

        Task<ServiceResponse<bool>> Login(LoginModel loginModel);
    }
}
