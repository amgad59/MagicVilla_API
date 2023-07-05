using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Utilities;
using static Utilities.SD;

namespace MagicVilla_Web.Services
{
    public class VillaService : BaseService, IVillaService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string villaUrl;
        public VillaService(IHttpClientFactory httpClient,IConfiguration configuration) : base(httpClient)
        {
            _clientFactory = httpClient;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaApi");
        }

        public Task<T> CreateAsync<T>(VillaCreateDTO Entity, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/villaApi",
                ApiType = ApiType.POST,
                data = Entity,
                Token = token
            });
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/villaApi",
                ApiType = ApiType.GET,
                Token = token
            });
        }

        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/villaApi/" + id,
                ApiType = ApiType.GET,
                Token = token
            });
        }

        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/villaApi/" + id,
                ApiType = ApiType.DELETE,
                Token = token
            });
        }

        public Task<T> UpdateAsync<T>(VillaUpdateDTO Entity, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                Url = villaUrl + "api/villaApi/" + Entity.Id,
                ApiType = ApiType.PUT,
                data = Entity,
                Token = token
            });
        }
    }
}
