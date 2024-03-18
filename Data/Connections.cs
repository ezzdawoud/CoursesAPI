using Courses.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Courses.Data
{
    public class Connections : IdentityDbContext<Users>
    {
        public new DbSet<Users> Users { get; set; } // Hide the original Users property
        public DbSet<Cards> Cards { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Models.Courses> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Lessons> Lessons { get; set; }
        public Connections(DbContextOptions<Connections> options)
       : base(options)
        {
        }


    }
}
