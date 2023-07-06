
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db) {
            _db = db;
            dbSet = _db.Set<T>();
        }
        public async Task CreateVilla(T Entity)
        {
            await dbSet.AddAsync(Entity);
            await Save();
        }

        public async Task<T> Get(Expression<Func<T, bool>>? filter = null, bool isTracked = true,string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (!isTracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if(includeProperties != null)
            {
                foreach(var property in includeProperties.Split(new char[','],StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include(property);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null,string ? includeProperties = null, int pageSize = 0, int pageNumber = 1)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
			}

            if(pageSize > 0)
            {
                if(pageSize > 100)
                {
                    pageSize = 100;
                }
                query = query.Skip(pageSize*(pageNumber-1)).Take(pageSize);
            }
			if (includeProperties != null)
			{
				foreach (var property in includeProperties.Split(new char[','], StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);
				}
			}
			return await query.ToListAsync();
        }

        public async Task Remove(T Entity)
        {
            dbSet.Remove(Entity);
            await Save();
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

    }
}
