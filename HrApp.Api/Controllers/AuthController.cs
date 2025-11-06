using HrApp.Core.DTOs;
using HrApp.Core.Entities;
using HrApp.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "მომხმარებელი უკვე არსებობს !"
                });
            }


            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PersonalNumber = dto.PersonalNumber,
                Email = dto.Email,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender
            };
            await _userRepository.RegisterUserAsync(user,dto.Password);
            return Ok(new {Success= true});
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUserDto dto)
        {
            var user = await _userRepository.LoginUserAsync(dto.Email, dto.Password);
            if (user == null) return BadRequest(new { Success = false, Message = "მომხმარებელი ან პაროლი არასწორია" });

            return Ok(new { Success = true });
        }

    }
}
