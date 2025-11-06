using HrApp.Core.DTOs;
using HrApp.Core.Entities;
using HrApp.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HrApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public AuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }


        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            {
                return BadRequest(new { Success = false, Message = "აუცილებელი ველები ცარიელია" });
            }
            if (dto.PersonalNumber == null || dto.PersonalNumber.Length != 11 || !dto.PersonalNumber.All(char.IsDigit))
            {
                return BadRequest(new { Success = false, Message = "პირადი ნომერი უნდა იყოს 11 ციფრი" });
            }
            try
            {
                var mail = new System.Net.Mail.MailAddress(dto.Email);
            }
            catch
            {
                return BadRequest(new { Success = false, Message = "ელ.ფოსტის ფორმატი არასწორია" });
            }
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "ასეთი მომხმარებელი უკვე არსებობს, გთხოვთ შეხვიდეთ სისტემაში"
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
            var token = GenerateToken(user);
            return Ok(new { Success = true, Token = token });
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUserDto dto)
        {
            var user = await _userRepository.LoginUserAsync(dto.Email, dto.Password);
            if (user == null) return BadRequest(new { Success = false, Message = "მომხმარებელი ან პაროლი არასწორია" });

            var token = GenerateToken(user);
            return Ok(new { Success = true, Token = token });
        }

        private string GenerateToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, ($"{user.FirstName} {user.LastName}").Trim())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
