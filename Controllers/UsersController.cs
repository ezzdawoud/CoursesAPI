﻿using Courses.Models;
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

        public UsersController(Connections conections, UserManager<Users> userManeger, SignInManager<Users> signInManeger, IConfiguration configuration,IEmailSender emailSender, GenarateToken tokenGenerator, Cloudinary cloudinary)
        {
            _context = conections;
            _userManeger = userManeger;
            _signInManeger = signInManeger;
            _configuration = configuration;
            _emailSender = emailSender;
            _tokenGenerator = tokenGenerator;
            _cloudinary = cloudinary;


        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IActionResult> Get(string token)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if(user == null) { 
            return BadRequest("the token is not valid !");    
            }
            var roles = await _userManeger.GetRolesAsync(user);
            if (roles.FirstOrDefault() == "admin") {
                return Ok(_context.Users.ToList());
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
            if(user.EmailConfirmed == false)
            {
                return BadRequest("you must confierm your email");
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
                    usertoken= token,
                    Id=user.Id
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
                return BadRequest("This email address is already in use.");
            }


            var newUser = new Users()
            {
                UserName = registerViewModel.UserName,
                Email = registerViewModel.Email,
                PhoneNumber = registerViewModel.PhoneNumber
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
                var confirmationLink = $"http://localhost:4200/confirm-email/{newUser.Id}/{encodedToken}";

                // Send confirmation email using IEmailSender
                // Assuming you have an email sender service
                await _emailSender.SendEmailAsync(newUser.Email, "Confirm your email", $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>");

                return Ok("Registration successful. Please check your email for confirmation.");

            }

            // If registration fails, return errors
            return BadRequest(string.Join("\n", newUserResponse.Errors.Select(e => e.Description)));
        }

        [HttpPost("upload user pictures/{id}/{token}")]
        public async Task<IActionResult> uploadUserImage(string id, string token, IFormFile file)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return NotFound("User not found!");
            }

            if (user.Id != id)
            {
                return BadRequest("You don't have access");
            }
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
            var users = await _context.Users.Where(m => m.Id == id).FirstOrDefaultAsync();
            users.UsersPictrues = uploadResult.SecureUri.ToString();

            // Save changes to the database
            await _context.SaveChangesAsync();
            return Ok(uploadResult);
        }




        [HttpPost("confirmemail/{userId}/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Invalid token or user ID.");
            }
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var user = await _userManeger.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await _userManeger.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully.");
            }
            else
            {
                return BadRequest("Error confirming email.");
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
            var confirmationLink = $"http://localhost:4200/changeEmail?token={encodedToken}&email={oldEmail}";

            await _emailSender.SendEmailAsync(oldEmail, "Confirm your email change", $"Please confirm your email change by clicking this link: <a href='{confirmationLink}'>Confirm Email Change</a>");

            return Ok(new { message = "\"Confirmation link has been sent to your email.\""});

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
            var Email=await _userManeger.FindByEmailAsync(model.NewEmail);
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
            var confirmationLink = $"http://localhost:4200/confirm-new-email/{user.Id}/{encodedNewEmailToken}/{model.NewEmail}";

            // Send confirmation email to the new email address
            await _emailSender.SendEmailAsync(model.NewEmail, "Confirm your new email", $"Please confirm your new email by clicking this link: <a href='{confirmationLink}'>Confirm New Email</a>");
            return Ok(new { message = "Please check your new email to confirm the change"});

        }

        [HttpPost("confirm-new-email/{userId}/{token}/{newEmail}")]
        public async Task<IActionResult> ConfirmNewEmail(string userId, string token, string newEmail)
        {
            var user = await _userManeger.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Decode the token
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            // Confirm the email change
            var result = await _userManeger.ChangeEmailAsync(user, newEmail, decodedToken);

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
            public string Email { get; set; }
            public string Token { get; set; }
            public string NewEmail { get; set; }
        }

        [HttpPost("getUserRole/{id}/{token}")]
        public async Task<IActionResult> getUserRole(string id, string token)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return NotFound("User not found!");
            }

            if (user.Id != id)
            {
                return BadRequest("You don't have access");
            }

            // Await the GetRolesAsync method to get the actual list of roles
            var roles = await _userManeger.GetRolesAsync(user);

            return Ok(roles);
        }

        [HttpPost("getUserData/{id}/{token}")]
        public async Task<IActionResult> getUserInformation(string id, string token)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return NotFound("User not found!");
            }

            if (user.Id != id)
            {
                return BadRequest("You don't have access");
            }
           UserSTO userData=new UserSTO()
           {
               UserName = user.UserName,
               PhoneNumber=user.PhoneNumber,
               UsersPictrues=user.UsersPictrues,
               userEmail=user.Email,
           };

            return Ok(userData);
          

        }

        [HttpPost("update user data/{id}/{token}/{name}/{number}")]
        public async Task<IActionResult> UpdateUserData(string id, string token, string name, string number)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(name))
            {
                return BadRequest("Invalid input parameters");
            }
            try
            {
                Users user = await _tokenGenerator.GetUserFromToken(token);
                if (user == null)
                {
                    return NotFound("User not found!");
                }
                var users=_context.Users.Where(m=>m.UserName==name).FirstOrDefault();
                if (users != null)
                {
                    return BadRequest((new { message = " user name alaredy used." }));
                }
                if (user.Id != id)
                {
                    return BadRequest("You don't have access");
                }
                else
                {
                    var oldUser = _context.Users.FirstOrDefault(u => u.Id == id);
                    if (oldUser == null)
                    {
                        return NotFound("User not found!");
                    }
                    
                    oldUser.UserName = name;
                    oldUser.PhoneNumber = number;
                    await _context.SaveChangesAsync(); // Save changes asynchronously

                    return Ok((new { message = " updated successfully." }));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating user data");
            }
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