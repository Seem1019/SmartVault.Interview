using Moq;
using SmartVault.BusinessLogic.Services;
using SmartVault.Program.BusinessObjects;
using SmartVault.Program.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartVault.Tests
{
    public class DocumentSizeCalculatorTest
    {
        [Fact]
        public async Task CalculateTotalSizeAsync_ReturnsCorrectSumOfDocumentSizes()
        {
            // Arrange
            var mockDocumentRepository = new Mock<IDocumentRepository>();
            var testDocuments = new List<Document>
            {
                new Document { FilePath = "test1.txt" },
                new Document { FilePath = "test2.txt" }
            };
            mockDocumentRepository.Setup(repo => repo.GetAllDocumentsAsync()).ReturnsAsync(testDocuments);

            // Use FileInfo to create test files and write some content to determine their size.
            foreach (var doc in testDocuments)
            {
                File.WriteAllText(doc.FilePath, "Test content");
            }

            var calculator = new DocumentSizeCalculator(mockDocumentRepository.Object);
            var expectedTotalSize = testDocuments.Sum(doc => new FileInfo(doc.FilePath).Length);

            // Act
            var totalSize = await calculator.CalculateTotalSizeAsync();

            // Assert
            Assert.Equal(expectedTotalSize, totalSize);

            // Cleanup
            foreach (var doc in testDocuments)
            {
                File.Delete(doc.FilePath);
            }
        }

        [Fact]
        public async Task CalculateTotalSizeAsync_IgnoresMissingFiles()
        {
            // Arrange
            var mockDocumentRepository = new Mock<IDocumentRepository>();
            var documentsWithMissing = new List<Document>
            {
                new Document { FilePath = "existing.txt" },
                new Document { FilePath = "missing.txt" }
            };
            mockDocumentRepository.Setup(repo => repo.GetAllDocumentsAsync()).ReturnsAsync(documentsWithMissing);

            // Create only one test file
            File.WriteAllText("existing.txt", "Test content");

            var calculator = new DocumentSizeCalculator(mockDocumentRepository.Object);
            var expectedSize = new FileInfo("existing.txt").Length;

            // Act
            var totalSize = await calculator.CalculateTotalSizeAsync();

            // Assert
            Assert.Equal(expectedSize, totalSize);

            // Cleanup
            File.Delete("existing.txt");
        }


    }
}
