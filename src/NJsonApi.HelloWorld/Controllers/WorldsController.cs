using System.Collections.Generic;
using System.Linq;
using NJsonApi.Infrastructure;
using NJsonApi.HelloWorld.Models;
using Microsoft.AspNet.Mvc;

namespace NJsonApi.HelloWorld.Controllers
{
    [Route("api/[controller]")]
    public class WorldsController : Controller
    {
        [HttpGet]
        public IEnumerable<World> Get()
        {
            return StaticPersistentStore.Worlds;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new ObjectResult(StaticPersistentStore.Worlds.Single(w => w.Id == id));
        }

        [HttpPost]
        public IActionResult Post([FromBody]Delta<World> worldDelta)
        {
            var world = worldDelta.ToObject();
            world.Id = StaticPersistentStore.GetNextId();
            StaticPersistentStore.Worlds.Add(world);

            return new CreatedAtRouteResult(new { id = world.Id  }, world);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody]Delta<World> worldDelta, int id)
        {
            var world = StaticPersistentStore.Worlds.Single(w => w.Id == id);
            worldDelta.Apply(world);
            return new NoContentResult();
        }
    }
}
