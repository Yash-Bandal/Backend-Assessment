
using PetPal.Helpers;
using PetPal.Models.Authentication.Requests;
using PetPal.Models.Authentication.Responses;
using PetPal.Models.Db;
using PetPal.Services.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PetPal.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthService(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> SignUp(SignupRequest request)
        {
            ValidationHelper.ValidateNull(request, "Signup Request");

            request.Name = request.Name?.Trim();
            request.Email = request.Email?.Trim();
            request.PhoneNumber = request.PhoneNumber?.Trim();

            ValidationHelper.ValidateString(request.Name, "Name");
            ValidationHelper.ValidateEmail(request.Email);
            ValidationHelper.ValidateString(request.PhoneNumber, "Phone Number");
            ValidationHelper.ValidatePassword(request.Password);

            var existingUser = await _userService.Get(request.Email);

            if (existingUser != null)
            {
                throw new ValidationException("Email already exists.");
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber
            };

            user.Id = await _userService.Create(user);

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                AccessToken = token
            };
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            ValidationHelper.ValidateNull(request, "Login Request");

            request.Email = request.Email?.Trim();

            ValidationHelper.ValidateEmail(request.Email);
            ValidationHelper.ValidateString(request.Password, "Password");

            var user = await _userService.Get(request.Email);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new ValidationException("Invalid credentials.");
            }

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                AccessToken = token
            };
        }
    }
}
