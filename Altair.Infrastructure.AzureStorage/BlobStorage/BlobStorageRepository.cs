using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Altair.Infrastructure.BlobStorage
{
    public class BlobStorageRepository
    {
        private readonly CloudBlobContainer _cloudBlobContainer;
        public readonly string _containerName;
        public readonly string _connection;

        public BlobStorageRepository(string connection, string containerName)
        {
            _connection = connection;
            _containerName = containerName;
            _cloudBlobContainer = CreateReferenceBlob(_connection, _containerName);
        }

        public async Task<CloudBlockBlob> UploadFileAsync(string sourceFile, string localFileName)
        {
            CloudBlockBlob cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference(localFileName);
            await cloudBlockBlob.UploadFromFileAsync(sourceFile);
            return cloudBlockBlob;
        }

        public async Task<List<string>> GetFilesAsync()
        {
            List<string> files = new List<string>();
            BlobContinuationToken continuation = null;
            var list = await _cloudBlobContainer.ListBlobsSegmentedAsync(continuation);

            foreach (CloudBlockBlob item in list.Results)
            {
                files.Add(item.Name);
            }
            return files;
        }

        public async Task<CloudBlockBlob> UploadFromByteArrayAsync(string nameBlob, byte[] buffer)
        {
            CloudBlockBlob cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference(nameBlob);
            if (await cloudBlockBlob.ExistsAsync())
            {
                await CopyFile(cloudBlockBlob);
                await DeleteBlobAsync(cloudBlockBlob);
            }
            await cloudBlockBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Length);
            return cloudBlockBlob;
        }

        public async Task<CloudBlockBlob> UploadTextAsync(string nameBlob, string text)
        {
            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(nameBlob);
            if (await blockBlob.ExistsAsync())
            {
                await CopyFile(blockBlob);
                await DeleteBlobAsync(blockBlob);

            }
            await blockBlob.UploadTextAsync(text);
            return blockBlob;
        }

        public async Task<byte[]> DownloadBlobToByteArrayAsync(string fileName)
        {
            byte[] fileContent = null;
            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(fileName);
            if (await blockBlob.ExistsAsync())
            {
                await blockBlob.FetchAttributesAsync();
                long fileByteLength = blockBlob.Properties.Length;
                fileContent = new byte[fileByteLength];

                await blockBlob.DownloadToByteArrayAsync(fileContent, 0);
            }
            return fileContent;
        }

        public async Task<CloudBlockBlob> CopyFile(CloudBlockBlob blockBlob)
        {
            string[] blobNameExt = blockBlob.Name.Split('.');
            var newBlobName = string.Format("{0}_{1}.{2}", blobNameExt[0], DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"), blobNameExt[1]);
            byte[] content = new byte[blockBlob.Properties.Length];
            int result = await blockBlob.DownloadToByteArrayAsync(content, 0);
            if (result > 0 && content != null)
            {
                CloudBlockBlob newblockBlob = _cloudBlobContainer.GetBlockBlobReference(newBlobName);
                await newblockBlob.UploadFromByteArrayAsync(content, 0, content.Length);
                return newblockBlob;
            }
            return null;
        }

        public async Task DeleteBlobAsync(string blobName)
        {
            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(blobName);
            await DeleteBlobAsync(blockBlob);
        }

        public async Task DeleteBlobAsync(CloudBlockBlob blockBlob)
        {
            await blockBlob.DeleteIfExistsAsync();
        }

        public async Task<bool> DeleteBlobByFileName(string fileName)
        {
            try
            {
                var blob = this._cloudBlobContainer.GetBlockBlobReference(fileName);
                return await blob.DeleteIfExistsAsync();
            }
            catch
            {
                return false;
            }
        }

        #region private methods

        public CloudBlobContainer CreateReferenceBlob(string connection, string containerName)
        {
            CloudBlobContainer containerBlob = null;

            try
            {
                BlobContainerPermissions containerPermissions = new BlobContainerPermissions();

                if (CloudStorageAccount.TryParse(connection, out CloudStorageAccount account))
                {
                    CloudBlobClient client = account.CreateCloudBlobClient();
                    containerBlob = client.GetContainerReference(containerName);
                    containerBlob.CreateIfNotExistsAsync();

                    //WARNING: do not change access here or you will insta-open the whole container to public access.
                    containerPermissions.PublicAccess = BlobContainerPublicAccessType.Off;  //Keep Off			

                    containerBlob.SetPermissionsAsync(containerPermissions);
                    return containerBlob;
                }
                if (containerPermissions?.PublicAccess != BlobContainerPublicAccessType.Off)
                {
                    throw new Exception("incorrect security configuration");
                }
            }
            catch (Exception exp)
            {
                containerBlob = null;
            }

            return containerBlob;
        }

        #endregion
    }

}
 
