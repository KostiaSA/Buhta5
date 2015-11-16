using System.Web.Mvc;

namespace Buhta.Areas.BuhtaSchemaDesigner
{
    public class BuhtaSchemaDesignerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BuhtaSchemaDesigner";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "BuhtaSchemaDesigner_default",
            //    "BuhtaSchemaDesigner/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
                "BuhtaSchemaDesigner_default1",
                "{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Buhta" }

            );
        }
    }
}