using MagicVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        Task<T> Get(Expression<Func<T, bool>>? filter = null, bool isTracked = true,string? includeProperties = null);
        Task CreateVilla(T Entity);
        Task Remove(T Entity);
        Task Save();
    }
}
