using Microsoft.AspNet.Mvc.ApiExplorer;
using Microsoft.AspNet.Mvc.Controllers;
using NJsonApi.Test.TestControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace NJsonApi.Test.Fakes
{
    public class FakeApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
    {
        private readonly ApiDescriptionGroupCollection groupsCollection;
        private readonly List<ApiDescriptionGroup> groups;
        private readonly List<ApiDescription> actions;
        private ApiDescriptionGroup group;

        public FakeApiDescriptionGroupCollectionProvider()
        {
            groups = new List<ApiDescriptionGroup>();
            groupsCollection = new ApiDescriptionGroupCollection(groups, 1);
            actions = new List<ApiDescription>();
        }

        public FakeApiDescriptionGroupCollectionProvider WithPostsController()
        {
            group = new ApiDescriptionGroup("posts", actions);
            groups.Add(group);
            return this;
        } 

        public FakeApiDescriptionGroupCollectionProvider WithGetAction()
        {
            var action = new ApiDescription()
            {
                GroupName = "posts",
                HttpMethod = "GET",
                RelativePath = "posts/{id}",
                ActionDescriptor = new ControllerActionDescriptor()
                {
                    ControllerTypeInfo = typeof(PostsController).GetTypeInfo()
                }
            };

            action.ParameterDescriptions.Add(new ApiParameterDescription()
            {
                Name = "id"
            });
            actions.Add(action);
            return this;
        }

        public ApiDescriptionGroupCollection ApiDescriptionGroups => groupsCollection;
    }
}
