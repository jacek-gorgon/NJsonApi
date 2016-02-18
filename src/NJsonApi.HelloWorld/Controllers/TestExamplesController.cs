using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.HelloWorld.Controllers
{
    [Route("api/[controller]")]
    public class TestExamplesController : Controller
    {
        [HttpGet]
        [Route("[action]")]
        public void ThrowException()
        {
            throw new NotImplementedException("An example exception thrown");
        }
    }
}
