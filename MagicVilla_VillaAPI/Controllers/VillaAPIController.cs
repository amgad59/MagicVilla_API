﻿


using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	[ApiController]
	[Route("api/VillaAPI")]
	public class VillaAPIController : ControllerBase
	{
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas() {
			return Ok(VillaStore.villaList);
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
			var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
			if (villa == null)
				return NotFound();
			return villa;
		}



		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<VillaDTO> createVilla([FromBody] VillaDTO villa)
		{
			if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villa.Name.ToLower()) != null)
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
			villa.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
			VillaStore.villaList.Add(villa);
			return CreatedAtRoute("GetVilla", new { villa.Id }, villa);
		}


		[HttpDelete("{id:int}",Name ="DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult DeleteVilla(int id)
		{
			if(id == 0) return BadRequest();
			var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
			if (villa == null) return NotFound();

			VillaStore.villaList.Remove(villa);
			return NoContent();
		}


		[HttpPut("{id:int}",Name ="UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult UpdateVilla(int id,[FromBody] VillaDTO villaDTO)
		{
			if(villaDTO == null || id != villaDTO.Id) return BadRequest();

			var villa = VillaStore.villaList.FirstOrDefault(u=>u.Id == id);
			if(villa == null) return NotFound();

			villa.Name = villaDTO.Name;
			villa.Occupancy = villaDTO.Occupancy;
			villa.Sqft = villaDTO.Sqft;
			return NoContent();
		}


		[HttpPatch("{id:int}",Name ="UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
		{
			if (patchDTO == null || id == 0) return BadRequest();
			var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
			if (villa == null) return BadRequest();


			patchDTO.ApplyTo(villa, ModelState);
			if (!ModelState.IsValid) return BadRequest(ModelState);
			return NoContent();

		}

	}
}
