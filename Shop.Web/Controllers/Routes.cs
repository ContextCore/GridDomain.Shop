using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Controllers
{
    public static class Routes
    {
        public static class Api
        {
            public static class User
            {
                public const string Controller = "/api/user/";
                public const string Create = "create";
            }

            public static class SampleData
            {
                public const string Controller = "/api/SampleData/";
                public const string WeatherForecasts = "WeatherForecasts";
            }
        }
    }
}
