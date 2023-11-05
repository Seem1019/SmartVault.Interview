using System.Threading.Tasks;

namespace SmartVault.Program.Interfaces
{
    public interface IDocumentProcessor
    {
        Task ProcessDocumentsForAccountAsync(int accountId, string outputPath);
    }
}
