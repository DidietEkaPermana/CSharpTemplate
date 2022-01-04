using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Service.Infrastructure.Interfaces;

namespace Service.Infrastructure.Storage
{
    public class AzureStorage : IStorage
    {
        private string _connectionString = string.Empty;

        public AzureStorage(
            IConfiguration config)
        {
            _connectionString = config.GetValue<string>("AzureStorage");
        }

        public async Task<string> SaveFileAsync(byte[] uploadFileStream, string container, string fileName)
        {
            Stream stream = new MemoryStream(uploadFileStream);
            string cloudfilename = fileName.Replace('\\', '/');

            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, container);

            BlobClient blobClient = containerClient.GetBlobClient(cloudfilename);
            await blobClient.UploadAsync(stream);

            return blobClient.Uri.ToString();
        }

        public void DeleteFile(string uri)
        {
            var strSplit = uri.Split('/');

            var cloudfilename = strSplit[strSplit.Length - 2] + "/" + strSplit[strSplit.Length - 1];

            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, strSplit[strSplit.Length - 3]);
            BlobClient blobClient = containerClient.GetBlobClient(cloudfilename);

            blobClient.DeleteIfExists();
        }
    }
}
