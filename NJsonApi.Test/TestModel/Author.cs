using System;
using System.Collections.Generic;

namespace SocialCee.Framework.NJsonApi.Test.TestModel
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public Array Objects { get; set; }
        public Comment[] Comments { get; set; }
        public IList<Post> Posts { get; set; }

        public Author()
        {
            Posts = new List<Post>();
            Comments = new Comment[1];
        }
    }
}
