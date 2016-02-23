using Microsoft.AspNet.Mvc;
using NJsonApi.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.HelloWorld.Controllers
{
    [Route("[controller]")]
    public class ArticlesController : Controller
    {
        [HttpGet]
        public IEnumerable<Article> Get()
        {
            return StaticPersistentStore.Articles;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new ObjectResult(StaticPersistentStore.Articles.Single(w => w.Id == id));
        }
    }
}
