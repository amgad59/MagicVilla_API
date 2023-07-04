using MagicVilla_Web.Models.DTO;
using System.Linq.Expressions;

namespace MagicVilla_Web.Services.IServices
{
    public interface IVillaNumberService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(VillaNumberCreateDTO Entity);
        Task<T> UpdateAsync<T>(VillaNumberUpdateDTO Entity);
        Task<T> DeleteAsync<T>(int id);
    }
}
