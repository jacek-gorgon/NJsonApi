using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace NJsonApi
{
    public interface IConfiguration
    {
        MediaTypeHeaderValue DefaultJsonApiMediaType { get; }

        void AddMapping(IResourceMapping resourceMapping);
        void Apply(IServiceCollection services);
        IResourceMapping GetMapping(Type type);
        IResourceMapping GetMapping(object objectGraph);
        bool IsMappingRegistered(Type type);
        bool ValidateIncludedRelationshipPaths(string[] includedPaths, object objectGraph);
    }
}