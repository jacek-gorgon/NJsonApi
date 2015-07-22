using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using NJsonApi.HelloWorld.Common.Models;

namespace NJsonApi.HelloWorld.Common.Controllers
{
    public class ContinentsController : ApiController
    {
        // GET api/worlds
        public IEnumerable<Continent> Get()
        {
            return StaticPersistentStore.Continents;
        }

        // GET api/worlds
        public Continent Get(int id)
        {
            try
            {
                return StaticPersistentStore.Continents.Single(w => w.Id == id);
            }
            catch
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}
