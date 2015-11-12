using System.Web.Mvc;

namespace Buhta.Areas.BuhtaCore
{
    public class BuhtaCoreAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BuhtaCore";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BuhtaCore_default",
                "BuhtaCore/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}