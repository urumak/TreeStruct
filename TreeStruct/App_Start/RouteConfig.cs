using System.Web.Mvc;
using System.Web.Routing;

namespace TreeStruct
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //samo menu nie powinno się wyświtlać
            routes.IgnoreRoute("Node/Menu");
            //nie ma widoku dla Admin/DeleteNode
            routes.IgnoreRoute("Admin/DeleteNode");

            //tu bez zmian, zostawiam ustawienia domyślne
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
