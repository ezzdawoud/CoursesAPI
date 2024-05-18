using Courses.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

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
        public DbSet<SubComments> SubComments { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Rating> Rating { get; set; }

        public int GetEnrollmentCountForTeacher(string teacherId)
        {
            SqlParameter teacherIdParam = new SqlParameter("@TeacherId", teacherId);
            SqlParameter enrollmentCountParam = new SqlParameter("@EnrollmentCount", SqlDbType.Int);
            enrollmentCountParam.Direction = ParameterDirection.Output;

            this.Database.ExecuteSqlRaw("EXECUTE GetEnrollmentCountForTeacher @TeacherId, @EnrollmentCount OUTPUT", teacherIdParam, enrollmentCountParam);

            return (int)enrollmentCountParam.Value;
        }
        public int CalculateTotalCourseValueForTeacher(string teacherId)
        {
            SqlParameter teacherIdParam = new SqlParameter("@teacher_id", teacherId);
            SqlParameter totalValueParam = new SqlParameter("@total_value", SqlDbType.Int);
            totalValueParam.Direction = ParameterDirection.Output;

            this.Database.ExecuteSqlRaw("EXECUTE CalculateTotalCourseValueForTeacher @teacher_id, @total_value OUTPUT", teacherIdParam, totalValueParam);

            return (int)totalValueParam.Value;
        }
        public Connections(DbContextOptions<Connections> options)
       : base(options)
        {
        }


    }
}
