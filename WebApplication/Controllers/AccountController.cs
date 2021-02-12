using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Entitati;
using WebApplication.Payloads;
using static WebApplication.Enums;
using BC = BCrypt.Net.BCrypt;
namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly WebApplicationContext _db;
        private IConfiguration _config { get; }
        public AccountController(WebApplicationContext db,IConfiguration configuration)
        {
            _config = configuration;
            _db = db;

        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterPayload registerPayload)
        {
            try
            {
                var existingUser = _db.Users
                    .Any(u => u.Email == registerPayload.Email);
                if(existingUser)
                {
                    return Ok(new { status = false, message = "inca un cont la fel" });
                }
                var userToCreate = new Users
                {
                    Name = registerPayload.Name,
                    Pass = registerPayload.Pass,
                    Email = registerPayload.Email,
                    PassHash = BC.HashPassword(registerPayload.Pass),
                };
                _db.Users.Add(userToCreate);
                
                _db.SaveChanges();
                return Ok(new { status = true, message = " mere ba" });
            }
            catch (Exception e)
            {

                return new JsonResult(new { eroare = e.Message });
            }
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginPayload loginPayload)
        {
            var foundUser = _db.Users
                .SingleOrDefault(u => u.Name == loginPayload.Name);
          if (foundUser != null)
            {
                if (BC.Verify(loginPayload.Pass,foundUser.PassHash))
                {
                    string tokenString = GenerateJSONWebToken(foundUser);
                        return Ok(new { status = true, token = tokenString });
                }
                return BadRequest(new { status = false, message = "NU mere pass sau nume" });
            }

            else
            {
                return BadRequest(new { status = false, message = "NU mere" });
            }
        }
            private string GenerateJSONWebToken(Users user)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim("Id",user.Id.ToString())
            };
                var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                  _config["Jwt:Issuer"],
                  claims,
                  expires: DateTime.Now.AddDays(10),
                  signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

        [Authorize]
        [HttpPost("post")]

        public async Task<IActionResult> Posts([FromBody] PostPayload postPayload)
        {

            if (HttpContext.User.HasClaim(claim => claim.Type == "Id")) {
                int Id;
                Id=int.Parse(HttpContext.User.Claims.FirstOrDefault(Claim => Claim.Type == "Id").Value);
                try
                {
                    var newPost = new Post
                    {
                        Title = postPayload.Title,
                        Text = postPayload.Text,
                        UserId = Id
                        
                    };
                    _db.Posts.Add(newPost);
                    _db.SaveChanges();
                    return Ok(new { status = true, message = " mere ba" });
                }
                catch (Exception e)
                {

                    return new JsonResult(new { eroare = e.Message });
                }
            }
            return new JsonResult(new { eroare ="Muie ba" });
        }
        [HttpGet("getallPost")]
        public ActionResult<List<Post>> GetAllPost()
        {

            return _db.Posts.ToList();

        }


    } 
    }

