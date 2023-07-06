using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Controllers
{
	[ApiController]
	[Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiVersion("1.0")]

    public class VillaAPIController : ControllerBase
	{
		private readonly IVillaRepository _dbVilla;
		private readonly IMapper _mapper;
		protected APIResponse _response = new APIResponse();


        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
		{
            _dbVilla = dbVilla;
            _mapper = mapper;
        }

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ResponseCache(CacheProfileName = "Default30")]
		public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name ="FilterOccupancy")] int? occupancy
			,string? search, int pageSize = 0, int pageNumber = 1) {
			try {
				IEnumerable<Villa> villas;
				if(occupancy > 0)
				{
					villas = await _dbVilla.GetAll(u=>u.Occupancy == occupancy, pageNumber:pageNumber, pageSize:pageSize);
                }
				else
				{
					villas = await _dbVilla.GetAll(pageNumber: pageNumber, pageSize: pageSize);
                }
				if (!string.IsNullOrEmpty(search))
				{
					villas = villas.Where(u => u.Name.ToLower().Contains(search));
				}
				Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize};
				
				Response.Headers.Add("X-Pagination",JsonSerializer.Serialize(pagination));
				_response.Result = _mapper.Map<List<VillaDTO>>(villas);
				_response.StatusCode = HttpStatusCode.OK;
				return Ok(_response);
				}
			catch (Exception ex)
			{
				_response.isSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
			}
			return _response;
		}


		[HttpGet("{id:int}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> GetVilla(int id) {
			try { 
			if (id == 0)
			{
				return BadRequest();
			}
			var villa = await _dbVilla.Get(u => u.Id == id);
			if (villa == null)
				return NotFound(); 
			_response.Result = _mapper.Map<VillaDTO>(villa);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }



        [Authorize(Roles = "admin")]
        [HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> createVilla([FromBody] VillaCreateDTO villa)
		{
			try { 
			if (await _dbVilla.Get(u => u.Name.ToLower() == villa.Name.ToLower()) != null)
			{
				ModelState.AddModelError("ErrorMessages", "Villa name already exists");
				return BadRequest(ModelState);
			}
			if (villa == null)
			{
				BadRequest();
			}
			Villa model = _mapper.Map<Villa>(villa);
			await _dbVilla.CreateVilla(model); 
			_response.Result = _mapper.Map<VillaDTO>(model);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetVilla", new { model.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}",Name ="DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
		{
			try { 
			if(id == 0) return BadRequest();
			var villa = await _dbVilla.Get(u => u.Id == id);
			if (villa == null) return NotFound();

            await _dbVilla.Remove(villa);
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }


        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}",Name ="UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> UpdateVilla(int id,[FromBody] VillaUpdateDTO villa)
		{
			try { 
			if(villa == null || id != villa.Id) return BadRequest();

			Villa model = _mapper.Map<Villa>(villa);

            await _dbVilla.Update(model);

            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }


        [Authorize(Roles = "admin")]
        [HttpPatch("{id:int}",Name ="UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
		{
			if (patchDTO == null || id == 0) return BadRequest();
			var villa = await _dbVilla.Get(u => u.Id == id,isTracked:false);
			if (villa == null) return BadRequest();

			VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);
            if (villa == null) return BadRequest();

			patchDTO.ApplyTo(villaDTO, ModelState);
			Villa model = _mapper.Map<Villa>(villaDTO);
            await _dbVilla.Update(model);

			if (!ModelState.IsValid) return BadRequest(ModelState);
			return NoContent();

		}


	}
}
