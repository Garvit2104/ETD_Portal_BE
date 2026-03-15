using Reimbursement__Managment.DTOs.TPClinet_DTO;
using System.Net.Http;
using System.Text.Json;

namespace Reimbursement__Managment.BLL.ClientServices
{
    public class TPClientServiceClass
    {
        private readonly HttpClient _httpClient;

        public TPClientServiceClass(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("TPClient");
        }

        public async Task<TpResponseDTO> GetTravellingDates(int trId)
        {
            var response = await _httpClient.GetAsync($"api/TravelPlanner/{trId}");

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TpResponseDTO>();

            return result;
        }

        public async Task<TpResponseDTO> GetTravelRequestById(int id)
        {
          
            var response = await _httpClient.GetAsync($"api/TravelPlanner/{id}");

            var travelRequest = await response.Content.ReadFromJsonAsync<TpResponseDTO>();
            return travelRequest;

        }
    }
    
}
