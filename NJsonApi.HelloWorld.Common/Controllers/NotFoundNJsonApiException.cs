using System.Net;
using NJsonApi.Common.Infrastructure;

namespace NJsonApi.HelloWorld.Common.Controllers
{
   public class NotFoundNJsonApiException : NJsonApiBaseException
   {
      public NotFoundNJsonApiException(string message)
         :base(message)
      {
         
      }
      public override HttpStatusCode GetHttpStatusCode() => HttpStatusCode.NotFound;
   }
}