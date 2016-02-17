using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NJsonApi.Test.TestModel
{
    internal class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public IList<Post> Posts { get; set; }
        public Author()
        {
            Posts = new List<Post>();
        }
    }
}
