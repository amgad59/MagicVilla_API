﻿using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System.Text;
using static Utilities.SD;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }
        public BaseService(IHttpClientFactory httpClient)
        {
            responseModel = new ();
            this.httpClient = httpClient;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                if (apiRequest.data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.data),
                        Encoding.UTF8, "application/json");
                }
                switch (apiRequest.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post; 
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put; 
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete; 
                        break;
                    default:
                        message.Method = HttpMethod.Get; 
                        break;
                }
                HttpResponseMessage apiResponse = null;
                apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                try
				{
					APIResponse ApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                    if(apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound || 
                        apiResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
						ApiResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
						ApiResponse.isSuccess = false;
                        var res = JsonConvert.SerializeObject(ApiResponse);
                        var returnObj = JsonConvert.DeserializeObject<T> (res);
                        return returnObj;
                    }
				}
				catch (Exception e)
                {
                var exceptionResponse = JsonConvert.DeserializeObject<T>(apiContent);
                    return exceptionResponse;
				}
				var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
				return APIResponse;

            }
            catch (Exception ex)
            {
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
                    isSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T> (res);
                return APIResponse;
            }
        }
    }
}
