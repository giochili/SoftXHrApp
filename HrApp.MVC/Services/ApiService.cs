using System.Net.Http.Json;
using HrApp.MVC.Models;
using Microsoft.AspNetCore.Http;


namespace HrApp.MVC.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AttachBearer()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<ApiResponse> RegisterUserAsync(RegisterViewModel model)
        {
            AttachBearer();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/Register", model);
                if (!response.IsSuccessStatusCode)
                {
                    var err = await SafeRead<ApiResponse>(response);
                    if (err != null) return err;
                    return new ApiResponse { Success = false, Message = $"Register failed ({(int)response.StatusCode})" };
                }
                var ok = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return ok ?? new ApiResponse { Success = false, Message = "ცარიელი პასუხი სერვერიდან" };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse> LoginUserAsync(LoginViewModel model)
        {
            AttachBearer();
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", model);
                if (!response.IsSuccessStatusCode)
                {
                    var err = await SafeRead<ApiResponse>(response);
                    if (err != null) return err;
                    return new ApiResponse { Success = false, Message = $"Login failed ({(int)response.StatusCode})" };
                }
                var ok = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return ok ?? new ApiResponse { Success = false, Message = "ცარიელი პასუხი სერვერიდან" };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<List<EmployeeViewModel>> GetEmployeesAsync(string? query = null)
        {
            AttachBearer();
            var path = string.IsNullOrWhiteSpace(query) ? "api/Employees/GetAll" : $"api/Employees/GetAll?q={Uri.EscapeDataString(query)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<EmployeeViewModel>>(path);
                return response ?? new List<EmployeeViewModel>();
            }
            catch
            {
                return new List<EmployeeViewModel>();
            }
        }

        public async Task<EmployeeViewModel?> GetEmployeeAsync(int id)
        {
            AttachBearer();
            return await _httpClient.GetFromJsonAsync<EmployeeViewModel>($"api/Employees/{id}");
        }

        public async Task<ApiResponse> CreateEmployeeAsync(EmployeeViewModel model)
        {
            AttachBearer();
            var response = await _httpClient.PostAsJsonAsync("api/Employees/Create", model);
            if (!response.IsSuccessStatusCode)
            {
                var err = await SafeRead<ApiResponse>(response);
                return err ?? new ApiResponse { Success = false };
            }
            return new ApiResponse { Success = true };
        }

        public async Task<ApiResponse> UpdateEmployeeAsync(int id, EmployeeViewModel model)
        {
            AttachBearer();
            var response = await _httpClient.PutAsJsonAsync($"api/Employees/{id}", model);
            if (!response.IsSuccessStatusCode)
            {
                var err = await SafeRead<ApiResponse>(response);
                return err ?? new ApiResponse { Success = false };
            }
            return new ApiResponse { Success = true };
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            AttachBearer();
            var response = await _httpClient.DeleteAsync($"api/Employees/{id}");
            return response.IsSuccessStatusCode;
        }
 
        private static async Task<T?> SafeRead<T>(HttpResponseMessage response) where T : class
        {
            try
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch
            {
                return null;
            }
        }
    }
}
