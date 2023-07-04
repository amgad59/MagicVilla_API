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

        public Task<T> CreateAsync<T>(VillaNumberCreateDTO Entity)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/VillaNumberAPI",
                ApiType = ApiType.POST,
                data = Entity
            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/VillaNumberAPI",
                ApiType = ApiType.GET
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/VillaNumberAPI/" + id,
                ApiType = ApiType.GET
            });
        }

        public Task<T> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/VillaNumberAPI/" + id,
                ApiType = ApiType.DELETE
            });
        }

        public Task<T> UpdateAsync<T>(VillaNumberUpdateDTO Entity)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/VillaNumberAPI/" + Entity.VillaNo,
                ApiType = ApiType.PUT,
                data = Entity
            });
        }
    }
}
