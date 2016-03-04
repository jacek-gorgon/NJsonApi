using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApiExplorer;
using Microsoft.AspNet.Routing.Template;
using NJsonApi.Extensions;
using NJsonApi.Serialization.Representations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Serialization
{
    internal class LinkBuilder : ILinkBuilder
    {
        private readonly IApiDescriptionGroupCollectionProvider descriptionProvider;
        private readonly IUrlHelper urlHelper;

        public LinkBuilder(IApiDescriptionGroupCollectionProvider descriptionProvider,
            IUrlHelper urlHelper)
        {
            this.descriptionProvider = descriptionProvider;
            this.urlHelper = urlHelper;
        }

        public ILink FindResourceSelfLink(Context context, string resourceId, IResourceMapping resourceMapping)
        {
            var actions = descriptionProvider.From(resourceMapping.Controller).Items;

            var action = actions.Single(a =>
                a.HttpMethod == "GET" &&
                a.ParameterDescriptions.Count(p => p.Name == "id") == 1);

            var values = new Dictionary<string, object>();
            values.Add("id", resourceId);

            return ToUrl(context, action, values);
        }

        public ILink RelationshipRelatedLink(Context context, string resourceId, IResourceMapping resourceMapping, IRelationshipMapping linkMapping)
        {
            var selfLink = FindResourceSelfLink(context, resourceId, resourceMapping).Href;
            var completeLink = $"{selfLink}/{linkMapping.RelationshipName}";
            return new SimpleLink(new Uri(completeLink));
           
        }

        public ILink RelationshipSelfLink(Context context, string resourceId, IResourceMapping resourceMapping, IRelationshipMapping linkMapping)
        {
            var selfLink = FindResourceSelfLink(context, resourceId, resourceMapping).Href;
            var completeLink = $"{selfLink}/relationships/{linkMapping.RelationshipName}";
            return new SimpleLink(new Uri(completeLink));
        }

        // TODO replace with UrlHelper method once RC2 has been released
        private SimpleLink ToUrl(Context context, ApiDescription action, Dictionary<string, object> values)
        {
            var template = TemplateParser.Parse(action.RelativePath);
            var result = action.RelativePath.ToLowerInvariant();

            foreach (var parameter in template.Parameters)
            {
                var value = values[parameter.Name];
                result = result.Replace(parameter.ToPlaceholder(), value.ToString());
            }

            return new SimpleLink(new Uri(context.BaseUri, result));
        }
    }
}
