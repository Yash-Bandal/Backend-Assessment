using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetPal.Models.Authentication.Requests;
using PetPal.Models.Authentication.Responses;
using PetPal.Models.Responses;
using PetPal.Services.Interfaces;
using System.Threading.Tasks;

namespace PetPal.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignupRequest request)
        {
            var response = await _authService.SignUp(request);
            return Ok(new ApiResponse<AuthResponse>
            {
                Success = true,
                Message = "Signup Successfull",
                data = response
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.Login(request);
            return Ok(new ApiResponse<AuthResponse>
            {
                Success = true,
                Message = "Login Successfull",
                data = response
            });
        }
    }
}
