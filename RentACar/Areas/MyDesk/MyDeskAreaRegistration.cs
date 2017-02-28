using System.Web.Mvc;

namespace RentACar.Areas.MyDesk
{
    public class MyDeskAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MyDesk";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MyDesk_default",
                "MyDesk/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "MyDesk_index",
                "MyDesk/",
                new { action = "Index", controller = "Cars", id = UrlParameter.Optional }
            );
        }
    }
}