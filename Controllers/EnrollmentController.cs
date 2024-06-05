using Courses.Data;
using Courses.Helper;
using Courses.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Courses.Controllers.EnrollmentController;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly UserManager<Users> _userManeger;
        private readonly Connections _context;
        private readonly IConfiguration _configuration;
        private readonly GenarateToken _tokenGenerator;
        public EnrollmentController(Connections conections, UserManager<Users> userManeger, IConfiguration configuration, GenarateToken tokenGenerator)
        {
            _context = conections;
            _userManeger = userManeger;
            _configuration = configuration;
            _tokenGenerator = tokenGenerator;


        }

        // POST api/<EnrollmentController>
        public class enrollment
        {
            public string token { get; set; }
            public string id { get; set; }
            public int courseId {  get; set; }
            public string cardNumber {  get; set; }
            public DateTime ?EnrollmentDate { get; set; } 

        }

        [HttpPost("Enrollment")]
        public async Task<IActionResult> Post([FromBody]enrollment enrollment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Users user = await _tokenGenerator.GetUserFromToken(enrollment.token);
            if (user == null)
            {
                return NotFound("User Not Found");
            }
            else if (user.Id == enrollment.id)
            {
                Cards Card = _context.Cards.FirstOrDefault(m => m.CardNumber == enrollment.cardNumber);
                Models.Courses course = _context.Courses.FirstOrDefault(m => m.courseId == enrollment.courseId);

                if (Card == null)
                {
                    return NotFound("Card Not Found");
                }
                if (course == null)
                {
                    return NotFound("Course Not Found");
                }
                if (Card.CardValue >= course.CouresValue)
                {
                    Card.CardValue -= course.CouresValue;
                    Card.userId = user.Id;
                    // Save the changes to the database
                    _context.SaveChanges();

                    var newEnrollment = new Enrollment { CouresId = course.courseId, UserId = user.Id, enrollmentValue = course.CouresValue,teacherId= course.UsersId,EnrollmentDate= DateTime.Now };
                    _context.Enrollments.Add(newEnrollment);
                    _context.SaveChanges();

                    return Ok(new { message = "Course Buy Successfully" });

                }
                else
                {
                    return BadRequest("Card Value");
                }
            }
            else
            {
                return BadRequest("You don't have access");
            }
        }


        [HttpPost("get user Courses")]
        public async Task<IActionResult> Post(string token, string id)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (id != user.Id)
            {
                return BadRequest("user not Found");
            }
            List<Enrollment> enrollments = await _context.Enrollments
       .Where(m => m.UserId == user.Id)
       .ToListAsync();

            List<Models.Courses> courses = new List<Models.Courses>(); // Create a list to store courses

            foreach (var enrollment in enrollments)
            {
                Models.Courses course = await _context.Courses
                    .FirstOrDefaultAsync(m => m.courseId == enrollment.CouresId);

                if (course != null)
                {
                    courses.Add(course); // Add the course to the list
                }
            }
            return Ok(courses);


        }



    }
}
