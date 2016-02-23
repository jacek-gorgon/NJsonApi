using Microsoft.AspNet.Mvc;
using NJsonApi.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.HelloWorld.Controllers
{
    [Route("[controller]")]
    public class PeopleController : Controller
    {
        [HttpGet]
        public IEnumerable<Comment> Get()
        {
            return StaticPersistentStore.Comments;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new ObjectResult(StaticPersistentStore.Comments.Single(w => w.Id == id));
        }
    }
}
