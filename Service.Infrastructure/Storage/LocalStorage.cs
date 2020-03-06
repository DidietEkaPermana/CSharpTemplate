using Service.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Service.Infrastructure.Storage
{
    public class LocalStorage : IStorage
    {
        private string _targetFilePath = string.Empty;

        public LocalStorage(IConfiguration config)
        {
            // To save physical files to a path provided by configuration:
            _targetFilePath = config.GetValue<string>("LocalStorage");
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
    }
}
