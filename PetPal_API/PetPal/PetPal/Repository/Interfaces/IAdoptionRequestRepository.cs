using PetPal.Models.Db;
using System.Threading.Tasks;

namespace PetPal.Repository.Interfaces
{
    public interface IAdoptionRequestRepository
    {
        Task<int> Create(AdoptionRequest request);
    }
}
