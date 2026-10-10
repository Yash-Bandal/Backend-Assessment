using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetPal.Models.Responses;
using PetPal.Services.Interfaces;
using System.Threading.Tasks;

namespace PetPal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdoptionRequestsController : ControllerBase
    {
        private readonly IAdoptionRequestService _adoptionRequestService;

        public AdoptionRequestsController(IAdoptionRequestService adoptionRequestService)
        {
            _adoptionRequestService = adoptionRequestService;
        }

        [Authorize]
        [HttpPost("~/api/pets/{petId}/adoption-requests")]
        public async Task<IActionResult> Create(int petId)
        {
            var requestId = await _adoptionRequestService.Create(petId);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Adoption request created successfully",
                data = new { Id = requestId }
            });
        }

        [Authorize]
        [HttpGet("~/api/pets/{petId}/best-adopter")]
        public async Task<IActionResult> GetBestAdopter(int petId)
        {
            var result = await _adoptionRequestService.GetBestAdopter(petId);

            return Ok(new ApiResponse<BestAdopterResponse>
            {
                Success = true,
                Message = "Best adopter Results",
                data = result
            });
        }

    }
}
