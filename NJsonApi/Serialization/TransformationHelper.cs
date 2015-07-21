using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using NJsonApi.Common.Infrastructure;
using NJsonApi.Exceptions;

namespace NJsonApi.Serialization
{
    public class TransformationHelper
    {
        public const int RECURSION_DEPTH_LIMIT = 10;

        public CompoundDocument HandleException(Exception exception)
        {
            var scfException = exception is NJsonApiBaseException
                ? exception as NJsonApiBaseException
                : new NJsonApiBaseException(exception.Message, exception);

            var compoundDocument = new CompoundDocument
            {
                Errors = new Dictionary<string, Error>
                {
                    { "error", new Error
                    {
                        Id = scfException.Id.ToString(),
                        Title = scfException.Message,
                        Status = ((int)scfException.GetHttpStatusCode()).ToString(CultureInfo.InvariantCulture),
                    }}
                }
            };

            return compoundDocument;
        }

        public CompoundDocument HandleHttpError(HttpError error)
        {
            return new CompoundDocument
            {
                Errors = new Dictionary<string, Error>
                {
                    { "Internal server error", new Error
                    {
                        Status = error.Message,
                    }}
                }
            };
        }

        public object GetPrimaryResource(object resource, IResourceMapping resourceMapping, string routePrefix)
        {
            var resourcesList = UnifyObjectsToList(resource);

            var result = Enumerable.Select<object, Dictionary<string, object>>(resourcesList, res => CreateResourceRepresentation(res, resourceMapping, routePrefix))
                .ToList();

            if (resource is IEnumerable) return result;

            return result.Count == 1
                ? (object)result.Single()
                : result;
        }

        public Dictionary<string, List<object>> CreateLinkedRepresentation(object resource, IResourceMapping resourceMapping)
        {
            var linkedDictionary = new Dictionary<string, List<object>>();
            var depth = 0;
            CreateLinkedRepresentationInner(resource, resourceMapping, linkedDictionary, ref depth, new HashSet<object>());

            var distinctValues = GetDistinctValues(linkedDictionary);

            var objects = (resource as IList);
            if (objects != null)
            {
                List<string> primaryIds = objects
                    .Cast<object>()
                    .Select(r => resourceMapping.IdGetter(r).ToString())
                    .ToList();

                return RemovePrimaryResource(distinctValues, primaryIds, resourceMapping.ResourceType);
            }


            return distinctValues;
        }

        public Dictionary<string, List<object>> RemovePrimaryResource(Dictionary<string, List<object>> linkedDictionary, List<string> primaryResourceId, string resourceType)
        {
            if (!linkedDictionary.ContainsKey(resourceType))
            {
                return linkedDictionary;
            }

            foreach (var dictItem in linkedDictionary.Where(d => d.Key == resourceType))
            {
                IEnumerable<Dictionary<string, object>> linked = dictItem.Value
                    .Cast<Dictionary<string, object>>()
                    .Where(i => i.ContainsKey("id"))
                    .ToList();

                foreach (var linkedInner in linked)
                {
                    if (primaryResourceId.Contains(linkedInner["id"]))
                    {
                        linkedDictionary[dictItem.Key].Remove(linkedInner);
                    }
                }

            }

            return linkedDictionary;
        }

        public void CreateLinkedRepresentationInner(object resource, IResourceMapping resourceMapping, Dictionary<string, List<object>> linkedDictionary, ref int depth, HashSet<object> hashSet)
        {
            if (resource is IEnumerable)
            {
                if (((IEnumerable)resource).OfType<object>().All(hashSet.Contains))
                {
                    return;
                }
                foreach (var res in (IEnumerable)resource)
                {
                    hashSet.Add(res);
                }
            }
            else
            {
                if (hashSet.Contains(resource))
                {
                    return;
                }
                hashSet.Add(resource);
            }

            depth++;

            foreach (var linkMapping in resourceMapping.Links.Where(l => l.SerializeAsLinked))
            {
                var key = linkMapping.ResourceMapping.ResourceType;
                var nestedObject = GetNestedResource(resource, linkMapping);
                List<object> nestedObjects = UnifyObjectsToList(nestedObject);

                List<object> linkedObjects = nestedObjects
                    .Select(o => (object)CreateLinkedResourceRepresentation(o, linkMapping.ResourceMapping))
                    .ToList();

                MergeResultToDictionary(linkedDictionary, key, linkedObjects);

                if (linkMapping.ResourceMapping.Links.Any() && nestedObjects.Any() && depth < RECURSION_DEPTH_LIMIT)
                {

                    CreateLinkedRepresentationInner(nestedObject, linkMapping.ResourceMapping, linkedDictionary, ref depth, hashSet);
                }
            }
        }

        public object GetNestedResource(object resource, ILinkMapping linkMapping)
        {
            if (resource is IEnumerable<object>)
            {
                var result = new List<object>();
                foreach (var rsx in (resource as IEnumerable<object>))
                {
                    var innerResource = linkMapping.Resource(rsx);
                    if (innerResource != null)
                    {
                        if (innerResource is IEnumerable<object>)
                        {
                            result.AddRange(innerResource as IEnumerable<object>);
                        }
                        else
                        {
                            result.Add(innerResource);
                        }
                    }
                }
                return result;
            }

            return linkMapping.Resource(resource);
        }

        public List<object> UnifyObjectsToList(object nestedObject)
        {
            if (nestedObject == null)
            {
                return new List<object>();
            }

            var list = new List<object>();
            if (nestedObject is IEnumerable<object>)
            {
                list.AddRange((IEnumerable<object>)nestedObject);
            }
            else
            {
                list.Add(nestedObject);
            }
            return list;
        }

        public void MergeResultToDictionary(Dictionary<string, List<object>> targerDictionary, string key, List<object> objects)
        {
            if (targerDictionary.ContainsKey(key))
            {
                targerDictionary[key].AddRange(objects);
            }
            else
            {
                targerDictionary[key] = objects;
            }
        }

        public Dictionary<string, List<object>> GetDistinctValues(Dictionary<string, List<object>> linkedObjectDictionary)
        {
            var resultDictionary = new Dictionary<string, List<object>>();
            foreach (var dictItem in linkedObjectDictionary)
            {
                var distinctObjects = dictItem.Value
                    .Cast<Dictionary<string, object>>()
                    .Where(i => i.ContainsKey("id"))
                    .GroupBy(i => i["id"])
                    .Select(g => (object)g.First())
                    .ToList();

                resultDictionary[dictItem.Key] = distinctObjects;
            }

            return resultDictionary;
        }

        public Dictionary<string, LinkTemplate> CreateLinkRepresentation(IResourceMapping resourceMapping, string routePrefix)
        {
            var links = new Dictionary<string, LinkTemplate>();

            CreateLinkRepresentationInner(resourceMapping, links, routePrefix);

            return links;
        }

        public void CreateLinkRepresentationInner(IResourceMapping resourceMapping, Dictionary<string, LinkTemplate> links, string routePrefix)
        {
            if (resourceMapping.Links.Any())
            {
                var urlBuilder = new UrlBuilder()
                {
                    RoutePrefix = routePrefix
                };

                foreach (var linkMapping in resourceMapping.Links)
                {
                    var linkTemplate = new LinkTemplate
                    {
                        Href = urlBuilder.GetFullyQualifiedUrl(linkMapping.ResourceMapping.UrlTemplate),
                        Type = linkMapping.ResourceMapping.ResourceType
                    };

                    var linkKey = string.Format("{0}.{1}", resourceMapping.ResourceType, linkMapping.LinkName);
                    if (links.ContainsKey(linkKey)) break;

                    links.Add(linkKey, linkTemplate);

                    if (resourceMapping.Links.Any())
                    {
                        CreateLinkRepresentationInner(resourceMapping, links, routePrefix);
                    }
                }
            }
        }

        public void VerifyTypeSupport(Type innerObjectType)
        {
            if (typeof(IMetaDataWrapper).IsAssignableFrom(innerObjectType))
            {
                throw new NotSupportedException(string.Format("Error while serializing type {0}. IEnumerable<MetaDataWrapper<>> is not supported.", innerObjectType));
            }

            if (typeof(IEnumerable).IsAssignableFrom(innerObjectType) && !innerObjectType.IsGenericType)
            {
                throw new NotSupportedException(string.Format("Error while serializing type {0}. Non generic IEnumerable are not supported.", innerObjectType));
            }
        }

        public object GetResourceObject(object objectGraph)
        {
            if (!(objectGraph is IMetaDataWrapper))
            {
                return objectGraph;
            }
            var metadataWrapper = objectGraph as IMetaDataWrapper;
            return metadataWrapper.GetValue();
        }

        public Dictionary<string, object> GetMetadata(object objectGraph)
        {
            if (objectGraph is IMetaDataWrapper)
            {
                var metadataWrapper = objectGraph as IMetaDataWrapper;
                return metadataWrapper.MetaData;
            }
            return new Dictionary<string, object>();
        }

        public Type GetObjectType(object objectGraph)
        {
            Type objectType = objectGraph.GetType();
            if (objectGraph is IMetaDataWrapper)
            {
                objectType = objectGraph.GetType().GetGenericArguments()[0];
            }

            if (typeof(IEnumerable).IsAssignableFrom(objectType) && objectType.IsGenericType)
            {
                objectType = objectType.GetGenericArguments()[0];
            }

            return objectType;
        }

        public Dictionary<string, object> CreateResourceRepresentation(object objectGraph, IResourceMapping resourceMapping, string routePrefix)
        {
            var urlBuilder = new UrlBuilder
            {
                RoutePrefix = routePrefix
            };

            var objectDict = new Dictionary<string, object>();
            var id = resourceMapping.IdGetter(objectGraph).ToString();

            if (resourceMapping.UrlTemplate != null)
                objectDict["href"] = urlBuilder.GetFullyQualifiedUrl(resourceMapping.UrlTemplate.Replace("{id}", id));
            objectDict["id"] = id;
            objectDict["type"] = resourceMapping.ResourceType;

            foreach (var propertyGetter in resourceMapping.PropertyGetters)
            {
                var propertyValue = propertyGetter.Value(objectGraph);
                if (propertyValue != null) objectDict[propertyGetter.Key] = propertyGetter.Value(objectGraph);
            }

            if (resourceMapping.Links.Any())
            {
                var links = CreateLinks(objectGraph, resourceMapping);
                if (Enumerable.Any<KeyValuePair<string, object>>(links))
                {
                    objectDict["links"] = links;
                }
            }

            return objectDict;
        }

        public Dictionary<string, object> CreateLinks(object objectGraph, IResourceMapping resourceMapping)
        {
            var links = new Dictionary<string, object>();
            foreach (var linkMapping in resourceMapping.Links)
            {
                var resourceType = linkMapping.LinkName;
                if (linkMapping.ResourceId != null)
                {
                    var nestedId = linkMapping.ResourceId(objectGraph);
                    if (nestedId != null)
                    {
                        links.Add(resourceType, nestedId.ToString());
                    }
                }
                else
                {
                    if (linkMapping.Resource == null)
                    {
                        continue;
                    }

                    ILinkMapping mapping = linkMapping;

                    IEnumerable<object> resources = UnifyObjectsToList(linkMapping.Resource(objectGraph));
                    var nestedObjectsIds = resources
                        .Select(o => mapping.ResourceMapping.IdGetter(o).ToString())
                        .ToList();

                    if (nestedObjectsIds.Any())
                    {
                        if (!linkMapping.IsCollection)
                        {
                            links.Add(resourceType, nestedObjectsIds.First());
                        }
                        else
                        {
                            links.Add(resourceType, nestedObjectsIds);
                        }
                    }
                }
            }
            return links;
        }

        public Dictionary<string, object> CreateLinkedResourceRepresentation(object objectGraph, IResourceMapping resourceMapping)
        {
            var objectDict = new Dictionary<string, object>();

            objectDict["id"] = resourceMapping.IdGetter(objectGraph).ToString();
            foreach (var propertyGetter in resourceMapping.PropertyGetters)
            {
                objectDict[propertyGetter.Key] = propertyGetter.Value(objectGraph);
            }

            if (resourceMapping.Links.Any())
            {
                var links = CreateLinks(objectGraph, resourceMapping);
                if (links.Any())
                {
                    objectDict["links"] = links;
                }
            }
            return objectDict;
        }

        public void AssureAllMappingsRegistered(Type type, Configuration config)
        {
            if (!config.IsMappingRegistered(type))
            {
                throw new MissingMappingException(type);
            }
        }

        public object GetCollection(JToken value, ILinkMapping mapping)
        {
            IList resultValue;

            if (!(value is JArray))
                throw new InvalidOperationException("Json array expected.");

            try
            {
                var array = (JArray)value;
                var listOfItems = array
                    .ToObject<List<string>>()
                    .Select(id =>
                    {
                        var obj = Activator.CreateInstance(mapping.ResourceMapping.ResourceRepresentationType);
                        mapping.ResourceMapping.IdSetter(obj, id);
                        return obj;
                    });

                resultValue = (IList)Activator.CreateInstance(mapping.CollectionProperty.PropertyType);
                foreach (var item in listOfItems)
                {
                    resultValue.Add(item);
                }
            }
            catch
            {
                resultValue = null;
            }
            return resultValue;
        }

        public object GetValue(JToken value, Type returnType)
        {
            object resultValue;
            try
            {
                if (returnType == typeof(string) && value.Value<string>() == null)
                {
                    resultValue = null;
                }
                else
                {
                    resultValue = Convert.ChangeType(value, returnType);
                }
            }
            catch
            {
                resultValue = default(ValueType);
            }
            return resultValue;
        }
    }
}