using PetPal.Models.Db;
using PetPal.Models.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetPal.Services.Interfaces
{
    public interface IPetService
    {
        Task<IEnumerable<Pet>> Get(PetFilter filter);
        Task<Pet> Get(int id);
        Task<IEnumerable<Pet>> GetByOwnerId(int userId);

        Task<IEnumerable<Pet>> GetByOwnerIds(IEnumerable<int> userIds);
    }
}