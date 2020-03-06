using System.Threading.Tasks;

namespace Service.Infrastructure.Interfaces
{
    public interface IStorage
    {
        Task<string> SaveFileAsync(byte[] uploadFileStream, string container, string fileName);
        void DeleteFile(string uri);
    }
}