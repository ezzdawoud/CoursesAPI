using Courses.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Courses.Data;
using Microsoft.OpenApi.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Courses.Helper;
using Microsoft.AspNetCore.WebUtilities;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;
using NuGet.Common;
using static Courses.Controllers.adminController;
using Microsoft.AspNetCore.Cors;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<Users> _userManeger;
        private readonly SignInManager<Users> _signInManeger;
        private readonly Connections _context;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly GenarateToken _tokenGenerator;
        private readonly Cloudinary _cloudinary;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(Connections conections, UserManager<Users> userManeger, SignInManager<Users> signInManeger, IConfiguration configuration, IEmailSender emailSender, GenarateToken tokenGenerator, Cloudinary cloudinary)
        {
            _context = conections;
            _userManeger = userManeger;
            _signInManeger = signInManeger;
            _configuration = configuration;
            _emailSender = emailSender;
            _tokenGenerator = tokenGenerator;
            _cloudinary = cloudinary;


        }
        public class UserRoleSummary
        {
            public string Role { get; set; }
            public int UserCount { get; set; }
        }
        public class EnrollmentSummary
        {
            public DateTime Date { get; set; }
            public int TotalEnrollments { get; set; }
        }
        public class CourseChartData
        {
            public string? Name { get; set; }
            public int? Value { get; set; }
        }
        public class CourseSummary
        {
            public DateTime Date { get; set; }
            public int TotalCourses { get; set; }
        }

        [HttpPost("getData")]
        public async Task<IActionResult> adminData([FromBody] userData adminData)
        {
            Users user = await _tokenGenerator.GetUserFromToken(adminData.token);
            if (user == null)
            {
                return NotFound("User not found!");
            }
            if (user.Id != adminData.id)
            {
                return BadRequest("You don't have access");
            }
            var roles = await _userManeger.GetRolesAsync(user);
            if (roles.FirstOrDefault() != "admin")
            {
                return BadRequest();
            }

            var allCourses = _context.Courses.ToList().Count();

            var usersByRole = _context.UserRoles
       .Join(
           _context.Roles,
           userRole => userRole.RoleId,
           role => role.Id,
           (userRole, role) => new { UserRole = userRole, RoleName = role.Name }
       )
       .GroupBy(joined => joined.RoleName)
       .Select(g => new UserRoleSummary
       {
           Role = g.Key,
           UserCount = g.Count()
       })
       .ToList();

            var balnced = _context.Enrollments.Select(m => m.enrollmentValue).Sum();

            // Get summaries of enrollments
            var summaries = _context.Enrollments
                .GroupBy(e => e.EnrollmentDate.Date)
                .Select(g => new EnrollmentSummary
                {
                    Date = g.Key,
                    TotalEnrollments = g.Sum(e => e.enrollmentValue)
                })
                .ToList();

            // Get chart data for courses by category
            var chartData = _context.Courses
                .GroupBy(c => c.CoursesCatagory)
                .Select(g => new CourseChartData
                {
                    Name = g.Key,
                    Value = g.Count()
                })
                .ToList();

            // Get total number of courses per day
            var coursesPerDay = _context.Courses
                .GroupBy(c => c.Date.Date)
                .Select(g => new CourseSummary
                {
                    Date = g.Key,
                    TotalCourses = g.Count() // Count the total number of courses for each day
                })
                .ToList();

            var data = new
            {
                numberofCourses = allCourses,
                Roles = usersByRole,
                summaries = summaries,
                pieChartData = chartData,
                coursesPerDay = coursesPerDay, // Include the total number of courses per day in the response
                balnced = balnced
            };

            return Ok(data);
        }

        [HttpPost("get all users and teacher")]
        [AllowAnonymous]
        public async Task<IActionResult> getAllUsers([FromBody] userData Datauser)
        {
            Users user = await _tokenGenerator.GetUserFromToken(Datauser.token);
            if (user == null) {
                return BadRequest("the token is not valid !");
            }
            if (user.Id != Datauser.id)
            {
                return BadRequest("");
            }
            var roles = await _userManeger.GetRolesAsync(user);
            if (roles.FirstOrDefault() == "admin") {
                var teachers = await _userManeger.GetUsersInRoleAsync("teacher");
                var users = await _userManeger.GetUsersInRoleAsync("users");

                var allData = new
                {
                    teacher = teachers,
                    users = users
                };

                return Ok(allData);
            }
            else
            {
                return BadRequest("You are not suported to use this api");
            }
        }

        // GET api/<UsersController>/5
        [HttpPost("singin")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            string email = model.Email;
            string password = model.Password;

            var user = await _userManeger.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound("didnt find this Email");
            }
            if (user.EmailConfirmed == false)
            {
                return BadRequest("you must confirm your email");
            }
            var result = await _signInManeger.PasswordSignInAsync(user, password, false, false);
            if (result.Succeeded)
            {
                var roles = await _userManeger.GetRolesAsync(user);

                // Create a custom object to return both message and role
                var token = await _tokenGenerator.GenerateJwtToken(user);
                var response = new
                {
                    Role = roles.FirstOrDefault(), // Assuming a user can have only one role
                    usertoken = token,
                    Id = user.Id
                };
                return Ok(response);
            }
            else if (result.IsLockedOut)
            {
                return BadRequest("User account locked out.");
            }
            else
            {
                return BadRequest("Wrong Passwrod!");
            }
        }
        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string token)
        {
            Users users = await _tokenGenerator.GetUserFromToken(token);

            // Get the currently authenticated user

            if (users == null)
            {
                // User is not authenticated, return error or redirect
                return Unauthorized();
            }

            // Call the method to remove the token from the user
            await _tokenGenerator.RemoveTokenFromUser(users);

            // Optionally perform additional logout operations

            // Return success response
            return Ok("Logout successful");
        }

        // POST api/<UsersController>
        [HttpPost("register")]
        public async Task<IActionResult> Register(Users registerViewModel)
        {


            var user = await _userManeger.FindByEmailAsync(registerViewModel.Email);

            if (user != null)
            {
                return BadRequest(new { message = "This email address is already in use" });

            }

            var userWithSameUsername = await _userManeger.FindByNameAsync(registerViewModel.UserName);
            if (userWithSameUsername != null)
            {
                return BadRequest(new { message = $"This {registerViewModel.UserName} is already in use" });

            }

            var newUser = new Users()
            {
                UserName = registerViewModel.UserName,
                Email = registerViewModel.Email,
                PhoneNumber = registerViewModel.PhoneNumber,
                UsersPictrues=registerViewModel.UsersPictrues
            };

            var newUserResponse = await _userManeger.CreateAsync(newUser, registerViewModel.PasswordHash);

            if (newUserResponse.Succeeded)
            {
                var addToRole = await _userManeger.AddToRoleAsync(newUser, UserRoles.Users);
                // Generate an email confirmation token
                var emailConfirmationToken = await _userManeger.GenerateEmailConfirmationTokenAsync(newUser);

                // Encode the email confirmation token
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));

                // Construct the confirmation link
                var confirmationLink = $"https://coursesv3.vercel.app/confirm-email/{newUser.Id}/{encodedToken}";

                // Send confirmation email using IEmailSender
                // Assuming you have an email sender service
                await _emailSender.SendEmailAsync(newUser.Email, "Confirm your email", $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>");
                return Ok(new { message = "Registration successful. Please check your email for confirmation" });


            }

            // If registration fails, return errors
            return BadRequest(new { message = string.Join("\n", newUserResponse.Errors.Select(e => e.Description)) });
        }

        [HttpPost("upload user pictures")]
        public async Task<IActionResult> UploadUserImage([FromForm] IFormFile file, [FromForm] string id, [FromForm] string token)
        {
            if (file == null || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Invalid input data" });
            }

            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {

                return NotFound(new { message = "User not found!" });
            }

            if (user.Id != id)
            {
                return BadRequest(new { message = "You don't have access" });
            }
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            // Upload image to Cloudinary
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // Assuming you're using Entity Framework Core, retrieve the course from the database

            // Update the Pictures property of the course with the URL of the uploaded image
            var users = await _context.Users.Where(m => m.Id == id).FirstOrDefaultAsync();
            users.UsersPictrues = uploadResult.SecureUri.ToString();

            // Save changes to the database
            await _context.SaveChangesAsync();
            return Ok(uploadResult);
        }


        public class confirem
        {
            public string userId { get; set; }
            public string token { get; set; }
        }

        [HttpPost("confirmemail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] confirem confirem)
        {
            if (confirem.userId == null || confirem.token == null)
            {
                return BadRequest(new { message = "Error confirming email. " });
            }
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(confirem.token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var user = await _userManeger.FindByIdAsync(confirem.userId);
            if (user == null)
            {
                return BadRequest(new { message = "Error confirming email. " });
            }

            var result = await _userManeger.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                return Ok(new { message = "done confirming email. " });
            }
            else
            {
                return BadRequest(new { message = "Error confirming email. " });

            }

        }

        [HttpPost("requestEmailChange/{oldEmail}")]
        public async Task<IActionResult> RequestEmailChange(string oldEmail)
        {
            var user = await _userManeger.FindByEmailAsync(oldEmail);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var emailChangeToken = await _userManeger.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "ChangeEmail");
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailChangeToken));
            var confirmationLink = $"https://coursesv3.vercel.app/changeEmail?token={encodedToken}&email={oldEmail}";

            await _emailSender.SendEmailAsync(oldEmail, "Confirm your email change", $"Please confirm your email change by clicking this link: <a href='{confirmationLink}'>Confirm Email Change</a>");

            return Ok(new { message = "\"Confirmation link has been sent to your email.\"" });

        }

        [HttpPost("verify-email-change-token")]
        public async Task<IActionResult> VerifyEmailChangeToken([FromBody] VerifyEmailChangeTokenModel model)
        {
            var user = await _userManeger.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var isValid = await _userManeger.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ChangeEmail", decodedToken);
            if (!isValid)
            {
                return BadRequest("Invalid or expired token");
            }

            return Ok(new { message = "Token is valid" });
        }


        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailModel model)
        {
            // Decode the token
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            // Find the user associated with the token
            var user = await _userManeger.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var Email = await _userManeger.FindByEmailAsync(model.NewEmail);
            if (Email != null)
            {
                return BadRequest("email alardy used");
            }
            // Verify the email change token
            var tokenValid = await _userManeger.VerifyUserTokenAsync(user, _userManeger.Options.Tokens.EmailConfirmationTokenProvider, "ChangeEmail", decodedToken);
            if (!tokenValid)
            {
                return BadRequest("Invalid token.");
            }

            // Generate the email confirmation token for the new email
            var newEmailToken = await _userManeger.GenerateChangeEmailTokenAsync(user, model.NewEmail);

            // Encode the new email confirmation token
            var encodedNewEmailToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(newEmailToken));

            // Construct the email confirmation link
            var confirmationLink = $"https://coursesv3.vercel.app/confirm-new-email/{user.Id}/{encodedNewEmailToken}/{model.NewEmail}";

            // Send confirmation email to the new email address
            await _emailSender.SendEmailAsync(model.NewEmail, "Confirm your new email", $"Please confirm your new email by clicking this link: <a href='{confirmationLink}'>Confirm New Email</a>");
            return Ok(new { message = "Please check your new email to confirm the change" });

        }


        [HttpPost("confirm-new-email/{userId}/{token}/{newEmail}")]
        public async Task<IActionResult> ConfirmNewEmail([FromBody] ChangeEmailModel changeEmailModel)
        {
            var user = await _userManeger.FindByIdAsync(changeEmailModel.userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Decode the token
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(changeEmailModel.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            // Confirm the email change
            var result = await _userManeger.ChangeEmailAsync(user, changeEmailModel.NewEmail, decodedToken);

            if (result.Succeeded)
            {
                return Ok(new { message = "Email address has been changed successfully." });

            }

            return BadRequest("Error changing email address.");
        }

        public class VerifyEmailChangeTokenModel
        {
            public string Email { get; set; }
            public string Token { get; set; }
        }

        public class ChangeEmailModel
        {
            public string? userId { get; set; }
            public string? Email { get; set; }
            public string Token { get; set; }
            public string NewEmail { get; set; }
        }

        public class userData
        {
            public string id { get; set; }
            public string token { get; set; }
        }

        [HttpPost("getUserRole")]
        public async Task<IActionResult> getUserRole([FromBody] userData data)
        {
            Users user = await _tokenGenerator.GetUserFromToken(data.token);
            if (user == null)
            {
                return NotFound("User not found!");
            }

            if (user.Id != data.id)
            {
                return BadRequest("You don't have access");
            }

            // Await the GetRolesAsync method to get the actual list of roles
            var roles = await _userManeger.GetRolesAsync(user);

            return Ok(roles);
        }

        [HttpPost("getUserData")]
        public async Task<IActionResult> getUserInformation([FromBody] userData data)
        {
            Users user = await _tokenGenerator.GetUserFromToken(data.token);
            if (user == null)
            {
                return NotFound("User not found!");
            }

            if (user.Id != data.id)
            {
                return BadRequest("You don't have access");
            }
            UserSTO userData = new UserSTO()
            {
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                UsersPictrues = user.UsersPictrues,
                userEmail = user.Email,
            };

            return Ok(userData);


        }
        public class update
        {
            public string id { get; set; }
            public string token { get; set; }
            public string name { get; set; }
            public string phoneNumber { get; set; }
        }

        [HttpPost("update user data")]
        public async Task<IActionResult> UpdateUserData([FromBody] update update)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(update.id) || string.IsNullOrEmpty(update.token) || string.IsNullOrEmpty(update.name))
            {
                return BadRequest("Invalid input parameters");
            }
            try
            {
                Users user = await _tokenGenerator.GetUserFromToken(update.token);
                if (user == null)
                {
                    return NotFound("User not found!");
                }
                var users = _context.Users.Where(m => m.UserName == update.name).FirstOrDefault();
                if (users != null)
                {
                    return BadRequest((new { message = " user name alaredy used." }));
                }
                if (user.Id != update.id)
                {
                    return BadRequest("You don't have access");
                }
                else
                {
                    var oldUser = _context.Users.FirstOrDefault(u => u.Id == update.id);
                    if (oldUser == null)
                    {
                        return NotFound("User not found!");
                    }

                    oldUser.UserName = update.name;
                    oldUser.PhoneNumber = update.phoneNumber;
                    await _context.SaveChangesAsync(); // Save changes asynchronously

                    return Ok((new { message = " updated successfully." }));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating user data");
            }
        }

        [HttpPost("request-password-change")]
        public async Task<IActionResult> RequestPasswordChange([FromBody] RequestPasswordChangeModel model)
        {
            var user = await _userManeger.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(new { message = " User not found" });
            }

            var token = await _userManeger.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationLink = $"https://coursesv3.vercel.app/changePassword/{encodedToken}/{user.Email}";
            await _emailSender.SendEmailAsync(user.Email, "Confirm Password Change", $"Please confirm your password change by clicking this link: <a href='{confirmationLink}'>Confirm Password Change</a>");
            return Ok(new { message = "Password change request initiated. Please check your email for confirmation." });
        }
        public class RequestPasswordChangeModel
        {
            public string Email { get; set; }
        }

        [HttpPost("confirm-password-change-token")]
        public async Task<IActionResult> ConfirmPasswordChangeToken([FromBody] ConfirmPasswordChangeTokenModel model)
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var user = await _userManeger.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var isValid = await _userManeger.VerifyUserTokenAsync(user, "Default", "ResetPassword", decodedToken);
            if (!isValid)
            {
                return BadRequest("Invalid or expired token");
            }

            return Ok(new { message = "Token is valid" });
        }
        public class ConfirmPasswordChangeTokenModel
        {
            public string Email { get; set; }
            public string Token { get; set; }
        }
        public class ResetPasswordModel
        {
            public string Email { get; set; }
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var user = await _userManeger.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManeger.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "Password changed successfully" });
        }
        public class contactData{
            public string email { get; set; }
            public string username { get; set; }
            public string subject {  get; set; }
            public string body { get; set; }

}

        [HttpPost("insert contact")]
        public async Task<IActionResult> contact([FromBody] contactData model)
        {

            var user = _context.Users.Where(m => m.Email == model.email).FirstOrDefault();
            if(user == null)
            {
                var contact = new Contact
                {
                    Email = model.email,
                    subject = model.subject,
                    Message = model.body
                };
                _context.Contacts.Add(contact);
                _context.SaveChanges();
            }
            else
            {
                var contact = new Contact
                {
                    Email = model.email,
                    subject = model.subject,
                    Message = model.body,
                    userId=user.Id
                };
                _context.Contacts.Add(contact);
                _context.SaveChanges();
            }
            return Ok();
           
        }
        public class delete
        {
            public string token { get; set; }
        public string id {  get; set; }
            public string userId {  get; set; }
        }


        [HttpPost("delete user")]
        public async Task<IActionResult> deleteUser([FromBody] delete model)
        {
            Users user = await _tokenGenerator.GetUserFromToken(model.token);
            if (user == null)
            {
                return NotFound("User not found!");
            }
            if (user.Id != model.id)
            {
                return BadRequest("You don't have access");
            }
            var roles = await _userManeger.GetRolesAsync(user);
            if (roles.FirstOrDefault() != "admin")
            {
                return BadRequest();
            }
            var deleteduser = _context.Users.Where(m => m.Id == model.userId).FirstOrDefault();
            if (deleteduser == null) {
                return BadRequest();
            }
            var reactions = _context.Reactions.Where(r => r.userId == model.userId).ToList();
            if (reactions.Any())
            {
                _context.Reactions.RemoveRange(reactions);
                _context.SaveChanges();
            }
            _context.Users.Remove(deleteduser);
            _context.SaveChanges();
            return Ok();
        }

        public class UserSTO
        {
            public string UserName { get; set; }
            public string PhoneNumber { get; set; }
            public string UsersPictrues { get; set; }
            public string userEmail { get; set; }
        }






    }
}