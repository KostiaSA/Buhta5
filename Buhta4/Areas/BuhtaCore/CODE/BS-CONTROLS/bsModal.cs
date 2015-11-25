using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class bsModal
    {
        public Controller Controller;
        public BaseModel ParentModel;
        public string ViewName;
        public BaseModel ViewModel;

        public string OnClose_Bind;

        public void Show()
        {
            var windowHtml = R.RenderViewToString(Controller, ViewName, ViewModel);

            var init = new JsObject();
            init.AddProperty("show",true);

            var script = new StringBuilder();
            script.Append("var tag = $("+ windowHtml.AsJavaScript() + ").appendTo('#popups').modal("+init.AsJavaScript()+");");

            //if (OnClose_Bind != null)
            //{
            //    script.AppendLine("tag.on('close',function(event){");
            //    script.AppendLine(" var args={}; if (event) {args=event.args || {}};");
            //    script.AppendLine(" bindingHub.server.sendEvent('" + ParentModel.BindingId + "','" + OnClose_Bind + "', args );");
            //    script.AppendLine("});");

            //}

            ParentModel.Hub.Clients.Group(ParentModel.BindingId).receiveScript(script.ToString());

        }

    }
}