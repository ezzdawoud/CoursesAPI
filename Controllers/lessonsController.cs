using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Courses.Data;
using Courses.Helper;
using Courses.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using static Courses.Controllers.lessonsController;

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
        
        [HttpPost("insert lessons")]
        public async Task<ActionResult<string>> insetLessons([FromForm] lessonsuserData data,[FromForm]  Lessons lessons)
        {
            Users user = await _tokenGenerator.GetUserFromToken(data.Token);
            if (user == null)
            {
                return NotFound();
            }
            else if (user.Id == data.userId)
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
                   
                    return Ok(newLessons);
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

        public class insertVideo
        {
            public int courseId { get; set; }
            public int lessonsId { get; set; }
            public string id {  get; set; }
            public string token {  get; set; }
        }

        [HttpPost("upload video")]
        public async Task<IActionResult> UploadVideo([FromForm]insertVideo data,[FromForm] IFormFile file)
        {
            Users user = await _tokenGenerator.GetUserFromToken(data.token);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Id != data.id)
            {
                return BadRequest();
            }
            var course = await _context.Courses.FindAsync(data.courseId);
            if (course == null)
            {
                return BadRequest("Invalid course ID.");
            }

            var lessons = await _context.Lessons.FindAsync(data.lessonsId);
            if (lessons == null)
            {
                return BadRequest("Invalid lesson ID.");
            }

            if (user.Id != data.id || user.Id != course.UsersId)
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
        public class GetLesson
        {
            public int courseId { get; set; }
            public int lessonsNumber { get; set; }
            public string id { get; set; }
            public string token { get; set; }
        }
        [HttpPost("getlessons")]
        public async Task<IActionResult> GetLessons([FromBody] GetLesson get)
        {
            Users user = await _tokenGenerator.GetUserFromToken(get.token);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Id != get.id)
            {
                return BadRequest();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.lessonsNum == get.lessonsNumber && m.courseId == get.courseId);

            if (lesson == null)
            {
                return NotFound();
            }

            // Fetch teacher name
            var course = await _context.Courses.FirstOrDefaultAsync(m => m.courseId == get.courseId);

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
            var AllLessons = _context.Lessons.Where(m => m.courseId == get.courseId).ToList();
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
                courseName = _context.Courses.Where(m => m.courseId == get.courseId).FirstOrDefault().CouresName,
            };

            return Ok(response);
        }

        public class lessonsuserData {
            public string userId { get; set; }
            public string Token { get; set; }
        }

       

        [HttpPost("get lessons comment")]
        public async Task<IActionResult> GetComments([FromBody] GetLesson get)
        {
            Users user = await _tokenGenerator.GetUserFromToken(get.token);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Id != get.id)
            {
                return BadRequest();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.lessonsNum == get.lessonsNumber && m.courseId == get.courseId);

            if (lesson == null)
            {
                return NotFound();
            }

            // Fetch teacher name
            var course = await _context.Courses.FirstOrDefaultAsync(m => m.courseId == get.courseId);

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
        isLiked = _context.Reactions.Any(m => m.userId == get.id && m.commentId == comment.CommentsId && m.reaction == true),
        isDisliked = _context.Reactions.Any(m => m.userId == get.id && m.commentId == comment.CommentsId && m.reaction == false),
        SubComments = _context.SubComments
         .Where(sc => sc.CommentsId == comment.CommentsId)
         .Select(sc => new
         {
             sc.SubCommentsId,
             sc.subComments,
             sc.like,
             sc.dislike,
             isLiked = _context.Reactions.Any(m => m.userId == get.id && m.SubCommentsId == sc.SubCommentsId && m.reaction == true),
             isDisliked = _context.Reactions.Any(m => m.userId == get.id && m.SubCommentsId == sc.SubCommentsId && m.reaction == false),
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
        public class addcomments
        {
            public int courseId { get; set; }
            public int lessonsNumber {  get; set; }
            public string id {  get; set; }
            public string token {  get; set; }
            public string comments {  get; set; }
        }
        [HttpPost("Add comments")]
        public async Task<IActionResult> AddComments([FromBody] addcomments addcomments)
        {
            Users user = await _tokenGenerator.GetUserFromToken(addcomments.token);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Id != addcomments.id)
            {
                return BadRequest();
            }

            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.lessonsNum == addcomments.lessonsNumber && m.courseId == addcomments.courseId);
            if (lesson == null)
            {
                return NotFound();
            }
            var newComment = new Comments
            {
                comments = addcomments.comments,
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
                                isLiked = _context.Reactions.Any(m => m.userId == addcomments.id && m.commentId == comment.CommentsId && m.reaction == true),
                                isDisliked = _context.Reactions.Any(m => m.userId == addcomments.id && m.commentId == comment.CommentsId && m.reaction == false),
                                SubComments = _context.SubComments
                                    .Where(sc => sc.CommentsId == comment.CommentsId)
                                    .Select(sc => new
                                    {
                                        sc.SubCommentsId,
                                        sc.subComments,
                                        sc.like,
                                        sc.dislike,
                                        isLiked = _context.Reactions.Any(m => m.userId == addcomments.id && m.SubCommentsId == sc.SubCommentsId && m.reaction == true),
                                        isDisliked = _context.Reactions.Any(m => m.userId == addcomments.id && m.SubCommentsId == sc.SubCommentsId && m.reaction == false),
                                        username = _context.Users.Where(m => m.Id == sc.userId).FirstOrDefault()!.UserName,
                                        pictures = _context.Users.Where(m => m.Id == sc.userId).FirstOrDefault()!.UsersPictrues,
                                    })
                                    .ToList()
                            }
                        })
                    .FirstOrDefaultAsync();

            return Ok(addedComment);

        }

        public class addSubComment
        {
            public int courseId { get; set; }
            public int lessonsNumber {  get; set; }
            public string id {  get; set; }
            public string token {  get; set; }
            public int commentId {  get; set; }
            public string comments { get; set; }
        }

        [HttpPost("add Sub comment")]
        public async Task<IActionResult> AddSubComments([FromBody] addSubComment add)
        {
            Users user = await _tokenGenerator.GetUserFromToken(add.token);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Id != add.id)
            {
                return BadRequest();
            }
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(m => m.lessonsNum == add.lessonsNumber && m.courseId == add.courseId);
            if (lesson == null)
            {
                return NotFound();
            }
            var comment=_context.Comments.Where(m=>m.CommentsId == add.commentId).FirstOrDefault();
            if (comment == null)
            {
                return BadRequest();
            }

            var newComment = new SubComments
            {
                CommentsId = add.commentId,
                LessonsId = lesson.LessonsId,
                userId = user.Id,
                subComments= add.comments,
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
                            isLiked = _context.Reactions.Any(m => m.userId == add.id && m.SubCommentsId == sc.SubCommentsId && m.reaction == true),
                            isDisliked = _context.Reactions.Any(m => m.userId == add.id && m.SubCommentsId == sc.SubCommentsId && m.reaction == false),
                            username = _context.Users.FirstOrDefault(u => u.Id == sc.userId).UserName,
                            picutres = _context.Users.FirstOrDefault(u => u.Id == sc.userId).UsersPictrues,
                            userId= _context.Users.FirstOrDefault(u => u.Id == sc.userId).Id,
                        })
                        .FirstOrDefault();
                
            
            return Ok(addedComment);

        }
        public class DeleteComment
        {
            public int courseId { get; set; }
            public int lessonsNumber {  get; set; }
            public string id {  get; set; }
            public string token {  get; set; }
            public int commentId {  get; set; }
            public int commentType {  get; set; }
        }

        // Define a method to delete reactions associated with a comment
        private void DeleteReactionsForComment(int commentId)
        {
            var reactions = _context.Reactions.Where(r => r.commentId == commentId);
            _context.Reactions.RemoveRange(reactions);
            _context.SaveChanges();
        }

        [HttpPost("delete comment")]
        public async Task<IActionResult> deleteComment([FromBody] DeleteComment delete)
        {
            // Validate user and course
            Users user = await _tokenGenerator.GetUserFromToken(delete.token);
            var course = _context.Courses.FirstOrDefault(m => m.courseId == delete.courseId);
            if (user == null || course == null || user.Id != delete.id)
            {
                return BadRequest();
            }

            // Retrieve lesson
            var lesson = await _context.Lessons.FirstOrDefaultAsync(m => m.lessonsNum == delete.lessonsNumber && m.courseId == delete.courseId);
            if (lesson == null)
            {
                return NotFound();
            }

            // Delete comment or subcomment
            if (delete.commentType == 1)
            {
                var comment = _context.Comments.FirstOrDefault(m => m.CommentsId == delete.commentId);
                if (comment == null)
                {
                    return BadRequest();
                }

                // Check ownership or course instructor
                if (course.UsersId == delete.id || comment.UsersId == delete.id)
                {
                    // Delete associated reactions
                    DeleteReactionsForComment(comment.CommentsId);

                    _context.Comments.Remove(comment);
                    _context.SaveChanges();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                var subComment = _context.SubComments.FirstOrDefault(m => m.SubCommentsId == delete.commentId);
                if (subComment == null)
                {
                    return BadRequest();
                }

                // Check ownership or course instructor
                if (course.UsersId == delete.id || subComment.userId == delete.id)
                {
                    _context.SubComments.Remove(subComment);
                    _context.SaveChanges();
                }
                else
                {
                    return BadRequest();
                }
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

        public class DeleteReacions
        {
            public string id { get; set; }
            public string token {  get; set; }
            public int commentId {  get; set; }
            public int commentType {  get; set; }
        }

        [HttpPost("delete reaction")]
        public async Task<IActionResult> deleteReaction([FromBody] DeleteReacions delete)
        {
            Users user = await _tokenGenerator.GetUserFromToken(delete.token);
            if (user == null)
            {
                return NotFound();
            }
            if(user.Id != delete.id)
            {
                return BadRequest();
            }
            if (delete.commentType == 1) { 
            var reaction=_context.Reactions.Where(m=>m.userId == delete.id && m.commentId== delete.commentId).FirstOrDefault();
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
                var reaction = _context.Reactions.Where(m => m.userId == delete.id && m.SubCommentsId == delete.commentId).FirstOrDefault();
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



       
    }
}
