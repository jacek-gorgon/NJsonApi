# NJsonApi
.NET server implementation of the {**json:api**} standard.
## Quick start
Given the two POCOs:
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

... starts returning the {**json:api**} compliant* JSON:
```json
{
  "links": {
    "worlds.continents": {
      "href": "http://localhost:56827/continents/{id}",
      "type": "continents"
    }
  },
  "linked": {
    "continents": [
      {
        "id": "1",
        "name": "Hello Europe",
        "worldId": 1,
        "links": {
          "world": "1"
        }
      },
      {
        "id": "2",
        "name": "Hello America",
        "worldId": 1,
        "links": {
          "world": "1"
        }
      },
      {
        "id": "3",
        "name": "Hello Asia",
        "worldId": 1,
        "links": {
          "world": "1"
        }
      }
    ],
    "worlds": []
  },
  "worlds": [
    {
      "href": "http://localhost:56827/worlds/1",
      "id": "1",
      "type": "worlds",
      "name": "Hello",
      "links": {
        "continents": [
          "1",
          "2",
          "3"
        ]
      }
    }
  ]
}
```

* the output complies with an outdated RC1 version, bringing it up to version 1.0 is in the TODO

## Sample project included
Play around with the working NJsonApi.HelloWorld sample project.

Use a browser and explore the related resource thanks to HATEOAS...

... or run Fiddler and load the included session file for a full test bench.