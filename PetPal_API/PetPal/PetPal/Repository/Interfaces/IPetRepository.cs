using PetPal.Models.Db;
using PetPal.Models.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetPal.Repository.Interfaces
{
    public interface IPetRepository
    {
        Task<IEnumerable<Pet>> Get(PetFilter filter);
        Task<Pet> Get(int petId);
        Task<IEnumerable<Pet>> GetByOwnerId(int userId);

        Task<IEnumerable<Pet>> GetByOwnerIds(IEnumerable<int> userIds);
    }
}