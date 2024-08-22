using ClubTools.Shared;
using ClubTools.Shared.Models;
using System.Net.Http.Json;

namespace ClubTools.Website.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<bool>> Register(RegistrationModel registrationModel)
        {
            var response = await _httpClient.PostAsJsonAsync("/register", new { registrationModel.Email, registrationModel.Password });

            if (response.IsSuccessStatusCode)
            {
                return new ServiceResponse<bool> { IsSuccess = true };
            }
            else
            {
                //TODO: Replace with read as json async?
                var errorResponse = await response.Content.ReadAsStringAsync();
                return new ServiceResponse<bool> { IsSuccess = false, Message = errorResponse };
            }
        }

        public async Task<ServiceResponse<bool>> Login(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("/login?useCookies=true", new { loginModel.Email, loginModel.Password });

            if (response.IsSuccessStatusCode)
            {
                return new ServiceResponse<bool> { IsSuccess = true };
            }
            else
            {
                //TODO: Replace with read as json async?
                var errorResponse = await response.Content.ReadAsStringAsync();
                return new ServiceResponse<bool> { IsSuccess = false, Message = errorResponse };
            }
        }
    }
}
