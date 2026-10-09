using Microsoft.AspNetCore.Mvc;
using PetPal.Models.Authentication.Requests;
using PetPal.Services.Interfaces;
using System.Threading.Tasks;

namespace PetPal.Controllers
{
    [ApiController]
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
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.Login(request);
            return Ok(response);
        }
    }
}
