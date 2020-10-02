using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FizzWare.NBuilder;
using Document.Core.Interfaces;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using Document.Infrastructure.Repository;

namespace Document.Test
{
    public class Tests
    {
        private string storeConnection = "DefaultEndpointsProtocol=https;AccountName=containervic;AccountKey=geW60RjFtu5GVH8igFqpgIYnzG+iiguQzzCtSPEYvM8zj4ETW/5Fi4x1IesQJm/7KeLS38C8vT9UXZDu4qv27g==;EndpointSuffix=core.windows.net";
        private string storeName = "containervic";
        private IBlobRepository<CloudBlockBlob> _blobRepository = null;

        public Tests()
        {
            _blobRepository = new BlobRepository(storeConnection, storeName);
        }

        [Fact]
        public async Task SaveFile()
        {
           var result = await uploadDocument();
            var exists = await result.ExistsAsync();

            Assert.True(exists);
        }

        [Fact]
        public async Task DownloadFile()
        {
            await uploadDocument();
            var result = await _blobRepository.DownloadBlobToByteArrayAsync("file1.txt");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteFile()
        {
            await uploadDocument();
            var result = await _blobRepository.DeleteBlobByFileName("file1.txt");
            Assert.True(result);
        }

        private async Task<CloudBlockBlob> uploadDocument()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("sample data");
            writer.Flush();
            stream.Position = 0;

            return await _blobRepository.UploadFromByteArrayAsync("file1.txt", stream.ToArray());
        }
    }
}