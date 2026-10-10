using PetPal.Models.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetPal.Repository.Interfaces
{
    public interface IAdoptionRequestRepository
    {
        Task<int> Create(AdoptionRequest request);

        Task<IEnumerable<AdoptionRequest>> GetByPetId(int petId);
        Task<Dictionary<int, int>> GetRejectedCounts(IEnumerable<int> userIds);
        Task<IEnumerable<int>> GetUsersWithPendingBreedRequest(
            IEnumerable<int> userIds,
            int breedId,
            int currentPetId);

        Task<IEnumerable<int>> GetUsersWhoAdoptedBreed(
            IEnumerable<int> userIds,
            int breedId);

        Task<bool> Exists(int applicantId, int petId);
    }
}
