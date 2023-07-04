using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _db;

        public VillaNumberRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<VillaNumber> Update(VillaNumber VillaNumber)
        {
            VillaNumber.UpdatedDate = DateTime.Now;
            _db.villaNumbers.Update(VillaNumber);
            await _db.SaveChangesAsync();
            return VillaNumber;
        }
    }
}
