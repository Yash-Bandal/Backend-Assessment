
using Microsoft.Extensions.Options;
using PetPal.Models.Db;
using PetPal.Models.Filters;
using PetPal.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetPal.Repository
{
    public class PetRepository : BaseRepository<Pet>, IPetRepository
    {
        public PetRepository(IOptions<ConnectionString> connectionString)
            : base(connectionString.Value.DBConnection)
        {
        }

        public async Task<IEnumerable<Pet>> Get(PetFilter filter)
        {
            string query = @"
                SELECT
                    Id,
                    Name,
                    Gender,
                    Description,
                    BreedId,
                    OwnerId,
                    DateOfBirth,
                    IsAvailable,
                    CreatedAt
                FROM Pets
                WHERE (@IsAvailable IS NULL OR IsAvailable = @IsAvailable)";

            return await QueryAsync(query, 
                new
                {
                    filter.IsAvailable
                });
        }

        public async Task<Pet> Get(int petId)
        {
            const string query = @"
                         SELECT 
                            Id,
	                        Name,
	                        Gender,
	                        Description,
	                        BreedId,
	                        OwnerId,
	                        DateOfBirth,
	                        IsAvailable,
	                        CreatedAt
                        FROM Pets
                        WHERE Id = @PetId";

            return await QuerySingleAsync(query, new { PetId = petId });
        }

        public async Task<IEnumerable<Pet>> GetByOwnerId(int userId)
        {
            const string query = @"
                        SELECT
                            Id,
                            Name,
                            Gender,
                            Description,
                            BreedId,
                            OwnerId,
                            DateOfBirth,
                            IsAvailable,
                            CreatedAt
                        FROM Pets
                        WHERE OwnerId = @UserId";

            return await QueryAsync(query, new { UserId = userId });
        }


        public async Task<IEnumerable<Pet>> GetByOwnerIds(
            IEnumerable<int> userIds)
        {
            var ids = userIds.Distinct().ToArray();

            if (ids.Length == 0)
                return Enumerable.Empty<Pet>();

            const string query = @"
                        SELECT
                            Id,
                            Name,
                            Gender,
                            Description,
                            BreedId,
                            OwnerId,
                            DateOfBirth,
                            IsAvailable,
                            CreatedAt
                        FROM Pets
                        WHERE OwnerId IN @UserIds;";

            return await QueryAsync(query, new { UserIds = ids });
        }


    }
}
