using System.Collections.Generic;
using System.Threading.Tasks;

namespace Document.Core.Interfaces
{
    public interface IBlobRepository<T>
    {
        Task<List<string>> GetFilesAsync();
        Task<T> UploadFileAsync(string sourceFile, string localFileName);
        Task<T> UploadFromByteArrayAsync(string nameBlob, byte[] buffer);
        Task<byte[]> DownloadBlobToByteArrayAsync(string localFileName);
        Task<bool> DeleteBlobByFileName(string fileName);
    }
}
