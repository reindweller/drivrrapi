using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Drivrr.Api.Model;
using Drivrr.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Drivrr.Api.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private IConfiguration _config;
        private readonly DrivrrContext _context;

        public LoginController(IConfiguration config, DrivrrContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody]LoginModel login)
        {
            IActionResult response = Unauthorized();
            var user = Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string BuildToken(User user)
        {

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
                //,
                //new Claim(JwtRegisteredClaimNames.Birthdate, user.Birthdate.ToString("yyyy-MM-dd")),
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User Authenticate(LoginModel login)
        {
            var user = _context.Users.Where(o => o.Username == login.Username).FirstOrDefault();
            var salt = user.Salt;
            var password = HashHelper.HashPassword(login.Password, salt);
            //User user = _context.Users.Where(o => o.Username == login.Username && o.Password == HashHelper.HashPassword(login.Password)).FirstOrDefault();

            //if (login.Username == "mario" && login.Password == "secret")
            //{
            //    user = new User { Name = "Mario Rossi", Email = "mario.rossi@domain.com", Birthdate = DateTime.Now.AddYears(-15) };
            //}

            if (user.Password == password)
            {
                return user;
            }

            return null;
        }
    }
}