using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Entitati;
using WebApplication.Payloads;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WebApplicationContext _db;
        private readonly ILogger<WeatherForecastController> _logger;

       
        public WeatherForecastController(WebApplicationContext db, ILogger<WeatherForecastController> logger)
        {
            _db =db;
            _logger = logger;
        }
        [HttpGet("getall")]
        public ActionResult<List<Users>> GetAllUsers()
        {
            return _db.Users.ToList();
        }
        [HttpGet]
        //public ActionResult<Users>GetById(int Id)
        //{
          //  return _db.Users.Where(user => Id == user.Id).Single();
        //}
        [HttpPost("create")]
        public ActionResult<Users>Create([FromBody]UsersPayload payload)
        {
            try
            {
                var userToAdd = new Users
                {
                    Name = payload.Name,
                    Pass = payload.Pass,
                };
                _db.Users.Add(userToAdd);
                _db.SaveChanges();

                return userToAdd;
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                
            }
        }
        [HttpPost("update")]
        public ActionResult<Users> Update([FromBody] UsersPayload payload)
        {
            try
            {
                if (payload.Id.HasValue)
                {
                    var userToUpdate = _db.Users.Single(user => payload.Id.Value == user.Id);
                    userToUpdate.Name = payload.Name;
                    userToUpdate.Pass = payload.Pass;
                    _db.SaveChanges();
                    return userToUpdate;
                }
                else return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            catch (Exception)
            {

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }


        }
        [HttpDelete("delete")]
        public ActionResult Delete(int Id)
        {
            try
            {
                var userToDelete = _db.Users.Single(User => Id == User.Id);
                _db.Users.Remove(userToDelete);
                _db.SaveChanges();
                return new JsonResult(new { status = true });
            }
            catch (Exception)
            {

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }
        
    }
}
