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
        public UsersController(Connections conections, UserManager<Users> userManeger, SignInManager<Users> signInManeger, IConfiguration configuration,IEmailSender emailSender, GenarateToken tokenGenerator)
        {
            _context = conections;
            _userManeger = userManeger;
            _signInManeger = signInManeger;
            _configuration = configuration;
            _emailSender = emailSender;
            _tokenGenerator = tokenGenerator;


        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IActionResult> Get(string token)
        {
            Users user = await _tokenGenerator.GetUserFromToken(token);
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
        [HttpPost("{email},{password}")]
        public async Task<IActionResult> Login(string email, string password)
        {

            var user = await _userManeger.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest("Invalid login attempt.");
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
                var response = new
                {
                    Message = "User logged in successfully.",
                    UserRole = roles.FirstOrDefault() // Assuming a user can have only one role
                };
                var token = await _tokenGenerator.GenerateJwtToken(user);
                return Ok(response);
            }
            else if (result.IsLockedOut)
            {
                return BadRequest("User account locked out.");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return BadRequest(ModelState);
            }
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
        [HttpPost("register/{registerViewModel}")]
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

                // Construct the confirmation link

                var confirmationLink = Url.Action("ConfirmEmail", "Users", new { userId = newUser.Id, token = emailConfirmationToken }, Request.Scheme);
                // Send confirmation email using IEmailSender
                // Assuming you have an email sender service
                await _emailSender.SendEmailAsync(newUser.Email, "Confirm your email", $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>");

                return Ok("Registration successful. Please check your email for confirmation.");
            }

            // If registration fails, return errors
            return BadRequest(string.Join("\n", newUserResponse.Errors.Select(e => e.Description)));
        }



        [HttpGet]
        [AllowAnonymous]
        [Route("api/confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Invalid token or user ID.");
            }

            var user = await _userManeger.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await _userManeger.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully.");
            }
            else
            {
                return BadRequest("Error confirming email.");
            }
        }

       
    }
}