using SmartVault.BusinessLogic.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SmartVault.Tests
{
    public class DocumentWriterTests
    {
        [Fact]
        public async Task WriteAsync_AppendsContentToFile()
        {
            // Arrange
            var documentWriter = new DocumentWriter();
            var testPath = Path.GetTempFileName();
            var content = "Test content";

            // Act
            await documentWriter.WriteAsync(testPath, content);

            // Assert
            var writtenContent = await File.ReadAllTextAsync(testPath);
            Assert.Contains(content, writtenContent);

            // Cleanup
            File.Delete(testPath);
        }

        [Fact]
        public async Task WriteAsync_HandlesExceptions_WhenInvalidPath()
        {
            // Arrange
            var documentWriter = new DocumentWriter();
            var invalidPath = "";
            var content = "Test content";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await documentWriter.WriteAsync(invalidPath, content));
        }

        [Fact]
        public async Task WriteAsync_AppendsContent_WhenFileAlreadyHasContent()
        {
            // Arrange
            var documentWriter = new DocumentWriter();
            var testPath = Path.GetTempFileName();
            var initialContent = "Initial content";
            File.WriteAllText(testPath, initialContent);
            var contentToAppend = "Appended content";

            // Act
            await documentWriter.WriteAsync(testPath, contentToAppend);

            // Assert
            var writtenContent = await File.ReadAllTextAsync(testPath);
            Assert.Contains(initialContent, writtenContent);
            Assert.Contains(contentToAppend, writtenContent);
            Assert.EndsWith(contentToAppend + Environment.NewLine, writtenContent);

            // Cleanup
            File.Delete(testPath);
        }


    }
}
