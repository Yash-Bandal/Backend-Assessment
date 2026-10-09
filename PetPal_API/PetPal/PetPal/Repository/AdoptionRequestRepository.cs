
using Microsoft.Extensions.Options;
using PetPal.Models.Db;
using PetPal.Repository.Interfaces;
using System.Threading.Tasks;

namespace PetPal.Repository
{
    public class AdoptionRequestRepository
        : BaseRepository<AdoptionRequest>, IAdoptionRequestRepository
    {
        public AdoptionRequestRepository(IOptions<ConnectionString> connectionString)
            : base(connectionString.Value.DBConnection)
        {
        }

        public async Task<int> Create(AdoptionRequest request)
        {
            const string query = @"
                    INSERT INTO AdoptionRequests 
                    (
	                    ApplicantId,
	                    PetId
                    )
                    VALUES 
                    (
	                    @ApplicantId,
	                    @PetId
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await ExecuteScalarAsync<int>(query, request);
        }
    }
}
