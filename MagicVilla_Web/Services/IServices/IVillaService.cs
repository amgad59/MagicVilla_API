﻿using MagicVilla_Web.Models.DTO;
using System.Linq.Expressions;

namespace MagicVilla_Web.Services.IServices
{
    public interface IVillaService
    {
        Task<T> GetAllAsync<T>(string token);
        Task<T> GetAsync<T>(int id, string token);
        Task<T> CreateAsync<T>(VillaCreateDTO Entity, string token);
        Task<T> UpdateAsync<T>(VillaUpdateDTO Entity, string token);
        Task<T> DeleteAsync<T>(int id, string token);
    }
}
