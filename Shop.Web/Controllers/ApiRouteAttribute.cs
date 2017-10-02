using Microsoft.AspNetCore.Mvc;

namespace Shop.Web.Controllers
{
    public class ApiRouteAttribute : RouteAttribute
    {
        public ApiRouteAttribute(string pathToController = null):base(pathToController == null ? "api/[controller]":$"api/{pathToController}/[controller]")
        {
        
        }
    }
}