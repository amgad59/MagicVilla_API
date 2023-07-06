using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiVersion("1.0")]
    public class VillaNumberAPIController : Controller
    {
        private readonly IVillaNumberRepository _dbVillaNumbers;
        private readonly IVillaRepository _dbVillas;
        private readonly IMapper _mapper;
        protected APIResponse _response = new APIResponse();

        public VillaNumberAPIController(IVillaNumberRepository dbVillaNumbers, IVillaRepository dbVillas, IMapper mapper)
        {
            _dbVillaNumbers = dbVillaNumbers;
            _dbVillas = dbVillas;
            _mapper = mapper;
        }

        [HttpGet("GetString")]
        public IEnumerable<string> Get()
        {
            return new string[]
            {
                "string1","string2"
            };
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumbers = await _dbVillaNumbers.GetAll(includeProperties:"Villa");
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbers);
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


        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villaNumber = await _dbVillaNumbers.Get(u => u.VillaNo == id, includeProperties: "Villa");
                if (villaNumber == null)
                    return NotFound();
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
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
        public async Task<ActionResult<APIResponse>> createVillaNumber([FromBody] VillaNumberCreateDTO villa)
        {
            try
            {
                if (await _dbVillaNumbers.Get(u => u.VillaNo == villa.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa number already exists");
                    return BadRequest(ModelState);
                }
                if (await _dbVillas.Get(u => u.Id == villa.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa doesn't exist");
                    return BadRequest(ModelState);
                }
                if (villa == null)
                {
                    BadRequest();
                }
                VillaNumber model = _mapper.Map<VillaNumber>(villa);
                await _dbVillaNumbers.CreateVilla(model);
                _response.Result = _mapper.Map<VillaNumberDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVillaNumber", new { id = model.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }


        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0) return BadRequest();
                var villa = await _dbVillaNumbers.Get(u => u.VillaNo == id);
                if (villa == null) return NotFound();

                await _dbVillaNumbers.Remove(villa);
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
        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO villa)
        {
            try
            {
                if (villa == null || id != villa.VillaNo) return BadRequest();

                if (await _dbVillas.Get(u => u.Id == id) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "VillaID is invalid");
                    return BadRequest(ModelState);
                }
				if (await _dbVillas.Get(u => u.Id == villa.VillaID) != null)
				{
					ModelState.AddModelError("ErrorMessages", "Villa doesn't exist");
					return BadRequest(ModelState);
				}
				VillaNumber model = _mapper.Map<VillaNumber>(villa);

                await _dbVillaNumbers.Update(model);

                _response.StatusCode = HttpStatusCode.OK;
                _response.isSuccess = true;
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
        [HttpPatch("{id:int}", Name = "UpdatePartialVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVillaNumber(int id, JsonPatchDocument<VillaNumberUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0) return BadRequest();
            var villa = await _dbVillaNumbers.Get(u => u.VillaNo == id, isTracked: false);
            if (villa == null) return BadRequest();

            VillaNumberUpdateDTO villaDTO = _mapper.Map<VillaNumberUpdateDTO>(villa);
            if (villa == null) return BadRequest();

            patchDTO.ApplyTo(villaDTO, ModelState);
            VillaNumber model = _mapper.Map<VillaNumber>(villaDTO);
            await _dbVillaNumbers.Update(model);

            if (!ModelState.IsValid) return BadRequest(ModelState);
            return NoContent();

        }
    }
}
