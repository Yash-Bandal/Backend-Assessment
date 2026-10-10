
using Microsoft.Extensions.Options;
using PetPal.Models.Db;
using PetPal.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
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

        // Get adoption requests for a pet
        public async Task<IEnumerable<AdoptionRequest>> GetByPetId(int petId)
        {
            const string query = @"
                        SELECT 
                            Id,
	                        ApplicantId,
	                        PetId,
	                        STATUS,
	                        RequestedAt
                        FROM AdoptionRequests
                        WHERE PetId = @PetId";

            return await QueryAsync(query, new { PetId = petId });
        }

        //Get rejected-request counts for multiple applicants
        private class RejectedCountRow
        {
            public int ApplicantId { get; set; }
            public int RejectedCount { get; set; }
        }

        public async Task<Dictionary<int, int>> GetRejectedCounts(
            IEnumerable<int> userIds)
        {
            var ids = userIds.Distinct().ToArray();

            if (ids.Length == 0)
                return new Dictionary<int, int>();

            const string query = @"
                     SELECT 
                        ApplicantId,
                        COUNT(*) AS RejectedCount
                     FROM AdoptionRequests
                     WHERE ApplicantId IN @UserIds
                        AND Status = 'Rejected'
                     GROUP BY ApplicantId;";

            var rows = await QueryAsync<RejectedCountRow>(
                query, new { UserIds = ids });

            return rows.ToDictionary(
                x => x.ApplicantId,
                x => x.RejectedCount);
        }

        // Find applicants with pending requests for the same breed
        public async Task<IEnumerable<int>> GetUsersWithPendingBreedRequest(
            IEnumerable<int> userIds,
            int breedId,
            int currentPetId)
        {
            var ids = userIds.Distinct().ToArray();

            if (ids.Length == 0)
                return Enumerable.Empty<int>();

            const string query = @"
                  SELECT DISTINCT
                    ar.ApplicantId
                  FROM AdoptionRequests ar
                    INNER JOIN Pets p ON p.Id = ar.PetId
                  WHERE ar.ApplicantId IN @UserIds
                      AND ar.Status = 'Pending'
                      AND p.BreedId = @BreedId
                      AND p.Id <> @CurrentPetId;";

            return await QueryAsync<int>(query, new
            {
                UserIds = ids,
                BreedId = breedId,
                CurrentPetId = currentPetId
            });
        }


        // 4. Find applicants who previously adopted the same breed
        public async Task<IEnumerable<int>> GetUsersWhoAdoptedBreed(IEnumerable<int> userIds, int breedId)
        {
            var ids = userIds.Distinct().ToArray();

            if (ids.Length == 0)
                return Enumerable.Empty<int>();

            const string query = @"
                    SELECT DISTINCT
                        ar.ApplicantId
                    FROM AdoptionRequests ar
                    INNER JOIN Pets p ON p.Id = ar.PetId
                    WHERE ar.ApplicantId IN @UserIds
                        AND ar.Status = 'Approved'
                        AND p.BreedId = @BreedId;";

            return await QueryAsync<int>(query, new
            {
                UserIds = ids,
                BreedId = breedId
            });
        }

        public async Task<bool> Exists(int applicantId, int petId)
        {
            const string query = @"
                    SELECT 
                        COUNT(*)
                    FROM AdoptionRequests
                    WHERE ApplicantId = @ApplicantId
                        AND PetId = @PetId;";

            var count = await ExecuteScalarAsync<int>(query, new
            {
                ApplicantId = applicantId,
                PetId = petId
            });

            return count > 0;
        }
    }
}
