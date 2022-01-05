using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Service.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Service.Infrastructure.Models;

namespace Service.Infrastructure.Storage
{
    public class AzureStorage : IStorage
    {
        private string _connectionString = string.Empty;
        private ILogger<AzureStorage> _logging;

        public AzureStorage(
            ILogger<AzureStorage> logging,
            IConfiguration config)
        {
            _connectionString = config.GetValue<string>("AzureStorage");
            _logging = logging;
        }

        public async Task<string> SaveFileAsync(byte[] uploadFileStream, string container, string fileName)
        {
            Stream stream = new MemoryStream(uploadFileStream);
            string cloudfilename = fileName.Replace('\\', '/');

            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, container);

            _logging.LogDebug("start GetBlobClient");
            BlobClient blobClient = containerClient.GetBlobClient(cloudfilename);
            _logging.LogDebug("ok GetBlobClient");

            _logging.LogDebug("Start UploadAsync");
            var returnResult = await blobClient.UploadAsync(stream, true);
            _logging.LogDebug("ok UploadAsync ==> {0}", returnResult.ToString());

            _logging.LogDebug("get Uri");
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
        
        public void DeleteFile(string container, string fileName)
        {
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, container);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            blobClient.DeleteIfExists();
        }

        public void DeleteFolder(string container, string folder)
        {
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, container);

            foreach (BlobItem blob in containerClient.GetBlobs(prefix: folder))
            {
                _logging.LogDebug("blob ==> {0}", blob.Name);

                containerClient.DeleteBlob(blob.Name);
            }
        }

        public IList<FileProperty> ListFileOnFolder(string container, string folder, int pageSize, int pageIndex)
        {
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, container);

            return containerClient.GetBlobs(prefix: folder).Select(p => new FileProperty()
            {
                Name = p.Name.Split('/').Last(),
                Size = p.Properties.ContentLength.GetValueOrDefault(),
                ModifiedOn = p.Properties.LastModified.ToString(),
                Uri = containerClient.Uri.AbsoluteUri + "/" + p.Name
            }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public int CountFileOnFolder(string container, string folder)
        {
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, container);

            return containerClient.GetBlobs(prefix: folder).Count();
        }
    }
}
