using Microsoft.AspNet.Mvc;
using NJsonApi.HelloWorld.Models;
using NJsonApi.Infrastructure;
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

        [HttpPost]
        public IActionResult Post([FromBody]Delta<Article> article)
        {
            var newArticle = article.ToObject();
            newArticle.Id = StaticPersistentStore.GetNextId();
            StaticPersistentStore.Articles.Add(newArticle);
            return CreatedAtAction("Get", new { id = newArticle.Id }, newArticle);
        }

        [HttpPatch("{id}")]
        public IActionResult Patch([FromBody]Delta<Article> update, int id)
        {
            var article = StaticPersistentStore.Articles.Single(w => w.Id == id);
            update.ApplySimpleProperties(article);
            return new ObjectResult(article);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            StaticPersistentStore.Articles.RemoveAll(x => x.Id == id);
            return new NoContentResult();
        }
    }
}
