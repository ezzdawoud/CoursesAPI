using Courses.Data;
using Courses.Helper;
using Courses.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Courses.Controllers.teacherController;
using Microsoft.EntityFrameworkCore;

namespace Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class adminController : ControllerBase
    {
        private readonly UserManager<Users> _userManeger;
        private readonly SignInManager<Users> _signInManeger;
        private readonly Connections _context;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly GenarateToken _tokenGenerator;
        private readonly RoleManager<IdentityRole> _roleManager;

        public adminController(Connections conections, UserManager<Users> userManeger, SignInManager<Users> signInManeger, IConfiguration configuration, IEmailSender emailSender, GenarateToken tokenGenerator, RoleManager<IdentityRole> roleManager)
        {
            _context = conections;
            _userManeger = userManeger;
            _signInManeger = signInManeger;
            _configuration = configuration;
            _emailSender = emailSender;
            _tokenGenerator = tokenGenerator;
            _roleManager = roleManager;
      _roleManager = roleManager;


        }

        public class AdminData
        {
            public string id { get; set; }
            public string token { get; set; }
        }
        
        
        
       
        
    }
}