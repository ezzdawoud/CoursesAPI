using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
    public class lessonsController : ControllerBase
    {
        private readonly UserManager<Users> _userManeger;
        private readonly Connections _context;
        private readonly GenarateToken _tokenGenerator;
        private readonly Cloudinary _cloudinary;
        public lessonsController(UserManager<Users> userManeger, Connections context, GenarateToken tokenGenerator, Cloudinary cloudinary)
        {
            _userManeger = userManeger;
            _context = context;
            _tokenGenerator = tokenGenerator;
            _cloudinary = cloudinary;
        }

        [HttpPost("insert lessons/{token}/{userId}")]
        public async Task<ActionResult<string>> insetLessons(string token, string userId, Lessons lessons)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return NotFound();
            }
            else if (user.Id == userId)
            {
                var course = _context.Courses.Where(m => m.courseId == lessons.courseId).FirstOrDefault();
                if (course == null)
                {
                    return BadRequest();
                }
                if (user.Id == course.UsersId)
                {
                    var newLessons = new Lessons()
                    {
                        LessonsName = lessons.LessonsName,
                        LessonsDescription = lessons.LessonsDescription,
                        courseId = lessons.courseId
                    };
                    await _context.Lessons.AddAsync(newLessons);
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("upload video")]
        public async Task<IActionResult> UploadVideo(int courseId, int lessonsId, string id, string token, IFormFile file)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return NotFound();
            }
            if(user.Id != id)
            {
                return BadRequest();
            }
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return BadRequest("Invalid course ID.");
            }

            var lessons = await _context.Lessons.FindAsync(lessonsId);
            if (lessons == null)
            {
                return BadRequest("Invalid lesson ID.");
            }

            if (user.Id != id || user.Id != course.UsersId)
            {
                return BadRequest("User does not have permission to upload video.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };

            try
            {
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                // Update the URL of the lesson with the public ID of the uploaded video
                lessons.URL = uploadResult.PublicId;

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return the URL of the uploaded video
                return Ok(uploadResult.SecureUri);
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the upload process
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("getlessons/{courseId}/{lessonsNumber}/{id}/{token}")]
        public async Task<IActionResult> GetLessons(int courseId, int lessonsNumber, string id, string token)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Id != id)
            {
                return BadRequest();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.lessonsNum == lessonsNumber && m.courseId==courseId);

            if (lesson == null)
            {
                return NotFound();
            }

            // Fetch teacher name
            var course = await _context.Courses.FirstOrDefaultAsync(m => m.courseId == courseId);
            if (course == null)
            {
                return BadRequest();
            }
            var teacherInfo = await _context.Users
     .Where(u => u.Id == course.UsersId)
     .Select(u => new
     {
         UserName = u.UserName,
         Picture = u.UsersPictrues
     })
     .FirstOrDefaultAsync();

            if (teacherInfo != null)
            {
                var teacherName = teacherInfo.UserName;
                var teacherPicture = teacherInfo.Picture;
            }
            var AllLessons=_context.Lessons.Where(m=>m.courseId==courseId).ToList();
            // Fetch comments specific to the lesson
            var comments = await _context.Comments
    .Where(c => c.LessonsId == lesson.LessonsId)
    .Join(
        _context.Users,
        comment => comment.UsersId,
        user => user.Id,
        (comment, user) => new
        {
            UserName = user.UserName,
            Picture = user.UsersPictrues,
            Comment = new
            {
                comment.CommentsId,
                comment.comments,
                comment.date,
                comment.like,
                comment.dislike,
                SubComments = _context.SubComments
                    .Where(sc => sc.CommentsId == comment.CommentsId)
                    .Select(sc => new
                    {
                        sc.SubCommentsId,
                        sc.subComments,
                        sc.like, 
                        sc.dislike,
                        username=_context.Users.Where(m=>m.Id==sc.userId).FirstOrDefault()!.UserName,
                        picutres= _context.Users.Where(m => m.Id == sc.userId).FirstOrDefault()!.UsersPictrues
                    })
                    .ToList()
            }
        })
    .ToListAsync();







            // Construct the response object
            var response = new
            {
                Lesson = new
                {
                    lesson.LessonsId,
                    lesson.LessonsName,
                    lesson.LessonsDescription,
                    lesson.courseId,
                    lesson.URL,
                    lesson.lessonsNum
                },
                courseLessons= AllLessons,
                teacherInfoApi = teacherInfo,
                Comments = comments
            };

            return Ok(response);
        }
    
     public class userData { 
        public string userName {  get; set; }
        public string pictrue { get; set; } 
    }



        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<lessonsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<lessonsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<lessonsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<lessonsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
