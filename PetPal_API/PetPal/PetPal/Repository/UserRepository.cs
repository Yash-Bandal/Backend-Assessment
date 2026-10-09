
using Microsoft.Extensions.Options;
using PetPal.Models.Db;
using PetPal.Repository.Interfaces;
using System.Threading.Tasks;

namespace PetPal.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IOptions<ConnectionString> connectionString)
            : base(connectionString.Value.DBConnection)
        {
        }

        public async Task<User> Get(string email)
        {
            const string query = @"
                            SELECT
                                Id,
	                            Name,
	                            Email,
	                            Password,
	                            PhoneNumber,
	                            CreatedAt
                            FROM Users
                            WHERE Email = @Email";

            return await QuerySingleAsync(query, new { Email = email });
        }

        public async Task<User> Get(int id)
        {
            const string query = @"
                            SELECT 
                                Id,
	                            Name,
	                            Email,
	                            Password,
	                            PhoneNumber,
	                            CreatedAt
                            FROM Users
                            WHERE Id = @Id";

            return await QuerySingleAsync(query, new { Id = id });
        }

        public async Task<int> Create(User user)
        {
            const string query = @"
                        INSERT INTO Users 
                        (
	                        Name,
	                        Email,
	                        Password,
	                        PhoneNumber
                        )
                        VALUES 
                        (
	                        @Name,
	                        @Email,
	                        @Password,
	                        @PhoneNumber
                        );

                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await ExecuteScalarAsync<int>(query, user);
        }

    }
}
