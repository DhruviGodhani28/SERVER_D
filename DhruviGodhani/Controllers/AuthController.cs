using ExpenseManagement.Data;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static User;

namespace EmployeeLeaveManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ExpenseDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ExpenseDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(User u)
        {
            _context.users.Add(u);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User Register successfully", data = u });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(Login model)
        {
            var user = await _context.users.FirstOrDefaultAsync(e => e.email == model.email && e.password == model.password);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid email or password" });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { message = "Login successful", token, user });
        }

        #region Private method
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["ConnectionStrings:TokenSecret"].ToString());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                     new Claim(ClaimTypes.Name, user.id.ToString()), // Corrected from Employee.id to user.EmployeeId
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Audience = _configuration["ConnectionStrings:Audience"],
                Issuer = _configuration["ConnectionStrings:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion
    }
}