using Drivrr.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drivrr.Api.Model
{
    public class DbInitializer
    {
        public static void Initialize(DrivrrContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var salt = HashHelper.GenerateSalt();
            var adminPassword = HashHelper.HashPassword("p@ssw0rd", salt);
            var users = new User[]
            {
                new User{Username="admin",Password=adminPassword,Email="admin@drivrr.com", Salt = salt}
            };
            foreach (User u in users)
            {
                context.Users.Add(u);
            }

            context.SaveChanges();

        }
    }
}
