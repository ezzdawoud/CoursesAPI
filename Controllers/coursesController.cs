using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Courses.Data;
using Courses.Helper;
using Courses.Models;
using EllipticCurve;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Courses.Controllers.coursesController;

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
        private readonly Cloudinary _cloudinary;

        public coursesController(UserManager<Users> userManeger, Connections context, GenarateToken tokenGenerator, Cloudinary cloudinary)
        {
            _userManeger = userManeger;
            _context = context;
            _tokenGenerator = tokenGenerator;
            _cloudinary = cloudinary;
        }




        // GET: api/<coursesController>
        [HttpPost("get teacher courses/{token}/{id}")]
        public async Task<ActionResult<IEnumerable<Models.Courses>>> GetCourses(string token, string id)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return NotFound("User Not Found !");
            }
            else if (user.Id == id)
            {
                var courses = await _context.Courses
     .Where(c => c.UsersId == id)
     .Select(c => new CourseDetails
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
         Rating = c.Rating
     })
     .ToListAsync();


                return Ok(courses);
            }
            else
            {
                return BadRequest("You don't have access to show this");
            }
        }
        public class CourseDetails
        {
            public int CourseId { get; set; }
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





        }

        [HttpPost("insert Course/{id}/{token}")]
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
                    if (user.Id == course.UsersId)
                    {
                        _context.Courses.Add(course);
                        _context.SaveChanges();

                        var courseId = course.courseId;

                        return Ok(new { message = "Insert Successfully!", courseId });
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
            catch (Exception e)
            {
                return BadRequest("You insert something wrong!" + e);
            }
        }

        [HttpPost("upload course Image/{courseId}/{id}/{token}")]
        public async Task<IActionResult> UploadCourseImage(int courseId, string id, string token, IFormFile file)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            var course = await _context.Courses.FindAsync(courseId);
            if (user == null)
            {
                return NotFound();
            }
            if (course == null)
            {
                return NotFound();
            }
            else if (user.Id == id)
            {

                if (user.Id == course.UsersId)
                {
                    if (file == null || file.Length == 0)
                        return BadRequest("No file uploaded.");

                    // Upload image to Cloudinary
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream())
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    // Assuming you're using Entity Framework Core, retrieve the course from the database

                    // Update the Pictures property of the course with the URL of the uploaded image
                    course.Pictures = uploadResult.SecureUri.ToString();

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Return the URL of the uploaded image
                    return Ok(uploadResult.SecureUri);
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


        [HttpPost("get courses")]
        public async Task<ActionResult<Models.Courses>> getCourse([FromBody] Filter filter)
        {

            var courses = _context.Courses.ToList();

            if (filter.sort == "eralist")
            {
                courses = courses.OrderBy(m => m.Date).ToList();

            }
            if (filter.sort == "latest")
            {
                courses = courses.OrderByDescending(m => m.Date).ToList();

            }
            if (filter.sort == "priceLow")
            {
                courses = courses.OrderBy(m => m.CouresValue).ToList();

            }
            if (filter.sort == "priceHigh")
            {
                courses = courses.OrderByDescending(m => m.CouresValue).ToList();

            }
            if (filter.sort == "ratingHigh")
            {
                courses = courses.OrderByDescending(m => m.Rating).ToList();

            }
            if (filter.sort == "ratingLow")
            {
                courses = courses.OrderBy(m => m.Rating).ToList();
            }
            if (filter.filterRating != 0)
            {
                courses = courses.Where(m => m.Rating >= filter.filterRating).ToList();

            }
            if (filter.filterPriceMin != 0)
            {
                courses = courses.Where(m => m.CouresValue >= filter.filterPriceMin).ToList();

            }
            if (filter.filterPriceMax != 0)
            {
                courses = courses.Where(m => m.CouresValue <= filter.filterPriceMax).ToList();

            }
            var Data = courses.Where(m => m.CouresName.ToLower().Contains(filter.serach.ToLower())).ToList()
                .Select(c => new CourseDetails
                {
                    CourseId = c.courseId,
                    CourseName = c.CouresName,
                    NumberOfStudents = _context.Enrollments.Count(e => e.CouresId == c.courseId),
                    TeacherName = _context.Users.Where(u => u.Id == c.UsersId).FirstOrDefault()!.UserName,
                    TeacherPicture = _context.Users.Where(m => m.Id == c.UsersId).FirstOrDefault()!.UsersPictrues,
                    Picture = c.Pictures,
                    CouresValue = c.CouresValue,
                    CouresDescription = c.CouresDescription,
                    CouresType = c.CouresType,
                    CoursesCatagory = c.CoursesCatagory,
                    CouresLanguage = c.CouresLanguage,
                    Date = c.Date,
                    Rating = c.Rating
                }).ToList();


            return Ok(Data);

        }
        public class Filter
        {
            public string serach { get; set; }
            public string sort { get; set; }
            public int filterRating { get; set; }
            public int filterPriceMin { get; set; }
            public int filterPriceMax { get; set; }


        }



        [HttpPost("get course/{courseId}")]
        public async Task<ActionResult<Models.Courses>> getCourse(int courseId)
        {
           


            var course = await _context.Courses.FirstOrDefaultAsync(m => m.courseId == courseId);
            if (course == null)
            {
                return NotFound("course Not Found !");
            }
            var Data = new CourseDetails
            {
                CourseId = course.courseId,
                CourseName = course.CouresName,
                NumberOfStudents = _context.Enrollments.Count(e => e.CouresId == course.courseId),
                TeacherName = _context.Users.Where(u => u.Id == course.UsersId).FirstOrDefault()!.UserName,
                Picture = course.Pictures,
                TeacherPicture = _context.Users.Where(m => m.Id == course.UsersId).FirstOrDefault()!.UsersPictrues,
                CouresValue = course.CouresValue,
                CouresDescription = course.CouresDescription,
                CouresType = course.CouresType,
                CoursesCatagory = course.CoursesCatagory,
                CouresLanguage = course.CouresLanguage,
                Date = course.Date,
                Rating = course.Rating
            };

            return Ok(Data);
        }



        [HttpPost("updateCourseRating/{token}/{userId}/{courseId}/{rating}")]
        public async Task<ActionResult<Models.Courses>> rating(string token, string userId, int courseId, int rating)
        {

            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id != userId)
            {
                return BadRequest("user Not Found !");
            }

            var ratingdata = new Rating()
            {
                courseId = courseId,
                usersId = user.Id,
                rating = rating
            };
            try
            {
                var userRating = _context.Rating.Where(m => m.usersId == user.Id && m.courseId == courseId).FirstOrDefault();

                if (userRating == null)
                {
                    _context.Rating.AddAsync(ratingdata);

                }
                else
                {
                    userRating.rating = rating;
                    _context.Rating.Update(userRating);
                }
                _context.SaveChanges();
                return Ok(new { success = true, message = "Rating added or updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("checkRating/{token}/{userId}/{courseId}")]
        public async Task<ActionResult<Models.Courses>> checkRating(string token, string userId, int courseId)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id != userId)
            {
                return BadRequest("user Not Found !");
            }
            var isPaid = _context.Enrollments.Where(m => m.UserId == userId && m.CouresId == courseId).FirstOrDefault();
            if (isPaid == null)
            {
                return BadRequest("You must buy this course.");
            }
            var userRating = _context.Rating.Where(m => m.usersId == user.Id && m.courseId == courseId).FirstOrDefault();

            if (userRating == null)
            {
                return NoContent();
            }

            else
            {
                var oldRating = new ratingDTO()
                {
                    rating = userRating.rating,
                };
                return Ok(oldRating);
            }
        }
        [HttpPost("get most common courses")]
        public async Task<ActionResult<Models.Courses>> commonCourses()
        {
            var course = _context.Courses.OrderByDescending(m => m.Rating).ThenBy(m => m.CouresValue).Take(10).ToList().Select(c => new CourseDetails
            {
                CourseId = c.courseId,
                CourseName = c.CouresName,
                NumberOfStudents = _context.Enrollments.Count(e => e.CouresId == c.courseId),
                TeacherName = _context.Users.Where(u => u.Id == c.UsersId).FirstOrDefault()!.UserName,
                TeacherPicture = _context.Users.Where(m => m.Id == c.UsersId).FirstOrDefault()!.UsersPictrues,
                Picture = c.Pictures,
                CouresValue = c.CouresValue,
                CouresDescription = c.CouresDescription,
                CouresType = c.CouresType,
                CoursesCatagory = c.CoursesCatagory,
                CouresLanguage = c.CouresLanguage,
                Date = c.Date,
                Rating = c.Rating
            }).ToList();

            return Ok(course);

        }

        [HttpPost("getCourseDataForTeacher/{token}/{userId}/{courseId}")]
        public async Task<ActionResult<Models.Courses>> getCourseDataForTeacher(string token, string userId, int courseId)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id == userId)
            {
                var course = _context.Courses.Where(m => m.courseId == courseId).FirstOrDefault();
                if (course == null)
                {
                    return NotFound();
                }
                if (user.Id == course.UsersId)
                {
                    return Ok(course);
                }
                else
                {
                    return BadRequest("you dont have acces");
                }
            }
            else
            {
                return BadRequest("you dont have acces");

            }
        }

        [HttpPost("update course/{token}/{userId}/{courseId}")]
        public async Task<ActionResult<Models.Courses>> updateCouese(string token, string userId, int courseId, Models.Courses courses)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id == userId)
            {
                if (user.Id == courses.UsersId)
                {
                    var olddata = _context.Courses.Where(m => m.courseId == courseId).FirstOrDefault();
                    if (olddata == null)
                    {
                        return NotFound();
                    }

                    olddata.CouresDescription = courses.CouresDescription;

                    olddata.CouresName = courses.CouresName;

                    olddata.CouresType = courses.CouresType;
                    olddata.CouresValue = courses.CouresValue;
                    olddata.CouresLanguage = courses.CouresLanguage;
                    olddata.CoursesCatagory = courses.CoursesCatagory;




                    _context.SaveChanges();
                    return Ok();

                }
                else
                {
                    return BadRequest("you dont have access");
                }

            }
            else
            {
                return BadRequest("user not found!");

            }

        }
        [HttpDelete("delete course/{token}/{userId}/{courseId}")]
        public async Task<ActionResult<Models.Courses>> deleteCourse(string token, string userId, int courseId)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id == userId)
            {
                var course = _context.Courses.Where(m => m.courseId == courseId).FirstOrDefault();
                if (course == null)
                {
                    return NotFound();
                }
                if (user.Id == course.UsersId)
                {
                    int? courseIdFilter = courseId; // Convert to nullable int
                    var enrollmentsToUpdate = _context.Enrollments.Where(e => e.CouresId == courseId).ToList();

                    foreach (var enrollment in enrollmentsToUpdate)
                    {
                        enrollment.CouresId = null; // Set the foreign key to null
                    }
                    _context.SaveChanges();

                    // Now, you can safely delete the course
                    var courseToDelete = _context.Courses.FirstOrDefault(c => c.courseId == courseId);
                    if (courseToDelete != null)
                    {
                        _context.Courses.Remove(courseToDelete);
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
                    return BadRequest("you dont have accsess");
                }
            }
            else
            {
                return BadRequest("user not found!");
            }


        }


        [HttpPost("check course/{token}/{userId}/{courseId}")]
        public async Task<ActionResult<Models.Courses>> checkCourse(string token, string userId, int courseId)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id == userId)
            {
                var ispurchased = _context.Enrollments.Where(m => m.UserId == userId && m.CouresId == courseId).FirstOrDefault();
                if (ispurchased == null)
                {
                    return BadRequest("You must buy this course.");
                }
                else
                {
                    return Ok();
                }
            }
            else
            {

                return BadRequest("user Not Found !");
            }


        }

        [HttpPost("get user courses/{token}/{userId}")]
        public async Task<ActionResult<Models.Courses>> getUserCourses(string token, string userId)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id != userId) { 
                return BadRequest("");
            }
            var enrollments = await _context.Enrollments.Where(m=>m.UserId==userId).ToListAsync();
            if(enrollments == null)
            {
                return BadRequest("there is no courses");
            }

            var data = enrollments
                .Where(m => m.UserId==userId)
                .Select(enrollment => new CourseDetails
                {
                    CourseId = enrollment.CouresId ?? 1,
                    CourseName = _context.Courses.FirstOrDefault(m => m.courseId == enrollment.CouresId)?.CouresName,
                    NumberOfStudents = _context.Enrollments.Count(e => e.CouresId == enrollment.CouresId),
                    TeacherName = _context.Users.FirstOrDefault(u => u.Id == enrollment.teacherId)?.UserName,
                    TeacherPicture = _context.Users.FirstOrDefault(u => u.Id == enrollment.teacherId)?.UsersPictrues,
                    Picture = _context.Courses.Where(m=>m.courseId==enrollment.CouresId).FirstOrDefault().Pictures,
                    CouresValue = _context.Courses.Where(m => m.courseId == enrollment.CouresId).FirstOrDefault().CouresValue,
                    CouresDescription = _context.Courses.Where(m => m.courseId == enrollment.CouresId).FirstOrDefault().CouresDescription,
                    CouresType = _context.Courses.Where(m => m.courseId == enrollment.CouresId).FirstOrDefault().CouresType,
                    CoursesCatagory = _context.Courses.Where(m => m.courseId == enrollment.CouresId).FirstOrDefault().CoursesCatagory,
                    CouresLanguage = _context.Courses.Where(m => m.courseId == enrollment.CouresId).FirstOrDefault().CouresLanguage,
                    Date = _context.Courses.Where(m => m.courseId == enrollment.CouresId).FirstOrDefault().Date,
                    Rating = _context.Courses.Where(m => m.courseId == enrollment.CouresId).FirstOrDefault().Rating,
                }).ToList();
            return Ok(data); 


        }
    }

    public class ratingDTO 
    {
        public int rating {  get; set; }

    }

}
