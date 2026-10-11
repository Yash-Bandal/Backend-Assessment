
using PetPal.Models.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetPal.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Get(string email);
        Task<int> Create(User user);

        Task<IEnumerable<User>> GetByIds(IEnumerable<int> userIds);
    }
}
