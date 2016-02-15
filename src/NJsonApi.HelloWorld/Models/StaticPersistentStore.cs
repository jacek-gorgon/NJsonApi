using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.HelloWorld.Models
{
    /// <summary>
    /// Primitive backing store for persistence.
    /// </summary>
    public static class StaticPersistentStore
    {
        public static List<World> Worlds { get; set; }
        public static List<Continent> Continents { get; set; }

        static StaticPersistentStore()
        {
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
                World = w,
                WorldId = 1
            };

            var c2 = new Continent
            {
                Id = 2,
                Name = "Hello America",
                World = w,
                WorldId = 1
            };

            var c3 = new Continent
            {
                Id = 3,
                Name = "Hello Asia",
                World = w,
                WorldId = 1
            };

            w.Continents.Add(c1);
            w.Continents.Add(c2);
            w.Continents.Add(c3);

            Worlds = new List<World> { w };
            Continents = new List<Continent> { c1, c2, c3 };
        }

        public static int GetNextId()
        {
            return Worlds.Max(w => w.Id) + 1;
        }
    }
}