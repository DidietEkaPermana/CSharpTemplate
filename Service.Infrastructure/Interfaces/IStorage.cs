using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Infrastructure.Models;

namespace Service.Infrastructure.Interfaces
{
    public interface IStorage
    {
        Task<string> SaveFileAsync(byte[] uploadFileStream, string container, string fileName);
        void DeleteFile(string uri);
        void DeleteFile(string container, string fileName);
        void DeleteFolder(string container, string folder);
        IList<FileProperty> ListFileOnFolder(string container, string folder, int pageSize, int pageIndex);
        int CountFileOnFolder(string container, string folder);
    }
}