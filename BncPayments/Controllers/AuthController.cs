using BncPayments.Models;
using BncPayments.Repositories;
using BncPayments.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace BncPayments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAppRepository _appRepository;

        public AuthController(IConfiguration config,
            IAppRepository appRepository)
        {
            _config = config;
            _appRepository = appRepository;
        }

        [HttpGet("Users")]
        public async Task<IActionResult> Users()
        {
            var result = await _appRepository.GetApps();
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM login)
        {
            var result = await _appRepository.ExistsApp(login);
            if (result != null)
            {
                var token = GenerateJwtToken(login.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized("No existe el usuario!");
        }
        
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] AppVM model)
        {
            try
            {
                model.Id = Guid.NewGuid();

                var createModel = await _appRepository.Create(new Application()
                {
                    IdApplication = model.Id.ToString(),
                    Name = model.Nombre,
                    Password = model.Clave // encriptar
                });

                return Ok(createModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, username)
            }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
