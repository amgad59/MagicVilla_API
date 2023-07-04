using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaRepository : Repository<Villa> ,  IVillaRepository
    {
        private readonly ApplicationDbContext _db;

        public VillaRepository(ApplicationDbContext db) : base (db)
        {
            _db = db;
        }
      
        public async Task<Villa> Update(Villa Entity)
        {
            Entity.UpdatedDate = DateTime.Now;
            _db.villas.Update(Entity);
            await Save();
            return Entity;
        }
    }
}
