using System.Threading.Tasks;

namespace Service.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        IPropertyRepository PropertyRepository { get; }

        void Save();
        Task SaveAsync();
    }
}