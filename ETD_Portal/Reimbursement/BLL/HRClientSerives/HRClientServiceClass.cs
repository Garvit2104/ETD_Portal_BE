using System.Net.Http;
using System.Text.Json;
using Reimbursement__Managment.DTOs.HRClient_DTO;

namespace Reimbursement__Managment.BLL.HRClientSerives
{
    public class HRClientServiceClass
    {
        private readonly HttpClient _httpClient;

        public HRClientServiceClass(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HRClient");
        }

        public async Task<HrResponseDTO> ValidateGetEmployeeId(int employeeId)
        {
            var result = await _httpClient.GetAsync($"/api/employee/{employeeId}");

            var user = await result.Content.ReadFromJsonAsync<HrResponseDTO>();
            return user;
        }
    }
}
