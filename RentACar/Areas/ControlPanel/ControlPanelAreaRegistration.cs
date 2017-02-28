using System.Web.Mvc;

namespace RentACar.Areas.ControlPanel
{
    public class ControlPanelAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ControlPanel";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ControlPanel_default",
                "ControlPanel/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "ControlPanel_index",
                "ControlPanel/",
                new { action = "Index", controller = "AppSettings", id = UrlParameter.Optional }
            );
        }
    }
}