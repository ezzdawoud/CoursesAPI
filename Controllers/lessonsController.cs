using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Courses.Data;
using Courses.Helper;
using Courses.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;

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
            if (user.Id != id)
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
                .FirstOrDefaultAsync(m => m.lessonsNum == lessonsNumber && m.courseId == courseId);

            if (lesson == null)
            {
                return NotFound();
            }

            // Fetch teacher name
            var course = await _context.Courses.FirstOrDefaultAsync(m => m.courseId == courseId);

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
            var AllLessons = _context.Lessons.Where(m => m.courseId == courseId).ToList();
            // Fetch comments specific to the lesson








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
                courseLessons = AllLessons,
                teacherInfoApi = teacherInfo,
                courseName = _context.Courses.Where(m => m.courseId == courseId).FirstOrDefault().CouresName,
            };

            return Ok(response);
        }

        public class userData {
            public string userName { get; set; }
            public string pictrue { get; set; }
        }

        [HttpPost("get lessons comment/{courseId}/{lessonsNumber}/{id}/{token}")]
        public async Task<IActionResult> GetComments(int courseId, int lessonsNumber, string id, string token)
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
                .FirstOrDefaultAsync(m => m.lessonsNum == lessonsNumber && m.courseId == courseId);

            if (lesson == null)
            {
                return NotFound();
            }

            // Fetch teacher name
            var course = await _context.Courses.FirstOrDefaultAsync(m => m.courseId == courseId);

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
    userId = user.Id,
    Comment = new
    {
        comment.CommentsId,
        comment.comments,
        comment.date,
        comment.like,
        comment.dislike,
        isLiked = _context.Reactions.Any(m => m.userId == id && m.commentId == comment.CommentsId && m.reaction == true),
        isDisliked = _context.Reactions.Any(m => m.userId == id && m.commentId == comment.CommentsId && m.reaction == false),
        SubComments = _context.SubComments
         .Where(sc => sc.CommentsId == comment.CommentsId)
         .Select(sc => new
         {
             sc.SubCommentsId,
             sc.subComments,
             sc.like,
             sc.dislike,
             isLiked = _context.Reactions.Any(m => m.userId == id && m.SubCommentsId == sc.SubCommentsId && m.reaction == true),
             isDisliked = _context.Reactions.Any(m => m.userId == id && m.SubCommentsId == sc.SubCommentsId && m.reaction == false),
             username = _context.Users.Where(m => m.Id == sc.userId).FirstOrDefault()!.UserName,
             picutres = _context.Users.Where(m => m.Id == sc.userId).FirstOrDefault()!.UsersPictrues,
             userId = _context.Users.Where(m => m.Id == sc.userId).FirstOrDefault()!.Id,
         })
         .ToList()
    }
})
.ToListAsync();

            return Ok(comments);

        }
        [HttpPost("Add comments/{courseId}/{lessonsNumber}/{id}/{token}/{comments}")]
        public async Task<IActionResult> AddComments(int courseId, int lessonsNumber, string id, string token, string comments)
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
                .FirstOrDefaultAsync(m => m.lessonsNum == lessonsNumber && m.courseId == courseId);
            if (lesson == null)
            {
                return NotFound();
            }
            var newComment = new Comments
            {
                comments = comments,
                LessonsId = lesson.LessonsId,
                UsersId = user.Id,
                date = DateTime.Now,
                like = 0,
                dislike = 0
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
            var addedComment = await _context.Comments
                    .Where(c => c.CommentsId == newComment.CommentsId)
                    .Join(
                        _context.Users,
                        comment => comment.UsersId,
                        user => user.Id,
                        (comment, user) => new
                        {
                            UserName = user.UserName,
                            Picture = user.UsersPictrues,
                            userId = user.Id,
                            Comment = new
                            {
                                comment.CommentsId,
                                comment.comments,
                                comment.date,
                                comment.like,
                                comment.dislike,
                                isLiked = _context.Reactions.Any(m => m.userId == id && m.commentId == comment.CommentsId && m.reaction == true),
                                isDisliked = _context.Reactions.Any(m => m.userId == id && m.commentId == comment.CommentsId && m.reaction == false),
                                SubComments = _context.SubComments
                                    .Where(sc => sc.CommentsId == comment.CommentsId)
                                    .Select(sc => new
                                    {
                                        sc.SubCommentsId,
                                        sc.subComments,
                                        sc.like,
                                        sc.dislike,
                                        isLiked = _context.Reactions.Any(m => m.userId == id && m.SubCommentsId == sc.SubCommentsId && m.reaction == true),
                                        isDisliked = _context.Reactions.Any(m => m.userId == id && m.SubCommentsId == sc.SubCommentsId && m.reaction == false),
                                        username = _context.Users.Where(m => m.Id == sc.userId).FirstOrDefault()!.UserName,
                                        pictures = _context.Users.Where(m => m.Id == sc.userId).FirstOrDefault()!.UsersPictrues,
                                    })
                                    .ToList()
                            }
                        })
                    .FirstOrDefaultAsync();

            return Ok(addedComment);

        }
        [HttpPost("add Sub comment/{courseId}/{lessonsNumber}/{id}/{token}/{commentId}/{comments}")]
        public async Task<IActionResult> AddSubComments(int courseId, int lessonsNumber, string id, string token,int commentId, string comments)
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
                .FirstOrDefaultAsync(m => m.lessonsNum == lessonsNumber && m.courseId == courseId);
            if (lesson == null)
            {
                return NotFound();
            }
            var comment=_context.Comments.Where(m=>m.CommentsId == commentId).FirstOrDefault();
            if (comment == null)
            {
                return BadRequest();
            }

            var newComment = new SubComments
            {
                CommentsId = commentId,
                LessonsId = lesson.LessonsId,
                userId = user.Id,
                subComments=comments,
                like = 0,
                dislike = 0
                
            };
            _context.SubComments.Add(newComment);
            await _context.SaveChangesAsync();

            var addedComment = _context.SubComments.Where(m => m.SubCommentsId == newComment.SubCommentsId)
                        .Select(sc => new
                        {
                            sc.SubCommentsId,
                            sc.subComments,
                            sc.like,
                            sc.dislike,
                            isLiked = _context.Reactions.Any(m => m.userId == id && m.SubCommentsId == sc.SubCommentsId && m.reaction == true),
                            isDisliked = _context.Reactions.Any(m => m.userId == id && m.SubCommentsId == sc.SubCommentsId && m.reaction == false),
                            username = _context.Users.FirstOrDefault(u => u.Id == sc.userId).UserName,
                            picutres = _context.Users.FirstOrDefault(u => u.Id == sc.userId).UsersPictrues,
                            userId= _context.Users.FirstOrDefault(u => u.Id == sc.userId).Id,
                        })
                        .FirstOrDefault();
                
            
            return Ok(addedComment);

        }


        [HttpPost("delete comment/{courseId}/{lessonsNumber}/{id}/{token}/{commentId}/{commentType}")]
        public async Task<IActionResult> deleteComment(int courseId, int lessonsNumber, string id, string token, int commentId,int commentType)
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
                .FirstOrDefaultAsync(m => m.lessonsNum == lessonsNumber && m.courseId == courseId);
            if (lesson == null)
            {
                return NotFound();
            }
            if (commentType == 1) {
            var comment = _context.Comments.Where(m => m.CommentsId == commentId).FirstOrDefault();
            if(comment== null)
            {
                return BadRequest();
            }
            else if(comment.UsersId != id)
            {
                return BadRequest();
            }
            _context.Comments.Remove(comment);
            _context.SaveChanges();
            }
            else
            {
                var comment = _context.SubComments.Where(m => m.SubCommentsId == commentId).FirstOrDefault();
                if (comment == null)
                {
                    return BadRequest();
                }
                else if (comment.userId != id)
                {
                    return BadRequest();
                }
                _context.SubComments.Remove(comment);
                _context.SaveChanges();
            }
            return Ok();
        }

        
        


        [HttpPost("add reaction")]
        public async Task<IActionResult> addReaction([FromBody] ReactionDTO userReation)
            {
            Users user = await _tokenGenerator.GetUserFromToken(userReation.token);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Id != userReation.id)
            {
                return BadRequest();
            }
            
            if (userReation.commentsType == 1) {
                var comment = _context.Comments.Where(m => m.CommentsId == userReation.commentId).FirstOrDefault();
                if (comment == null)
                {
                    return BadRequest();
                }
                var reaction = new Reactions() {

                    commentId = comment.CommentsId,
                    reaction = userReation.userReaction,
                    userId = user.Id,
                    SubCommentsId = null,
                };
                _context.Reactions.Add(reaction);
                try
                {
                    await _context.SaveChangesAsync();

                    return Ok((new { message = "Reaction added successfully." }));
                }
                catch (DbUpdateException ex)
                {
                    // Log the detailed exception
                    return StatusCode(500, "Internal server error");
                }
            }


            else
            {
                var comment = _context.SubComments.Where(m => m.SubCommentsId == userReation.commentId).FirstOrDefault();
                if (comment == null)
                {
                    return BadRequest();
                }
                var reaction = new Reactions
                {
                    SubCommentsId = comment.SubCommentsId,
                    reaction = userReation.userReaction,
                    userId = userReation.id,
                    commentId = null
                };
                try
                {
                    _context.Reactions.Add(reaction);
                    await _context.SaveChangesAsync();
                    return Ok((new { message = "Reaction added successfully." }));
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(500, "Internal server error");
                }

            }
        }

        [HttpPost("delete reaction/{id}/{token}/{commentId}/{commentType}")]
        public async Task<IActionResult> deleteReaction(string id, string token, int commentId,int commentType)
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
            if (commentType == 1) { 
            var reaction=_context.Reactions.Where(m=>m.userId == id&& m.commentId==commentId).FirstOrDefault();
                if (reaction == null)
                {
                    return BadRequest();
                }
                _context.Reactions.Remove(reaction);
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                var reaction = _context.Reactions.Where(m => m.userId == id && m.SubCommentsId == commentId).FirstOrDefault();
                if (reaction == null)
                {
                    return BadRequest();
                }
                _context.Reactions.Remove(reaction);
                _context.SaveChanges();
                return Ok();

            }

        }



        public class ReactionDTO
        {
          public  int courseId { get; set; }
            public int lessonsNumber { get; set; }
            public string id { get; set; }
            public string token { get; set; }
            public int commentId { get; set; }
            public bool userReaction { get; set; }
            public int commentsType { get; set; }
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
