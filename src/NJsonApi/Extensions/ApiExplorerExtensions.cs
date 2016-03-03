using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.ApiExplorer;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Routing.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApi.Extensions
{
    internal static class ApiExplorerExtensions
    {
        public static ApiDescriptionGroup From(this IApiDescriptionGroupCollectionProvider provider, Type controller)
        {
            foreach (var actionGroup in provider.ApiDescriptionGroups.Items)
            {
                foreach(var action in actionGroup.Items)
                {
                    var controllerAction = action.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerAction == null)
                    {
                        continue;
                    }

                    if (controllerAction.ControllerTypeInfo.FullName == controller.FullName)
                    {
                        return actionGroup;
                    }
                }
            }
            return null;
        }

        public static string ToPlaceholder(this TemplatePart part)
        {
            if (part.IsParameter)
            {
                return "{" + (part.IsCatchAll ? "*" : string.Empty) + part.Name + (part.IsOptional ? "?" : string.Empty) + "}";
            }
            else
            {
                return part.Text;
            }
        }

    }
}
