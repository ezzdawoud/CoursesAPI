using Courses.Data;
using Courses.Helper;
using Courses.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class coursesController : ControllerBase
    {
        private readonly UserManager<Users> _userManeger;
        private readonly Connections _context;
        private readonly GenarateToken _tokenGenerator;
        public coursesController(UserManager<Users> userManeger, Connections context, GenarateToken tokenGenerator)
        {
            _userManeger = userManeger;
            _context = context;
            _tokenGenerator = tokenGenerator;
        }


        // GET: api/<coursesController>
        [HttpGet("get courses")]
        public async Task<ActionResult<IEnumerable<Models.Courses>>> GetCourses(string token, string id)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return NotFound("User Not Found !");
            }
            else if (user.Id == id)
            {
                var courses = await _context.Courses.Where(m => m.UsersId == id).ToListAsync();
                return Ok(courses);
            }
            else
            {
                return BadRequest("You don't have access to show this");
            }
        }
        [HttpPost("insert Course")]
        public async Task<ActionResult<string>> insetCourse(string token, string id, Models.Courses course)
        {
            try
            {
                Users user = await _tokenGenerator.GetUserFromToken(token);
                if (user == null)
                {
                    return NotFound();
                }
                else if (user.Id == id)
                {
                    if (user.Id==course.UsersId) {  
                    _context.Courses.Add(course);
                    _context.SaveChanges();
                    return Ok("Insert Successfully!");
                    }
                    else
                    {
                        return BadRequest("You cant insert course to another Teacher!");
                    }
                }
                else
                {
                    return BadRequest("You dont have accsess to insert course!");
                }
            }
            catch(Exception e)
            {
                return BadRequest("You insert something wrong!");
            }
        }

        [HttpPost("get course")]
        public async Task<ActionResult<Models.Courses>> getCourse(string token, int courseId)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
           

            var course = await _context.Courses.FirstOrDefaultAsync(m => m.courseId == courseId);
            if (course == null)
            {
                return NotFound("course Not Found !");
            }

            return course;
        }




    }
}
