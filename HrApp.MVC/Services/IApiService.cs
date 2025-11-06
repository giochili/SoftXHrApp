using HrApp.MVC.Models;

namespace HrApp.MVC.Services
{
    public interface IApiService
    {
        Task<ApiResponse> RegisterUserAsync(RegisterViewModel model);
        Task<ApiResponse> LoginUserAsync(LoginViewModel model);
        Task<List<EmployeeViewModel>> GetEmployeesAsync();
    }
}
