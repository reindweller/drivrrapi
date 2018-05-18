using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drivrr.Api.Model
{
    public class DrivrrContext : DbContext
    {
        public DrivrrContext(DbContextOptions<DrivrrContext> options)
            : base(options)
        {
        }
        
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
