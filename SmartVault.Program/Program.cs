using Microsoft.Extensions.Configuration;
using SmartVault.BusinessLogic.Repository;
using SmartVault.BusinessLogic.Services;
using SmartVault.Program.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SmartVault.Program
{
    partial class Program
    {
        static async Task Main(string[] args)
        {
            // Build the configuration from the 'appsettings.json' file
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            // Attempt to parse account ID from command-line arguments
            int accountId;
            if (args.Length > 0 && int.TryParse(args[0], out accountId))
            {
                // If there is an argument and it's an integer, use it as accountId.
            }
            else
            {
                accountId = 1; // Default value if no valid argument is provided.
            }

            // Format the connection string using the database file name from configuration
            var connectionString = string.Format(configuration?["ConnectionStrings:DefaultConnection"] ?? "", configuration?["DatabaseFileName"]);

            // Initialize repositories and services
            var documentRepository = new DocumentRepository(connectionString);
            var documentWriter = new DocumentWriter();
            var documentProcessor = new DocumentProcessor(documentRepository, documentWriter);

            // Define the output path for the processed documents
            var outputPath = string.Format(configuration?["OutputPath"] ?? "output.txt");

            // Process documents and calculate file sizes
            await WriteEveryThirdFileToFile(accountId, outputPath, documentProcessor);
            await GetAllFileSizes(documentRepository);
        }

        private static async Task GetAllFileSizes(IDocumentRepository documentRepository)
        {
            // Create a document size calculator and calculate the total size
            var documentSizeCalculator = new DocumentSizeCalculator(documentRepository);
            Stopwatch stopwatch = Stopwatch.StartNew();
            long totalSize = await documentSizeCalculator.CalculateTotalSizeAsync();
            stopwatch.Stop();
            Console.WriteLine($"Tiempo de ejecución: {stopwatch.ElapsedMilliseconds} milisegundos");
            Console.WriteLine($"Tiempo de ejecución: {stopwatch.Elapsed.TotalSeconds} segundos");
            Console.WriteLine($"Total size of all files: {totalSize} bytes");
        }

        private static async Task WriteEveryThirdFileToFile(int accountId, string outputPath, IDocumentProcessor documentProcessor)
        {
            // Process documents for the given account and write to the output path
            await documentProcessor.ProcessDocumentsForAccountAsync(accountId, outputPath);

        }
    }
}
