using MagicVilla_Web.Models.DTO;
using System.Linq.Expressions;

namespace MagicVilla_Web.Services.IServices
{
    public interface IVillaService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(VillaCreateDTO Entity);
        Task<T> UpdateAsync<T>(VillaUpdateDTO Entity);
        Task<T> DeleteAsync<T>(int id);
    }
}
