using PetPal.Models.Db;
using System.Threading.Tasks;

namespace PetPal.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Get(string email);

        Task<User> Get(int id);

        Task<int> Create(User user);
    }
}
