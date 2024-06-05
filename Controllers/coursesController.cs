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



        public class CourseUserData
        {
            public string id { get; set; }
            public string token { get; set; }
        }
        // GET: api/<coursesController>
        [HttpPost("get teacher courses")]
        public async Task<ActionResult<IEnumerable<Models.Courses>>> GetCourses([FromBody] CourseUserData userData)
        {
            Users user = await _tokenGenerator.GetUserFromToken(userData.token);
            if (user == null)
            {
                return NotFound("User Not Found !");
            }
            else if (user.Id == userData.id)
            {
                var courses = await _context.Courses
     .Where(c => c.UsersId == userData.id)
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
         Rating = c.Rating,
         TeacherPicture = _context.Users.Where(u => u.Id == c.UsersId).FirstOrDefault()!.UsersPictrues
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

        [HttpPost("insert Course")]
        public async Task<ActionResult<string>> insetCourse([FromBody] coursesDTO courses)
        {


            try
            {
                Users user = await _tokenGenerator.GetUserFromToken(courses.token);
                if (user == null)
                {
                    return NotFound();
                }
                else if (user.Id == courses.usersId)
                {
                    if (user.Id == courses.usersId)
                    {
                        var newCourse = new Models.Courses()
                        {
                            CouresDescription = courses.couresDescription,
                            CouresLanguage = courses.couresLanguage,
                            CouresName = courses.couresName,
                            CouresType = courses.CouresType,
                            CouresValue = courses.couresValue,
                            CoursesCatagory = courses.coursesCatagory,
                            UsersId = user.Id,
                            Pictures = courses.pictures,
                        };
                        _context.Courses.Add(newCourse);
                        _context.SaveChanges();

                        var courseId = courses.courseId;

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

            var courses = await _context.Courses.ToListAsync();

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

        public class Updaterating
        {
            public string token { get; set; }
            public string userId { get; set; }
            public int courseId { get; set; }
            public int rating { get; set; }
        }

        [HttpPost("updateCourseRating")]
        public async Task<ActionResult<Models.Courses>> rating([FromBody] Updaterating update)
        {

            Users user = await _tokenGenerator.GetUserFromToken(update.token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id != update.userId)
            {
                return BadRequest("user Not Found !");
            }

            var ratingdata = new Rating()
            {
                courseId = update.courseId,
                usersId = user.Id,
                rating = update.rating
            };
            try
            {
                var userRating = _context.Rating.Where(m => m.usersId == user.Id && m.courseId == update.courseId).FirstOrDefault();

                if (userRating == null)
                {
                    _context.Rating.AddAsync(ratingdata);

                }
                else
                {
                    userRating.rating = update.rating;
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
        public class Check
        {
            public string token { get; set; }
            public string userId { get; set; }
            public int courseId { get; set; }
        }

        [HttpPost("checkRating")]
        public async Task<ActionResult<Models.Courses>> checkRating([FromBody] Check check)
        {
            Users user = await _tokenGenerator.GetUserFromToken(check.token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id != check.userId)
            {
                return BadRequest("user Not Found !");
            }
            var isPaid = _context.Enrollments.Where(m => m.UserId == check.userId && m.CouresId == check.courseId).FirstOrDefault();
            if (isPaid == null)
            {
                return BadRequest("You must buy this course.");
            }
            var userRating = _context.Rating.Where(m => m.usersId == user.Id && m.courseId == check.courseId).FirstOrDefault();

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

        [HttpGet("get most common courses/{id}")]
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



        [HttpPost("getCourseDataForTeacher")]
        public async Task<ActionResult<Models.Courses>> getCourseDataForTeacher([FromBody] Check courseData)
        {
            Users user = await _tokenGenerator.GetUserFromToken(courseData.token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id == courseData.userId)
            {
                var course = _context.Courses.Where(m => m.courseId == courseData.courseId).FirstOrDefault();
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

        public class coursesDTO
        {
            public string couresName { get; set; }
            public string couresDescription { get; set; }
            public string CouresType { get; set; }
            public string coursesCatagory { get; set; }
            public string couresLanguage { get; set; }
            public int couresValue { get; set; }
            public string usersId { get; set; }
            public string token { get; set; }
            public int courseId { get; set; }
            public string? pictures { get; set; }
        }

        [HttpPost("update course")]
        public async Task<ActionResult<Models.Courses>> updateCouese([FromBody] coursesDTO courses)
        {
            Users user = await _tokenGenerator.GetUserFromToken(courses.token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id == courses.usersId)
            {
                if (user.Id == courses.usersId)
                {
                    var olddata = _context.Courses.Where(m => m.courseId == courses.courseId).FirstOrDefault();
                    if (olddata == null)
                    {
                        return NotFound();
                    }

                    olddata.CouresDescription = courses.couresDescription;

                    olddata.CouresName = courses.couresName;

                    olddata.CouresType = courses.CouresType;
                    olddata.CouresValue = courses.couresValue;
                    olddata.CouresLanguage = courses.couresLanguage;
                    olddata.CoursesCatagory = courses.coursesCatagory;




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



        [HttpPost("delete course")]
        public async Task<ActionResult<Models.Courses>> deleteCourse([FromBody] Check check)
        {
            Users user = await _tokenGenerator.GetUserFromToken(check.token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id == check.userId)
            {
                var course = _context.Courses.Where(m => m.courseId == check.courseId).FirstOrDefault();
                if (course == null)
                {
                    return NotFound();
                }
                var roles = await _userManeger.GetRolesAsync(user);
               
                if (user.Id == course.UsersId || roles.FirstOrDefault() == "admin")
                {
                    int? courseIdFilter = check.courseId; // Convert to nullable int
                    var enrollmentsToUpdate = _context.Enrollments.Where(e => e.CouresId == check.courseId).ToList();

                    foreach (var enrollment in enrollmentsToUpdate)
                    {
                        enrollment.CouresId = null; // Set the foreign key to null
                    }
                    _context.SaveChanges();

                    // Now, you can safely delete the course
                    var courseToDelete = _context.Courses.FirstOrDefault(c => c.courseId == check.courseId);
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


        [HttpPost("check course")]
        public async Task<ActionResult<Models.Courses>> checkCourse([FromBody] Check check)
        {
            Users user = await _tokenGenerator.GetUserFromToken(check.token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id == check.userId)
            {
                var course = _context.Courses.Where(m => m.courseId == check.courseId).FirstOrDefault();
                if (course == null)
                {
                    return BadRequest();
                }
                else if (course.UsersId == check.userId)
                {
                    return Ok();
                }

                else
                {
                    var roles = await _userManeger.GetRolesAsync(user);
                    if (roles.FirstOrDefault() == "admin")
                    {
                        return Ok();
                    }
                }
                var ispurchased = _context.Enrollments.Where(m => m.UserId == check.userId && m.CouresId == check.courseId).FirstOrDefault();
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
        public class UserData

        {
            public string id { get; set; }
            public string token { get; set; }
        }


        [HttpPost("get user courses")]
        public async Task<ActionResult<Models.Courses>> getUserCourses([FromBody] UserData userdata)
        {
            Users user = await _tokenGenerator.GetUserFromToken(userdata.token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id != userdata.id)
            {
                return BadRequest("");
            }
            var enrollments = await _context.Enrollments.Where(m => m.UserId == userdata.id).ToListAsync();
            if (enrollments == null)
            {
                return BadRequest("there is no courses");
            }

            var data = enrollments
                .Where(m => m.UserId == userdata.id)
                .Select(enrollment => new CourseDetails
                {
                    CourseId = enrollment.CouresId ?? 1,
                    CourseName = _context.Courses.FirstOrDefault(m => m.courseId == enrollment.CouresId)?.CouresName,
                    NumberOfStudents = _context.Enrollments.Count(e => e.CouresId == enrollment.CouresId),
                    TeacherName = _context.Users.FirstOrDefault(u => u.Id == enrollment.teacherId)?.UserName,
                    TeacherPicture = _context.Users.FirstOrDefault(u => u.Id == enrollment.teacherId)?.UsersPictrues,
                    Picture = _context.Courses.Where(m => m.courseId == enrollment.CouresId).FirstOrDefault().Pictures,
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


        [HttpPost("Bad Courses")]
        public async Task<ActionResult<Models.Courses>> BadCourses([FromBody] UserData userdata)
        {
            Users user = await _tokenGenerator.GetUserFromToken(userdata.token);
            if (user == null)
            {
                return BadRequest("user Not Found !");
            }
            if (user.Id != userdata.id)
            {
                return BadRequest("");
            }
            var roles = await _userManeger.GetRolesAsync(user);
            if (roles.FirstOrDefault() != "admin")
            {
                return BadRequest();
            }
            var badCourses = _context.Courses.Where(m => m.Rating < 3).ToList();
            if (badCourses.Any()) {
                return Ok(badCourses);

            }
            return BadRequest("there is no bad Courses");

        }
    }
    public class ratingDTO 
    {
        public int rating {  get; set; }

    }

}
