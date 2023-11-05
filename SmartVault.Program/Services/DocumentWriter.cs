using SmartVault.Program.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SmartVault.BusinessLogic.Services
{
    public class DocumentWriter : IDocumentWriter
    {
        public async Task WriteAsync(string outputPath, string content)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentNullException(nameof(outputPath));
            }
            try
            {
                using var stream = new FileStream(outputPath, FileMode.Append, FileAccess.Write, FileShare.None);
                using var streamWriter = new StreamWriter(stream);
                await streamWriter.WriteLineAsync(content);
                Console.WriteLine($"The File content has been written to {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
            }
        }
    }

}
