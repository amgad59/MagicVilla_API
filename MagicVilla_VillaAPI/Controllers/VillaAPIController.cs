


using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
	[ApiController]
	[Route("api/VillaAPI")]
	public class VillaAPIController : ControllerBase
	{
		private readonly ApplicationDbContext _db;

		public VillaAPIController(ApplicationDbContext db)
		{
			_db = db;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas() {
			return Ok(_db.villas);
		}



		[HttpGet("{id:int}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<VillaDTO> GetVilla(int id) {
			if (id == 0)
			{
				return BadRequest();
			}
			var villa = _db.villas.FirstOrDefault(u => u.Id == id);
			if (villa == null)
				return NotFound();
			return Ok(villa);
		}

		

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<VillaDTO> createVilla([FromBody] VillaDTO villa)
		{
			if (_db.villas.FirstOrDefault(u => u.Name.ToLower() == villa.Name.ToLower()) != null)
			{
				ModelState.AddModelError("Duplicate Record", "Villa name already exists");
				return BadRequest(ModelState);
			}
			if (villa == null)
			{
				BadRequest();
			}
			if (villa.Id > 0)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			Villa model = ConvertToVilla(villa);
			_db.villas.Add(model);
			_db.SaveChanges();
			return CreatedAtRoute("GetVilla", new { villa.Id }, villa);
		}


		[HttpDelete("{id:int}",Name ="DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult DeleteVilla(int id)
		{
			if(id == 0) return BadRequest();
			var villa = _db.villas.FirstOrDefault(u => u.Id == id);
			if (villa == null) return NotFound();

			_db.villas.Remove(villa);
			_db.SaveChanges();
			return NoContent();
		}


		[HttpPut("{id:int}",Name ="UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult UpdateVilla(int id,[FromBody] VillaDTO villaDTO)
		{
			if(villaDTO == null || id != villaDTO.Id) return BadRequest();

			Villa villa = ConvertToVilla(villaDTO);

			_db.villas.Update(villa);
			_db.SaveChanges();

			return NoContent();
		}


		[HttpPatch("{id:int}",Name ="UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
		{
			if (patchDTO == null || id == 0) return BadRequest();
			var villa = _db.villas.AsNoTracking().FirstOrDefault(u => u.Id == id);
			if (villa == null) return BadRequest();

			VillaDTO villaDTO = ConvertToVillaDTO(villa);
			if (villa == null) return BadRequest();

			patchDTO.ApplyTo(villaDTO, ModelState);
			Villa model = ConvertToVilla(villaDTO);
			_db.villas.Update(model);
			_db.SaveChanges();

			if (!ModelState.IsValid) return BadRequest(ModelState);
			return NoContent();

		}


		public static Villa ConvertToVilla(VillaDTO villa)
		{
			return new Villa()
			{
				Id = villa.Id,
				Name = villa.Name,
				Amenity = villa.Amenity,
				Details = villa.Details,
				ImageUrl = villa.ImageUrl,
				Occupancy = villa.Occupancy,
				Rate = villa.Rate,
				Sqft = villa.Sqft,

			};
		}
		public static VillaDTO ConvertToVillaDTO(Villa villa)
		{
			return new VillaDTO()
			{
				Id = villa.Id,
				Name = villa.Name,
				Amenity = villa.Amenity,
				Details = villa.Details,
				ImageUrl = villa.ImageUrl,
				Occupancy = villa.Occupancy,
				Rate = villa.Rate,
				Sqft = villa.Sqft,

			};
		}
	}
}
