using PetPal.Models.Authentication.Requests;
using PetPal.Models.Authentication.Responses;
using System.Threading.Tasks;

namespace PetPal.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> SignUp(SignupRequest request);
        Task<AuthResponse> Login(LoginRequest request);
    }
}
