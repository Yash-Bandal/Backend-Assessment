using System;
using Microsoft.AspNetCore.Http;
using PetPal.Helpers;
using PetPal.Models;
using PetPal.Models.Db;
using PetPal.Models.Responses;
using PetPal.Repository.Interfaces;
using PetPal.Services.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PetPal.Services
{
    public class AdoptionRequestService : IAdoptionRequestService
    {
        private readonly IAdoptionRequestRepository _adoptionRequestRepository;
        private readonly IPetService _petService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public AdoptionRequestService(
            IAdoptionRequestRepository adoptionRequestRepository,
            IPetService petService,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService)
        {
            _adoptionRequestRepository = adoptionRequestRepository;
            _petService = petService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public async Task<int> Create(int petId)
        {
            ValidationHelper.ValidatePositiveInt(petId, "Pet Id");

            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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


            // Prevent duplicate adoption requests
            if (await _adoptionRequestRepository.Exists(applicantId, petId))
            {
                throw new ValidationException("You have already submitted an adoption request for this pet.");
            }


            var request = new AdoptionRequest
            {
                ApplicantId = applicantId,
                PetId = petId
            };

            return await _adoptionRequestRepository.Create(request);
        }

        public async Task<BestAdopterResponse> GetBestAdopter(int petId)
        {
            // 1. Fetch the pet and validate its existence.
            var pet = await _petService.Get(petId);

            // 2. Fetch all pending adoption requests for this pet.
            var pendingRequests = (await _adoptionRequestRepository.GetByPetId(petId))
                .Where(r => r.Status == "Pending")
                .ToList();

            if (!pendingRequests.Any())
            {
                return null;
            }

            // 3. Get distinct applicant IDs.
            var userIds = pendingRequests
                .Select(r => r.ApplicantId)
                .Distinct()
                .ToArray();

            // 4. Fetch all applicants in one query.
            var users = (await _userService.GetByIds(userIds))
                .ToDictionary(u => u.Id);

            // 5. Fetch all pets currently owned by these applicants.
            var ownedPets = (await _petService.GetByOwnerIds(userIds))
                .ToList();

            // Group pets by owner for quick lookups.
            var petsByOwner = ownedPets
                .GroupBy(p => p.OwnerId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 6. Fetch rejected-request counts.
            var rejectedCountsDict = await _adoptionRequestRepository.GetRejectedCounts(userIds);

            // 7. Find applicants with pending requests for the same breed.
            var usersWithPendingBreedRequest =
                (await _adoptionRequestRepository.GetUsersWithPendingBreedRequest(
                    userIds,
                    pet.BreedId,
                    pet.Id))
                .ToHashSet();

            // 8. Find applicants who previously adopted the same breed.
            var usersWhoAdoptedSameBreed =
                (await _adoptionRequestRepository.GetUsersWhoAdoptedBreed(
                    userIds,
                    pet.BreedId))
                .ToHashSet();

            var candidates = new List<BestAdopterCandidate>();

            // 9. Filter and prepare eligible candidates.
            foreach (var request in pendingRequests)
            {
                if (!users.TryGetValue(request.ApplicantId, out var user))
                {
                    continue;
                }

                var userPets = petsByOwner.TryGetValue(user.Id, out var pets)
                    ? pets
                    : new List<Pet>();

                int ownedPetsCount = userPets.Count;

                // Rule 1: Exclude applicants who own more than 3 pets.
                if (ownedPetsCount > 3)
                {
                    continue;
                }

                // Rule 2: Exclude applicants with a pending request
                // for another pet of the same breed.
                if (usersWithPendingBreedRequest.Contains(user.Id))
                {
                    continue;
                }

                rejectedCountsDict.TryGetValue(user.Id, out int rejectedCount);

                candidates.Add(new BestAdopterCandidate
                {
                    User = user,
                    Request = request,
                    OwnedPetsCount = ownedPetsCount,
                    RejectedCount = rejectedCount,
                    HasAdoptedSameBreed = usersWhoAdoptedSameBreed.Contains(user.Id)
                });
            }

            // 10. Return null if nobody qualifies.
            if (!candidates.Any())
            {
                return null;
            }

            // 11. Rank candidates using the specified priorities.
            var bestCandidate = candidates
                // Deprioritize applicants with more than 2 rejections.
                .OrderBy(c => c.RejectedCount > 2 ? 1 : 0)
                // Primary criteria. - bool true first
                .ThenByDescending(c => c.HasAdoptedSameBreed)
                .ThenByDescending(c => c.OwnedPetsCount > 0)
                .ThenBy(c => c.User.CreatedAt)
                // Tie-breakers.  (Eligible -> Post Filter)
                .ThenBy(c => c.OwnedPetsCount)  
                .ThenBy(c => c.Request.RequestedAt)
                .FirstOrDefault();

            if (bestCandidate == null)
            {
                return null;
            }

            // 12. Return the best adopter's details.
            return new BestAdopterResponse
            {
                UserId = bestCandidate.User.Id,
                FullName = bestCandidate.User.Name,
                Email = bestCandidate.User.Email
            };
        }


    }
}
