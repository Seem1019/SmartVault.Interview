using SmartVault.Program.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.BusinessLogic.Services
{
    public class DocumentProcessor : IDocumentProcessor
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentWriter _documentWriter;
        private const string SearchText = "Smith Property";

        public DocumentProcessor(IDocumentRepository documentRepository, IDocumentWriter documentWriter)
        {
            _documentRepository = documentRepository;
            _documentWriter = documentWriter;
        }

        public async Task ProcessDocumentsForAccountAsync(int accountId, string outputPath)
        {
            var documents = await _documentRepository.GetDocumentsForAccountAsync(accountId);
            var stringBuilder = new StringBuilder();

            // Accumulate the content of every third document that contains the search text
            for (int i = 2; i < documents.Count(); i += 3)
            {
                dynamic document = documents.ElementAt(i);
                if (!File.Exists(document.FilePath))
                {
                    throw new FileNotFoundException($"The file {document.FilePath} was not found.");
                }

                string content = await File.ReadAllTextAsync(document.FilePath);
                if (content.Contains(SearchText))
                {
                    stringBuilder.AppendLine(content);
                }
            }

            // Write all accumulated content to the output file at once if there is any content
            if (stringBuilder.Length > 0)
            {
                await _documentWriter.WriteAsync(outputPath, stringBuilder.ToString());
            }
        }

    }

}
