using System.Collections.Generic;
using System.Linq;
using NJsonApi.Common.Infrastructure;
using NJsonApi.HelloWorld.Dnx.Models;
using Microsoft.AspNet.Mvc;

namespace NJsonApi.HelloWorld.Dnx.Controllers
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
        public World Get(int id)
        {
            return StaticPersistentStore.Worlds.Single(w => w.Id == id);
        }

        [HttpPost]
        public World Post([FromBody]Delta<World> worldDelta)
        {
            var world = worldDelta.ToObject();
            world.Id = StaticPersistentStore.Worlds.Max(w => w.Id) + 1;
            StaticPersistentStore.Worlds.Add(world);
            return world;
        }

        [HttpPut("{id}")]
        public World Put([FromBody]Delta<World> worldDelta, int id)
        {
            var world = Get(id);
            worldDelta.Apply(world);
            return world;
        }
    }
}
