using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetPal.Models.Authentication.Responses;
using PetPal.Models.Db;
using PetPal.Models.Filters;
using PetPal.Models.Responses;
using PetPal.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetPal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;

        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PetFilter filter)
        {
            var pets = await _petService.Get(filter);

            return Ok(new ApiResponse<IEnumerable<Pet>>
            {
                Success = true,
                Message = "Pets fetched successfully",
                data = pets
            });
        }

        [Authorize]
        [HttpGet("~/api/users/{userId}/pets")]
        public async Task<IActionResult> GetByOwnerId(int userId)
        {
            var pets = await _petService.GetByOwnerId(userId);

            return Ok(new ApiResponse<IEnumerable<Pet>>
            {
                Success = true,
                Message = "User pets fetched successfully",
                data = pets
            });
        }

    }
}