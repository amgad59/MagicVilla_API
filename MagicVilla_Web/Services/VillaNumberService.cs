using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Utilities;
using static Utilities.SD;

namespace MagicVilla_Web.Services
{
    public class VillaNumberService : BaseService, IVillaNumberService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string villaUrl;
        public VillaNumberService(IHttpClientFactory httpClient,IConfiguration configuration) : base(httpClient)
        {
            _clientFactory = httpClient;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaApi");
        }

        public Task<T> CreateAsync<T>(VillaNumberCreateDTO Entity, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/v1/VillaNumberAPI",
                ApiType = ApiType.POST,
                data = Entity,
                Token = token
            });
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/v1/VillaNumberAPI",
                ApiType = ApiType.GET,
                Token = token
            });
        }

        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/v1/VillaNumberAPI/" + id,
                ApiType = ApiType.GET,
                Token = token
            });
        }

        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/v1/VillaNumberAPI/" + id,
                ApiType = ApiType.DELETE,
                Token = token
            });
        }

        public Task<T> UpdateAsync<T>(VillaNumberUpdateDTO Entity, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/v1/VillaNumberAPI/" + Entity.VillaNo,
                ApiType = ApiType.PUT,
                data = Entity,
                Token = token
            });
        }
    }
}
