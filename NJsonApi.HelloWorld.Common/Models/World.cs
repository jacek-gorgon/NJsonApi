using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NJsonApi.HelloWorld.Common.Models
{
    public class World
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Continent> Continents { get; set; }
    }
}