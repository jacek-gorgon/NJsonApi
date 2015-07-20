using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using NJsonApi.HelloWorld.Models;

namespace NJsonApi.HelloWorld.Controllers
{
    public class WorldsController : ApiController
    {
        /// <summary>
        /// Primitive backing store for persistence.
        /// </summary>
        private static List<World> Worlds { get; set; }

        static WorldsController()
        {
            // Primitive data seed

            var w = new World
            {
                Id = 1,
                Name = "Hello",
                Continents = new List<Continent>()
            };

            var c1 = new Continent
            {
                Id = 1,
                Name = "Hello Europe",
                //World = w,
                WorldId = 1
            };

            var c2 = new Continent
            {
                Id = 2,
                Name = "Hello America",
                //World = w,
                WorldId = 1
            };

            var c3 = new Continent
            {
                Id = 3,
                Name = "Hello Asia",
                //World = w,
                WorldId = 1
            };

            w.Continents.Add(c1);
            w.Continents.Add(c2);
            w.Continents.Add(c3);

            Worlds = new List<World>();
            Worlds.Add(w);
        }

        public WorldsController()
        {
        }

        // GET api/worlds
        public IEnumerable<World> Get()
        {
            return Worlds;
        }

        // GET api/worlds
        public World Get(int id)
        {
            try
            {
                return Worlds.Single(w => w.Id == id);
            }
            catch
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}
