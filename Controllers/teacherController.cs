using Courses.Data;
using Courses.Helper;
using Courses.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class teacherController : ControllerBase
    {
        private readonly UserManager<Users> _userManeger;
        private readonly SignInManager<Users> _signInManeger;
        private readonly Connections _context;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly GenarateToken _tokenGenerator;
        public teacherController(Connections conections, UserManager<Users> userManeger, SignInManager<Users> signInManeger, IConfiguration configuration, IEmailSender emailSender, GenarateToken tokenGenerator)
        {
            _context = conections;
            _userManeger = userManeger;
            _signInManeger = signInManeger;
            _configuration = configuration;
            _emailSender = emailSender;
            _tokenGenerator = tokenGenerator;


        }

        [HttpPost("get Teacher Data/{id}/{token}")]
        public async Task<IActionResult> Post(string id,string token)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return NotFound("user Not Found !");
            }
            if(user.Id != id)
            {
                return BadRequest("you dont have access");
            }

var count = _context.GetEnrollmentCountForTeacher(user.Id);
            var valud = _context.CalculateTotalCourseValueForTeacher(user.Id);

            var courseCount = _context.Courses
                .Where(m => m.UsersId == id)
                .Count();
            var totalValue = _context.Enrollments
              .Where(e => e.UserId == id) // Filter by UserId
              .GroupBy(e => e.UserId) // Group by UserId
              .Select(g => g.Sum(e => e.enrollmentValue)) // Sum of enrollmentValue for the teacher
              .FirstOrDefault(); // Retrieve the first (and only) value or default if no matching records are found


            var result = new
            {
                EnrollmentCount = count,
                TotalCourseValue = totalValue,
                CourseCount = courseCount
            };
            return Ok(result);



        }


        // PUT api/<teacherController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<teacherController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
