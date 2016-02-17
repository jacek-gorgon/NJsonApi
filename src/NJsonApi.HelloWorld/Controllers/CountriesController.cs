using Microsoft.AspNet.Mvc;
using NJsonApi.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.HelloWorld.Controllers
{
    [Route("api/[controller]")]
    public class CountriesController : Controller
    {
        [HttpGet]
        public IEnumerable<Country> Get()
        {
            return StaticPersistentStore.Countries;
        }

        [HttpGet("{id}")]
        public Country Get(int id)
        {
            return StaticPersistentStore.Countries.Single(c => c.Id == id);
        }
    }
}
