using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.HelloWorld.Models
{
    public class Country
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Continent Continent { get; set; }

        public int ContinentId { get; set; }
    }
}
