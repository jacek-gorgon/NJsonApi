using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.HelloWorld.Models
{
    public class Continent
    {
        public Continent()
        {
            Countries = new List<Country>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public World World { get; set; }
        public int WorldId { get; set; }
        public List<Country> Countries { get; set; }
    }
}