using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceAPI.Entities;
using ResourceAPI.Mappers;

namespace UserAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        UserContext context;
        public ValuesController(UserContext _context)
        {
            context = _context;
        }

        /// <summary>
        /// 以用户名和密码获取用户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [Authorize(Roles = "AuthServer")]
        [HttpGet("{userName}/{password}")]
        public IActionResult AuthUser(string userName, string password)
        {
            var res = context.Users.Where(p => p.UserName == userName && p.Password == password)
                .Include(p=>p.Claims)
                .FirstOrDefault();
            return Ok(res.ToModel());
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user">用户Model</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] ResourceAPI.Models.User user)
        {
            context.Users.Add(user.ToEntity());
            if (context.SaveChanges() > 0)
                return Ok(true);
            else
                return Ok(false);
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Authorize(Roles = "AuthServer")]
        [HttpGet("{id}/{name}/{value}")]
        public ActionResult<IEnumerable<string>> Get(string id, string name, string value)
        {
            return new string[] { id, name, value };
        }

        // GET api/values/5
        [Authorize(Roles = "AuthServer")]
        [HttpGet("{userName}")]
        public ActionResult<string> Get(string userName)
        {
            var res = context.Users.Where(p => p.UserName == userName)
               .Include(p => p.Claims)
               .FirstOrDefault();
            return Ok(res.ToModel());
        }


        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
