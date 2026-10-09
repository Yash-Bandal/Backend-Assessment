using System.Threading.Tasks;

namespace PetPal.Services.Interfaces
{
    public interface IAdoptionRequestService
    {
        Task<int> Create(int petId);
    }
}
