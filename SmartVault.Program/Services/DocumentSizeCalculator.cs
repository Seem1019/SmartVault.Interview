using SmartVault.Program.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace SmartVault.BusinessLogic.Services
{
    public class DocumentSizeCalculator : IDocumentSizeCalculator
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentSizeCalculator(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<long> CalculateTotalSizeAsync()
        {
            long totalSize = 0;


            var documents = await _documentRepository.GetAllDocumentsAsync();
            foreach (var doc in documents)
            {

                var fileInfo = new FileInfo(doc.FilePath);
                if (fileInfo.Exists)
                {
                    totalSize += fileInfo.Length;
                }

            }

            return totalSize;
        }

    }

}
