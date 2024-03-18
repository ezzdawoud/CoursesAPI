﻿using Courses.Data;
using Courses.Helper;
using Courses.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly UserManager<Users> _userManeger;
        private readonly Connections _context;
        private readonly IConfiguration _configuration;
        private readonly GenarateToken _tokenGenerator;
        public EnrollmentController(Connections conections, UserManager<Users> userManeger, IConfiguration configuration, GenarateToken tokenGenerator)
        {
            _context = conections;
            _userManeger = userManeger;
            _configuration = configuration;
            _tokenGenerator = tokenGenerator;


        }
  
        // POST api/<EnrollmentController>
        [HttpPost("Enrollment{token},{id},{courseId},{CardId}")]
        public async Task<IActionResult> Post(string token, string id, int courseId, int CardId)
        {
            
            Users user = await _tokenGenerator.GetUserFromToken(token);
            if (user == null)
            {
                return NotFound("User Not Found");
            }
            else if (user.Id == id)
            {
                Cards Card = _context.Cards.Where(m => m.CardId == CardId).FirstOrDefault();
                Models.Courses course = _context.Courses.Where(m => m.courseId == courseId).FirstOrDefault();

                if (Card == null)
                {
                    return NotFound("Card Not Found");
                }
                if (course == null)
                {
                    return NotFound("course Not Found");

                }
                if (Card.CardValue >= course.CouresValue)
                {
                    Card.CardValue -= course.CouresValue;
                    Card.userId = user.Id;
                    // Save the changes to the database
                    _context.SaveChanges();
                    var newEnrollment = new Enrollment { CouresId = course.courseId, UserId = user.Id };
                    _context.Enrollments.Add(newEnrollment);
                    _context.SaveChanges();
                    return Ok("Course Buy Successfully");
                }
                else
                {
                    return BadRequest("Card Value");
                }
            }
            else
            {
                return BadRequest("you dont have accsess");
            }

        }

      
    }
}