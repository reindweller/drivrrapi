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
    public class UserController : Controller
    {
        private IConfiguration _config;
        private readonly DrivrrContext _context;

        public UserController(IConfiguration config, DrivrrContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpGet, Authorize]
        public IEnumerable<UserModel> GetAll()
        {
            return _context.Users.Select(o => new UserModel
            {
                Id = o.Id,
                Username = o.Username,
                Password = o.Password,
                Email = o.Email
            });
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult GetById(Guid id)
        {
            var item = _context.Users.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(new UserModel
            {
                Id = item.Id,
                Username = item.Username,
                Password = item.Password,
                Email = item.Email
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] UserModel item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            var itemToInsert = new User
            {
                Username = item.Username,
                Email = item.Email
            };
            var salt = HashHelper.GenerateSalt();
            var password = HashHelper.HashPassword(item.Password, salt);
            itemToInsert.Password = password;
            itemToInsert.Salt = salt;

            _context.Users.Add(itemToInsert);
            _context.SaveChanges();

            return CreatedAtRoute("GetUser", new { id = item.Id }, item);
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] UserModel item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var user = _context.Users.FirstOrDefault(t => t.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var salt = HashHelper.GenerateSalt();
            var password = HashHelper.HashPassword(item.Password, salt);

            //State.Id = item.Id;
            user.Username = item.Username;
            user.Password = password;
            user.Email = item.Email;
            user.Salt = salt;


            _context.Users.Update(user);
            _context.SaveChanges();
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var State = _context.Users.FirstOrDefault(t => t.Id == id);
            if (State == null)
            {
                return NotFound();
            }


            _context.Users.Remove(State);
            _context.SaveChanges();
            return Ok();
        }


    }
}