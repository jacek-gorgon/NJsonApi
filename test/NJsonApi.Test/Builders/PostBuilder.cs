using NJsonApi.Test.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Test.Builders
{
    internal class PostBuilder
    {
        private readonly Post post;

        public static Author Asimov
        {
            get
            {
                return new Author()
                {
                    Id = 1,
                    Name = "Isaac Asimov"
                };
            }
        }

        public PostBuilder()
        {
            this.post = new Post()
            {
                Id = 1,
                Title = "Post Title 1",
            };
        }

        public PostBuilder WithAuthor(int authorId, string authorName)
        {
            post.AuthorId = authorId;
            post.Author = new Author()
            {
                Id = authorId,
                Name = authorName,
                DateTimeCreated = new DateTime(2000, 01, 01)
            };
            return this;
        }

        public PostBuilder WithAuthor(Author author)
        {
            post.Author = author;
            return this;
        }

        public PostBuilder WithComment(int id, string body)
        {
            var comment = new Comment()
            {
                Body= body,
                Id = id,
                Post = post
            };

            post.Replies.Add(comment);
            return this;
        }

        public Post Build()
        {
            return post;
        }
        
    }
}
