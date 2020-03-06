using Service.Core.DTOs;
using Service.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Core.Services.Interfaces
{
    public interface IPropertyService
    {
        IList<PropertyOutput> Get();
        public PropertyOutput Get(string Id);
        IList<PropertyOutput> Page(int iPage, int iSize);
        PropertyOutput Add(PropertyInput newProperty);
        void Edit(string Id, PropertyInput newProperty);
        void Delete(string strGuid);
        Task<string> SaveFilesAsync(byte[] streamedFileContent, string fileName);
        void DeleteFiles(List<string> uri);
    }
}