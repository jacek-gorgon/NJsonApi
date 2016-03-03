using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;

namespace NJsonApi
{
    public interface IConfiguration
    {
        MediaTypeHeaderValue DefaultJsonApiMediaType { get; }
        
        void AddMapping(IResourceMapping resourceMapping);
        void Apply(IServiceCollection services);
        IResourceMapping GetMapping(Type type);
        IResourceMapping GetMapping(object objectGraph);
        IEnumerable<IResourceMapping> All();
        bool IsMappingRegistered(Type type);
        bool ValidateIncludedRelationshipPaths(string[] includedPaths, object objectGraph);
    }
}