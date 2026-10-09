using PetPal.Helpers;
using PetPal.Models.Db;
using PetPal.Models.Filters;
using PetPal.Repository.Interfaces;
using PetPal.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetPal.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;

        public PetService(IPetRepository petRepository)
        {
            _petRepository = petRepository;
        }

        public async Task<IEnumerable<Pet>> Get(PetFilter filter)
        {
            filter.IsAvailable ??= true;
            return await _petRepository.Get(filter);
        }

        public async Task<Pet> Get(int id)
        {
            ValidationHelper.ValidatePositiveInt(id, "Pet Id");
            var pet = await _petRepository.Get(id);
            ValidationHelper.ValidateNotFound(pet, "Pet");

            return pet;
        }

        public async Task<IEnumerable<Pet>> GetByOwnerId(int userId)
        {
            ValidationHelper.ValidatePositiveInt(userId, "User Id");
            return await _petRepository.GetByOwnerId(userId);
        }


        public async Task<IEnumerable<Pet>> GetByOwnerIds(IEnumerable<int> userIds)
        {
            ValidationHelper.ValidateList(userIds, "User List");
            return await _petRepository.GetByOwnerIds(userIds);
        }
    }
}