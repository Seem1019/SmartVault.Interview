using Moq;
using SmartVault.BusinessLogic.Services;
using SmartVault.Program.BusinessObjects;
using SmartVault.Program.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SmartVault.Tests
{



    public class DocumentProcessorTests
    {
        [Fact]
        public async Task ProcessDocumentsForAccountAsync_AccumulatesAndWritesCorrectContent()
        {
            // Arrange
            var mockRepository = new Mock<IDocumentRepository>();
            var mockWriter = new Mock<IDocumentWriter>();
            var testPath = Path.Combine(Path.GetTempPath(), "DocumentProcessorTests");
            Directory.CreateDirectory(testPath);

            // Crear documentos de prueba y contenido
            var documents = new List<Document>();
            for (int i = 0; i < 9; i++)
            {
                var filePath = Path.Combine(testPath, $"doc{i}.txt");
                var content = i % 3 == 2 ? "Content with Smith Property" : "Other Content";
                await File.WriteAllTextAsync(filePath, content);
                documents.Add(new Document { FilePath = filePath });
            }

            mockRepository.Setup(r => r.GetDocumentsForAccountAsync(It.IsAny<int>())).ReturnsAsync(documents);
            var processor = new DocumentProcessor(mockRepository.Object, mockWriter.Object);

            // Act
            await processor.ProcessDocumentsForAccountAsync(123, "output.txt");

            // Assert
            var expectedContent = string.Join(
                Environment.NewLine,
                documents.Where((_, i) => (i + 1) % 3 == 0 && File.ReadAllText(documents[i].FilePath).Contains("Smith Property"))
                         .Select(doc => File.ReadAllText(doc.FilePath))
            ) + Environment.NewLine;

            mockWriter.Verify(w => w.WriteAsync("output.txt", expectedContent), Times.Once());

            // Cleanup
            Directory.Delete(testPath, true);
        }

        [Fact]
        public async Task ProcessDocumentsForAccountAsync_DoesNotWrite_WhenSearchTextIsNotFound()
        {
            // Arrange
            var mockRepository = new Mock<IDocumentRepository>();
            var mockWriter = new Mock<IDocumentWriter>();
            var documents = CreateTestDocumentsWithoutSearchText();
            mockRepository.Setup(repo => repo.GetDocumentsForAccountAsync(It.IsAny<int>()))
                          .ReturnsAsync(documents);
            var processor = new DocumentProcessor(mockRepository.Object, mockWriter.Object);

            // Act
            await processor.ProcessDocumentsForAccountAsync(123, "output.txt");

            // Assert
            mockWriter.Verify(writer => writer.WriteAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async Task ProcessDocumentsForAccountAsync_HandlesExceptions_WhenFileReadFails()
        {
            // Arrange
            var mockRepository = new Mock<IDocumentRepository>();
            var mockWriter = new Mock<IDocumentWriter>();
            var documents = CreateTestDocumentsWithInaccessibleFiles();
            mockRepository.Setup(repo => repo.GetDocumentsForAccountAsync(It.IsAny<int>()))
                          .ReturnsAsync(documents);
            var processor = new DocumentProcessor(mockRepository.Object, mockWriter.Object);

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(async () =>
                await processor.ProcessDocumentsForAccountAsync(123, "output.txt"));
        }

        private List<Document> CreateTestDocumentsWithoutSearchText()
        {
            var testPath = Path.GetTempPath();
            var documents = new List<Document>();

            for (int i = 0; i < 9; i++)
            {
                var filePath = Path.Combine(testPath, $"docWithoutSearchText{i}.txt");
                File.WriteAllText(filePath, "This is a test content without the key phrase.");
                documents.Add(new Document { FilePath = filePath });
            }

            return documents;
        }


        private List<Document> CreateTestDocumentsWithInaccessibleFiles()
        {
            var testPath = Path.GetTempPath();
            var documents = new List<Document>();

            for (int i = 0; i < 3; i++)
            {
                var filePath = Path.Combine(testPath, $"docInaccessible{i}.txt");
                if (i != 2)
                {
                    File.WriteAllText(filePath, "This is a test content that is accessible.");
                }
                documents.Add(new Document { FilePath = filePath });
            }

            File.Delete(documents[2].FilePath);

            return documents;
        }


    }




}
