using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.HelloWorld.Dnx.Models
{
    public class World
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Continent> Continents { get; set; }
    }
}