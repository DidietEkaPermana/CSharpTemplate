using Service.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Service.Infrastructure.Models;
using Microsoft.Extensions.Logging;
namespace Service.Infrastructure.Storage
{
    public class LocalStorage : IStorage
    {
        private string _targetFilePath = string.Empty;
        private ILogger<LocalStorage> _logging;

        public LocalStorage(ILogger<LocalStorage> logging, IConfiguration config)
        {
            // To save physical files to a path provided by configuration:
            _targetFilePath = config.GetValue<string>("LocalStorage");
            _logging = logging;
        }

        public async Task<string> SaveFileAsync(byte[] uploadFileStream, string container, string fileName)
        {
            if (!string.IsNullOrEmpty(_targetFilePath))
                container = Path.Combine(_targetFilePath, container);

            fileName = Path.Combine(container, fileName);

            using (var targetStream = File.Create(fileName))
            {
                await targetStream.WriteAsync(uploadFileStream);
                return fileName;
            }
        }

        public void DeleteFile(string uri)
        {
            File.Delete(uri);
        }

        public void DeleteFile(string container, string fileName)
        {

        }

        public void DeleteFolder(string container, string folder)
        {

        }

        public IList<FileProperty> ListFileOnFolder(string container, string folder, int pageSize, int pageIndex)
        {
            return null;
        }

        public int CountFileOnFolder(string container, string folder)
        {
            return 0;
        }
    }
}
