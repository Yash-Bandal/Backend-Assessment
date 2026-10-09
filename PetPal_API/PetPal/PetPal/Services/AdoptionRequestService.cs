
using PetPal.Helpers;
using PetPal.Models.Db;
using PetPal.Repository.Interfaces;
using PetPal.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PetPal.Services
{
    public class AdoptionRequestService : IAdoptionRequestService
    {
        private readonly IAdoptionRequestRepository _adoptionRequestRepository;
        private readonly IPetService _petService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdoptionRequestService(
            IAdoptionRequestRepository adoptionRequestRepository,
            IPetService petService,
            IHttpContextAccessor httpContextAccessor)
        {
            _adoptionRequestRepository = adoptionRequestRepository;
            _petService = petService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> Create(int petId)
        {
            ValidationHelper.ValidatePositiveInt(petId, "Pet Id");

            var userIdClaim = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int applicantId))
            {
                throw new UnauthorizedAccessException("Authenticated user ID was not found.");
            }

            // Validate pet existence 
            Pet pet = await _petService.Get(petId);

            if (!pet.IsAvailable)
            {
                throw new ValidationException("This pet is not available for adoption.");
            }

            if (pet.OwnerId == applicantId)
            {
                throw new ValidationException("You cannot request adoption of your own pet.");
            }

            var request = new AdoptionRequest
            {
                ApplicantId = applicantId,
                PetId = petId
            };

            return await _adoptionRequestRepository.Create(request);
        }
    }
}
