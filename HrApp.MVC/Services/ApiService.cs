using System.Net.Http.Json;
using HrApp.MVC.Models;


namespace HrApp.MVC.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse> RegisterUserAsync(RegisterViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/Register", model);
            return await response.Content.ReadFromJsonAsync<ApiResponse>() ?? new ApiResponse { Success = false };
        }

        public async Task<ApiResponse> LoginUserAsync(LoginViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", model);
            return await response.Content.ReadFromJsonAsync<ApiResponse>() ?? new ApiResponse { Success = false };
        }

        public async Task<List<EmployeeViewModel>> GetEmployeesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<EmployeeViewModel>>("api/Employees");
            return response ?? new List<EmployeeViewModel>();
        }
    }
}
