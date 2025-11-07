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
            var path = string.IsNullOrWhiteSpace(query) ? "api/Employees" : $"api/Employees?q={Uri.EscapeDataString(query)}";
            try
            {
                var httpResponse = await _httpClient.GetAsync(path);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    var body = await httpResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"GetEmployees failed: {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase} - {body}");
                    return new List<EmployeeViewModel>();
                }
                var employees = await httpResponse.Content.ReadFromJsonAsync<List<EmployeeViewModel>>();
                return employees ?? new List<EmployeeViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetEmployees exception: {ex.Message}");
                return new List<EmployeeViewModel>();
            }
        }

        public async Task<List<PositionTreeViewModel>> GetPositionsHierarchyAsync()
        {
            AttachBearer();
            try
            {
                var httpResponse = await _httpClient.GetAsync("api/Positions/hierarchy");
                if (!httpResponse.IsSuccessStatusCode)
                {
                    var body = await httpResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"GetPositionsHierarchy failed: {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase} - {body}");
                    return new List<PositionTreeViewModel>();
                }

                var nodes = await httpResponse.Content.ReadFromJsonAsync<List<PositionTreeViewModel>>();
                return nodes ?? new List<PositionTreeViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetPositionsHierarchy exception: {ex.Message}");
                return new List<PositionTreeViewModel>();
            }
        }

        public async Task<EmployeeViewModel?> GetEmployeeAsync(int id)
        {
            AttachBearer();
            return await _httpClient.GetFromJsonAsync<EmployeeViewModel>($"api/Employees/{id}");
        }

        public async Task<List<PositionViewModel>> GetPositionsAsync()
        {
            AttachBearer();
            try
            {
                var httpResponse = await _httpClient.GetAsync("api/Positions");
                if (!httpResponse.IsSuccessStatusCode)
                {
                    var body = await httpResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"GetPositions failed: {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase} - {body}");
                    return new List<PositionViewModel>();
                }

                var positions = await httpResponse.Content.ReadFromJsonAsync<List<PositionViewModel>>();
                return positions ?? new List<PositionViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetPositions exception: {ex.Message}");
                return new List<PositionViewModel>();
            }
        }

        public async Task<PositionViewModel?> GetPositionAsync(int id)
        {
            AttachBearer();
            try
            {
                return await _httpClient.GetFromJsonAsync<PositionViewModel>($"api/Positions/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetPosition exception: {ex.Message}");
                return null;
            }
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
 
        public async Task<ApiResponse> CreatePositionAsync(PositionViewModel model)
        {
            AttachBearer();
            var response = await _httpClient.PostAsJsonAsync("api/Positions/Create", model);
            if (!response.IsSuccessStatusCode)
            {
                var err = await SafeRead<ApiResponse>(response);
                return err ?? new ApiResponse { Success = false };
            }
            return new ApiResponse { Success = true };
        }

        public async Task<ApiResponse> UpdatePositionAsync(int id, PositionViewModel model)
        {
            AttachBearer();
            var response = await _httpClient.PutAsJsonAsync($"api/Positions/{id}", model);
            if (!response.IsSuccessStatusCode)
            {
                var err = await SafeRead<ApiResponse>(response);
                return err ?? new ApiResponse { Success = false };
            }
            return new ApiResponse { Success = true };
        }

        public async Task<bool> DeletePositionAsync(int id)
        {
            AttachBearer();
            var response = await _httpClient.DeleteAsync($"api/Positions/{id}");
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
