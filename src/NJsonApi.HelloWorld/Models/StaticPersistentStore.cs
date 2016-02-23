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
        private static int currentId { get; set; }

        public static List<Article> Articles { get; set; }

        public static List<Person> People { get; set; }

        public static List<Comment> Comments { get; set; }

        static StaticPersistentStore()
        {
            currentId = 1;

            Articles = new List<Article>();
            People = new List<Person>();
            Comments = new List<Comment>();

            var article1 = new Article("JSON API paints my bikeshed!");
            var article2 = new Article("JSON API makes the tea!");

            var person1 = new Person("Dan", "Gebhardt", "dgeb");
            var person2 = new Person("Rob", "Lang", "brainwipe");

            var comment1 = new Comment("First!");
            var comment2 = new Comment("I like XML Better");

            article1.Author = person1;
            article1.Comments.Add(comment1);
            article1.Comments.Add(comment2);
            Articles.Add(article1);

            article2.Author = person2;
            Articles.Add(article2);

            People.Add(person1);
            People.Add(person2);

            comment1.Author = person1;
            comment2.Author = person2;

            Comments.Add(comment1);
            Comments.Add(comment2);
        }

        /*
        static StaticPersistentStore()
        {
            var w = new World
            {
                Id = 1,
                Name = "Hello",
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

            var co1 = new Country
            {
                Id = 1,
                Name = "United Kingdom",
                Continent = c1,
                ContinentId = c1.Id
            };

            var co2 = new Country
            {
                Id = 2,
                Name = "France",
                Continent = c1,
                ContinentId = c1.Id
            };

            var co3 = new Country
            {
                Id = 3,
                Name = "United States of America",
                Continent = c2,
                ContinentId = c2.Id
            };

            var co4 = new Country
            {
                Id = 4,
                Name = "Canada",
                Continent = c2,
                ContinentId = c2.Id
            };

            var co5 = new Country
            {
                Id = 5,
                Name = "Japan",
                Continent = c3,
                ContinentId = c3.Id
            };

            var co6 = new Country
            {
                Id = 6,
                Name = "China",
                Continent = c3,
                ContinentId = c3.Id
            };

            w.Continents.Add(c1);
            w.Continents.Add(c2);
            w.Continents.Add(c3);

            c1.Countries.Add(co1);
            c1.Countries.Add(co2);
            c2.Countries.Add(co3);
            c2.Countries.Add(co4);
            c3.Countries.Add(co5);
            c3.Countries.Add(co6);

            Worlds = new List<World> { w };
            Continents = new List<Continent> { c1, c2, c3 };
            Countries = new List<Country> { co1, co2, co3, co4, co5, co6 };
        }*/

        public static int GetNextId()
        {
            return currentId++;
        }
    }
}