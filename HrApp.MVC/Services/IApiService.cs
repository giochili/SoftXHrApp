using HrApp.MVC.Models;

namespace HrApp.MVC.Services
{
    public interface IApiService
    {
        Task<ApiResponse> RegisterUserAsync(RegisterViewModel model);
        Task<ApiResponse> LoginUserAsync(LoginViewModel model);
        Task<List<EmployeeViewModel>> GetEmployeesAsync(string? query = null);
        Task<EmployeeViewModel?> GetEmployeeAsync(int id);
        Task<ApiResponse> CreateEmployeeAsync(EmployeeViewModel model);
        Task<ApiResponse> UpdateEmployeeAsync(int id, EmployeeViewModel model);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
