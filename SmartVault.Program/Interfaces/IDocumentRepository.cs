using SmartVault.Program.BusinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartVault.Program.Interfaces
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<dynamic>> GetDocumentsForAccountAsync(int accountId);
        Task<IEnumerable<dynamic>> GetAllDocumentsAsync();
    }

}
