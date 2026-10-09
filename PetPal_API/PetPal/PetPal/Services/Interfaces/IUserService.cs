
using PetPal.Models.Db;
using System.Threading.Tasks;

namespace PetPal.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Get(string email);
        Task<User> Get(int id);
        Task<int> Create(User user);
    }
}
