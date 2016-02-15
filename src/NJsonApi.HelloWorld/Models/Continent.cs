using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.HelloWorld.Models
{
    public class Continent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public World World { get; set; }
        public int WorldId { get; set; }
    }
}