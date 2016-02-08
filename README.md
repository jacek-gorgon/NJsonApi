# NJsonApi
The .NET server implementation of the {**json:api**} standard. 

> The library is in the process of upgrading to spec version 1.0. Some features are not yet compatibile with the current version of spec!

Spawned as an internal project used in production environment, the package is now available in the open thanks to courtesy of [**SocialCee**](http://socialcee.com)! Further development, including updating to {**json:api**} 1.0 will take place here.
## Quick start
After installing the package:
```
PM> Install-Package NJsonApi
```

... given the two POCOs:
```cs
    public class World
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Continent> Continents { get; set; }
    }

	public class Continent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public World World { get; set; }
        public int WorldId { get; set; }
    }
```

... and the **NJsonApi** mapping and bootstrapping:
```cs
	var configBuilder = new ConfigurationBuilder();

	configBuilder
		.Resource<World>()
		.WithAllProperties()
		.WithLinkTemplate("/worlds/{id}");

	configBuilder
		.Resource<Continent>()
		.WithAllProperties()
		.WithLinkTemplate("/continents/{id}");

	var nJsonApiConfig = configBuilder.Build();
	nJsonApiConfig.Apply(httpConfiguration);
```

... a standard Web API controller method:
```cs
	[HttpGet, Route]
	public IEnumerable<World> Get()
	{
		return new List<World>() { ... };
	}
```

... starts returning the {**json:api**} compliant JSON:
```json
{
  "data": {
    "id": "1",
    "type": "worlds",
    "attributes": {
      "name": "Hello"
    },
    "relationships": {
      "continents": {
        "data": [
          {
            "id": "1",
            "type": "continents"
          },
          {
            "id": "2",
            "type": "continents"
          },
          {
            "id": "3",
            "type": "continents"
          }
        ],
        "meta": {
          "count": "3"
        }
      }
    },
    "links": {
      "self": "http://localhost:56827/worlds/1"
    }
  },
  "included": [
    {
      "id": "1",
      "type": "continents",
      "attributes": {
        "name": "Hello Europe",
        "worldId": 1
      },
      "relationships": {
        "world": {
          "data": {
            "id": "1",
            "type": "worlds"
          }
        }
      },
      "links": {
        "self": "http://localhost:56827/continents/1"
      }
    },
    {
      "id": "2",
      "type": "continents",
      "attributes": {
        "name": "Hello America",
        "worldId": 1
      },
      "relationships": {
        "world": {
          "data": {
            "id": "1",
            "type": "worlds"
          }
        }
      },
      "links": {
        "self": "http://localhost:56827/continents/2"
      }
    },
    {
      "id": "3",
      "type": "continents",
      "attributes": {
        "name": "Hello Asia",
        "worldId": 1
      },
      "relationships": {
        "world": {
          "data": {
            "id": "1",
            "type": "worlds"
          }
        }
      },
      "links": {
        "self": "http://localhost:56827/continents/3"
      }
    }
  ]
}
```

## Sample projects included
NJsonApi can be hosted in IIS (see the NJsonApi.HelloWorld example) or OWIN/Katana hosted in an example console application (NJsonApi.Console.Katana).

Use a browser and explore the related resource thanks to HATEOAS or run Fiddler and load the included session file for a full test bench.
