using Courses.Data;
using Courses.Helper;
using Courses.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using static Courses.Controllers.coursesController;

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
        public class TeacherUserData
        {
            public string id { get; set; }
            public string token { get; set; }
        }

        [HttpPost("get Teacher Data")]
        public async Task<IActionResult> Post([FromBody] TeacherUserData userData)
        {
            Users user = await _tokenGenerator.GetUserFromToken(userData.token);
            if (user == null)
            {
                return NotFound("user Not Found !");
            }
            if(user.Id != userData.id)
            {
                return BadRequest("you dont have access");
            }

var count = _context.GetEnrollmentCountForTeacher(user.Id);
            var valud = _context.CalculateTotalCourseValueForTeacher(user.Id);

            var courseCount = _context.Courses
                .Where(m => m.UsersId == userData.id)
                .Count();
            var totalValue = _context.Enrollments
              .Where(e => e.UserId == userData.id) // Filter by UserId
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

        public class teacherProfile
        {
            public string userId { get; set; }
            public string token {  get; set; }
            public string teacherName {  get; set; }
        }

        public class teacherCourseDetails
        {
            public int? CourseId { get; set; }
            public string CourseName { get; set; }
            public int NumberOfStudents { get; set; }
            public string TeacherName { get; set; }
            public string TeacherPicture { get; set; }
            public string Picture { get; set; }
            public int CouresValue { get; set; }
            public string CouresDescription { get; set; }
            public string CouresType { get; set; }
            public string CoursesCatagory { get; set; }
            public string CouresLanguage { get; set; }
            public DateTime Date { get; set; }
            public int Rating { get; set; }
            public string token { get; set; }
            public string id { get; set; }




        }

        [HttpPost("get teacher profile")]
        public async Task<IActionResult> getTeacherProfile([FromBody] teacherProfile userData)
        {
            Users user = await _tokenGenerator.GetUserFromToken(userData.token);
            if (user == null)
            {
                return NotFound("user Not Found !");
            }
            if (user.Id != userData.userId)
            {
                return BadRequest("you dont have access");
            }
            var teacher = _context.Users.Where(m=>m.UserName==userData.teacherName).FirstOrDefault();
            if (teacher == null)
            {
                return BadRequest();
            }
           
            var teacherRole =await _userManeger.GetRolesAsync(teacher);
            if (teacherRole.FirstOrDefault() != "teacher") {
                return BadRequest("");
            }
            else
            {
                var userId = teacher.Id;

              
                var courses = await _context.Courses
    .Where(c => c.UsersId == teacher.Id)
    .Select(c => new teacherCourseDetails
    {
        CourseId = c.courseId,
        CourseName = c.CouresName,
        NumberOfStudents = _context.Enrollments.Count(e => e.CouresId == c.courseId),
        TeacherName = _context.Users.Where(u => u.Id == c.UsersId).FirstOrDefault()!.UserName,
        Picture = c.Pictures,
        CouresValue = c.CouresValue,
        CouresDescription = c.CouresDescription,
        CouresType = c.CouresType,
        CoursesCatagory = c.CoursesCatagory,
        CouresLanguage = c.CouresLanguage,
        Date = c.Date,
        Rating = c.Rating,
        TeacherPicture = _context.Users.Where(u => u.Id == c.UsersId).FirstOrDefault()!.UsersPictrues
    })
    .ToListAsync();

                // Calculate the average rating for all these courses
                var averageRating = (from course in _context.Courses
                                     join rating in _context.Rating on course.courseId equals rating.courseId
                                     where course.UsersId == userId
                                     select rating.rating)
                                    .DefaultIfEmpty() // To handle the case when there are no ratings
                                    .Average();

                return Ok(new { teacherData = teacher, Courses = courses ,reating=averageRating});
            }
        }

       


    }
}
