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

        public void Close()
        {
            ParentModel.Hub.Clients.Group(ParentModel.BindingId).receiveScript("$('#" + UniqueId + "').modal('hide');");
        }

        public void Show()
        {
            var windowHtml = R.RenderViewToString(Controller, ViewName, ViewModel);

            var init = new JsObject();
            init.AddProperty("show", true);
            init.AddProperty("backdrop", "static");
            //init.AddProperty("modalOverflow", true);
            init.AddRawProperty("maxHeight", "function(){return $(window).height()-200;}");

            var script = new StringBuilder();
            //            script.Append("var tag = $("+ windowHtml.AsJavaScript() + ").appendTo('#popups').modal("+init.AsJavaScript()+");");

            script.Append("docReady = function(callback) { callback() };");
            script.Append("var modal = $(" + windowHtml.AsJavaScript() + ");");
            script.Append("modal.attr('id','" + UniqueId + "');");
            script.Append("$('body').append(modal);");
            script.Append("modal.modal(" + init.AsJavaScript() + ");");

            //if (OnClose_Bind != null)
            //{
            //    script.AppendLine("tag.on('close',function(event){");
            //    script.AppendLine(" var args={}; if (event) {args=event.args || {}};");
            //    script.AppendLine(" bindingHub.server.sendEvent('" + ParentModel.BindingId + "','" + OnClose_Bind + "', args );");
            //    script.AppendLine("});");

            //}

            ParentModel.Hub.Clients.Group(ParentModel.BindingId).receiveScript(script.ToString());

        }

        string uniqueId;
        public string UniqueId
        {
            get
            {
                if (uniqueId == null)
                {
                    uniqueId = "modal" + Guid.NewGuid().ToString().Substring(0, 8);
                }
                return uniqueId;
            }
        }


    }
}