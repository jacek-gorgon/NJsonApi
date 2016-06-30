﻿using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using NJsonApi.HelloWorld.Common.Models;
using NJsonApi.Common.Infrastructure;

namespace NJsonApi.HelloWorld.Common.Controllers
{
    [RoutePrefix("worlds")]
    public class WorldsController : ApiController
    {
        [HttpGet, Route]
        public IEnumerable<World> Get()
        {
            return StaticPersistentStore.Worlds;
        }

        [HttpGet, Route("{id}")]
        public World Get([FromUri]int id)
        {
            try
            {
                return StaticPersistentStore.Worlds.Single(w => w.Id == id);
            }
            catch
            {
                throw new NotFoundNJsonApiException("No world with the id of " + id);
            }
        }

        [HttpPost, Route]
        public World Post([FromBody]Delta<World> worldDelta)
        {
            var world = worldDelta.ToObject();
            world.Id = StaticPersistentStore.Worlds.Max(w => w.Id) + 1;
            StaticPersistentStore.Worlds.Add(world);
            return world;
        }

        [HttpPut, Route("{id}")]
        public World Put([FromBody]Delta<World> worldDelta, [FromUri]int id)
        {
            var world = Get(id);
            worldDelta.Apply(world);
            return world;
        }
    }
}
