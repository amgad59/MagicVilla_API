using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI.Data
{
	public class VillaStore
	{
		public static List<VillaDTO> villaList = new List<VillaDTO>
		{
			new VillaDTO { Id = 1,Name="amgad", Occupancy=4, Sqft = 500},
			new VillaDTO { Id = 2,Name="Abd el basset", Occupancy = 3, Sqft = 400}
		};
	}
}
