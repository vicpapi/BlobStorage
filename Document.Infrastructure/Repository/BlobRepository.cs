using Altair.Infrastructure.BlobStorage;
using Document.Core.Interfaces;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Document.Infrastructure.Repository
{
    public class BlobRepository : BlobStorageRepository, IBlobRepository<CloudBlockBlob>
    {
        public BlobRepository(string connection, string containerName) : base(connection, containerName)
        { 
        }
         
    }

}
