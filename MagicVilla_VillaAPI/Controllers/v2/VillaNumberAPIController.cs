using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiVersion("2.0")]
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


        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[]
            {
                "value1","value2"
            };
        }
    }
}
