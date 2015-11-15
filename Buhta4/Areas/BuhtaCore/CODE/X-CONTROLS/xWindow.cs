using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class xWindow
    {
        public string ChromeWindowId;
        public Controller Controller;
        public BaseModel ParentModel;
        public string ViewName;
        public BaseModel ViewModel;

        public string OnClose_Bind;

        public void Show()
        {
            var windowHtml = R.RenderViewToString(Controller, @"~\Areas\BuhtaCore\Views\TableColumnEditorWindow.cshtml", ViewModel);

            var script = new StringBuilder();
            script.Append(
@"
            var tag = $('<div><div></div></div>').appendTo('#popups');
            tag.jqxWindow({
                content: "+ windowHtml.AsJavaScriptStringQuoted() + @",
                isModal: true,
                showCollapseButton: true, maxHeight: 400, maxWidth: 700, minHeight: 10, minWidth: 400,// height: 300, width: 500,
                initContent: function() {
                      tag.jqxWindow('focus');
                    }
                });
");

            if (OnClose_Bind != null)
            {
                script.AppendLine("tag.on('close',function(event){");
                script.AppendLine(" var args={}; if (event) {args=event.args || {}};");
                script.AppendLine(" bindingHub.server.sendEvent(window.name,'" + ParentModel.BindingId + "','" + OnClose_Bind + "', args );");
                script.AppendLine("});");

            }

            ParentModel.Hub.Clients.Group(ParentModel.BindingId).receiveScript(ChromeWindowId, script.ToString());
            //ParentModel.Hub.Clients.Group(ParentModel.BindingId).receiveShowWindow(windowHtml);

        }

    }
}