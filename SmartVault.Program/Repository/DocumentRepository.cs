using Dapper;
using SmartVault.Program.BusinessObjects;
using SmartVault.Program.Interfaces;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace SmartVault.BusinessLogic.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly string _connectionString;

        public DocumentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<dynamic>> GetAllDocumentsAsync()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<dynamic>(
                    $"SELECT * FROM Document");
                return result;
            }
        }

        public async Task<IEnumerable<dynamic>> GetDocumentsForAccountAsync(int accountId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<dynamic>(
                    $"SELECT * FROM Document WHERE AccountId = {accountId}");
                return result;
            }
        }
    }


}
