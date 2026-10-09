
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetPal.Repository
{
    public class BaseRepository<T> where T : class
    {
        private readonly string _connectionString;

        public BaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected async Task<IEnumerable<T>> QueryAsync(string query, object parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<T>(query, parameters);
        }

        protected async Task<IEnumerable<TResult>> QueryAsync<TResult>(string query, object parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<TResult>(query, parameters);
        }

        protected async Task<T> QuerySingleAsync(string query, object parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(query, parameters);
        }

        protected async Task<int> ExecuteAsync(string query, object parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(query, parameters);
        }

        protected async Task<TResult> ExecuteScalarAsync<TResult>(string query, object parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<TResult>(query, parameters);
        }
    }
}
