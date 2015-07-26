using System.Collections.Generic;
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
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
        }

        [HttpPost, Route]
        public void Post([FromBody]World world)
        {
            world.Id = StaticPersistentStore.Worlds.Max(w => w.Id) + 1;
            StaticPersistentStore.Worlds.Add(world);
        }

        [HttpPut, Route("{id}")]
        public void Put([FromBody]Delta<World> w, [FromUri]int id)
        {
            var world = Get(id);
            w.Apply(world);
        }
    }
}
