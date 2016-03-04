# NJsonApi
The .NET server implementation of the {**json:api**} standard running on .NET Core 1.0 (aka ASP.NET 5, MVC 6, DNX/vNext/OWIN).

> This library is not a complete implementation of the JSONApi 1.0 specification and is under heavy development.

## History
Originally courtesy of [**SocialCee**](http://socialcee.com) and forked from the work done by https://github.com/jacek-gorgon/NJsonApi

## How to use
There is currently no nuget package. You will need to download the code and build the NJsonApi.sln yourself, the nuget package is not part of this branch. 

A HelloWorld project (running ASP.NET Core 1.0) implements the sample below.

Unit tests are written using xUnit.

## Example
Using the same entities found on the [JSONApi homepage](http://jsonapi.org/). 

```cs
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Person> Author { get; set; }
        public List<Comment> Comments { get; set}
    }

	  public class Person
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Twitter { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
    }

```

During Startup, the entities are registered with the `ConfigurationBuilder` to build a Configuration. This is done in the ASP.NET Core method: `ConfigureServices(IServiceCollection services)`.

```cs
	var configBuilder = new ConfigurationBuilder();

	configBuilder
      .Resource<Article, ArticlesController>()
      .WithAllProperties();

  configBuilder
      .Resource<Person, PeopleController>()
      .WithAllProperties();

  configBuilder
      .Resource<Comment, CommentsController>()
      .WithAllProperties();

	var nJsonApiConfig = configBuilder.Build();
	nJsonApiConfig.Apply(httpConfiguration);
```

The Controller method requires no additional attributes or markup:

```cs
	[Route("[controller]")]
	public IEnumerable<Article> Get()
	{
		return new List<Article>() { ... };
	}
```

A GET request to `http://localhost:5000/articles/1?include=comments.people` with header `application/vnd.api+json` gives the compound document:

```json
{
  "data": {
    "id": "1",
    "type": "articles",
    "attributes": {
      "title": "JSON API paints my bikeshed!"
    },
    "relationships": {
      "author": {
        "links": {
          "self": "http://localhost:5000/articles/1/relationships/author",
          "related": "http://localhost:5000/articles/1/author"
        },
        "data": {
          "id": "3",
          "type": "people"
        }
      },
      "comments": {
        "links": {
          "self": "http://localhost:5000/articles/1/relationships/comments",
          "related": "http://localhost:5000/articles/1/comments"
        },
        "data": [
          {
            "id": "5",
            "type": "comments"
          },
          {
            "id": "6",
            "type": "comments"
          }
        ],
        "meta": {
          "count": "2"
        }
      }
    },
    "links": {
      "self": "http://localhost:5000/articles/1"
    }
  },
  "links": {
    "self": "http://localhost:5000/articles/1?include=comments.people"
  },
  "included": [
    {
      "id": "3",
      "type": "people",
      "attributes": {
        "firstName": "Dan",
        "lastName": "Gebhardt",
        "twitter": "dgeb"
      },
      "links": {
        "self": "http://localhost:5000/people/3"
      }
    },
    {
      "id": "5",
      "type": "comments",
      "attributes": {
        "body": "First!"
      },
      "relationships": {
        "author": {
          "links": {
            "self": "http://localhost:5000/comments/5/relationships/author",
            "related": "http://localhost:5000/comments/5/author"
          },
          "data": {
            "id": "3",
            "type": "people"
          }
        }
      },
      "links": {
        "self": "http://localhost:5000/comments/5"
      }
    },
    {
      "id": "6",
      "type": "comments",
      "attributes": {
        "body": "I like XML Better"
      },
      "relationships": {
        "author": {
          "links": {
            "self": "http://localhost:5000/comments/6/relationships/author",
            "related": "http://localhost:5000/comments/6/author"
          },
          "data": {
            "id": "4",
            "type": "people"
          }
        }
      },
      "links": {
        "self": "http://localhost:5000/comments/6"
      }
    },
    {
      "id": "4",
      "type": "people",
      "attributes": {
        "firstName": "Rob",
        "lastName": "Lang",
        "twitter": "brainwipe"
      },
      "links": {
        "self": "http://localhost:5000/people/4"
      }
    }
  ],
  "jsonapi": {
    "Version": "1.0"
  }
}
```
