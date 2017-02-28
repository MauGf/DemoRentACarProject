using System.Web.Mvc;

namespace RentACar.Areas.MyRents
{
    public class MyRentsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MyRents";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MyRents_default",
                "MyRents/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "MyRents_index",
                "MyRents/",
                new { action = "Index", controller = "Rents", id = UrlParameter.Optional }
            );
        }
    }
}