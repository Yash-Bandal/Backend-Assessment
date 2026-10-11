using PetPal.Helpers;
using PetPal.Models.Db;
using PetPal.Repository.Interfaces;
using PetPal.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetPal.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Get(string email)
        {
            ValidationHelper.ValidateEmail(email);
            return await _userRepository.Get(email);
        }

        public async Task<int> Create(User user)
        {
            ValidationHelper.ValidateNull(user, "User");
            ValidationHelper.ValidateString(user.Name, "User Name");
            ValidationHelper.ValidateString(user.PhoneNumber, "User Phone Number");
            ValidationHelper.ValidateEmail(user.Email);
            ValidationHelper.ValidatePassword(user.Password);

            return await _userRepository.Create(user);
        }

        public async Task<IEnumerable<User>> GetByIds(IEnumerable<int> userIds)
        {
            ValidationHelper.ValidateList(userIds, "User List");
            return await _userRepository.GetByIds(userIds);
        }
    }
}
