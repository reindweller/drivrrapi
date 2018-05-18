using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drivrr.Api.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public byte[] Salt { get; set; }
    }

    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
