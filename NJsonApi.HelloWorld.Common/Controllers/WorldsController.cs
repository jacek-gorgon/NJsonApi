using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using NJsonApi.HelloWorld.Common.Models;

namespace NJsonApi.HelloWorld.Common.Controllers
{
    public class WorldsController : ApiController
    {
        // GET api/worlds
        public IEnumerable<World> Get()
        {
            return StaticPersistentStore.Worlds;
        }

        // GET api/worlds
        public World Get(int id)
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
    }
}
