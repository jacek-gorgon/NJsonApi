using System.Collections.Generic;
using Newtonsoft.Json;

namespace UtilJsonApiSerializer.Test.TestModel
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IList<Comment> Replies { get; set; }
        public Author Author { get; set; }
        public int AuthorId { get; set; }

        [JsonIgnore]
        public int InternalNumber { get; set; }

        public Post()
        {
            Replies = new List<Comment>();
        }
    }
}
