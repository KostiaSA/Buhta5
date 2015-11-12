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
        public Controller Controller;
        public BaseModel ParentModel;
        public string ViewName;
        public BaseModel ViewModel;

        public void Show()
        {
            var windowHtml = R.RenderViewToString(Controller, @"~\Areas\BuhtaCore\Views\TableColumnEditorWindow.cshtml", ViewModel);

            var script = new StringBuilder();
            script.Append(
@"
            var win = $('<div><div></div></div>').appendTo('#popups');
            $(win).jqxWindow({
                content: "+ windowHtml.AsJavaScriptStringQuoted() + @",
                isModal: true,
                showCollapseButton: true, maxHeight: 400, maxWidth: 700, minHeight: 10, minWidth: 400,// height: 300, width: 500,
                initContent: function() {
                      $(win).jqxWindow('focus');
                    }
                });
");
            ParentModel.Hub.Clients.Group(ParentModel.BindingId).receiveScript(script.ToString());
            //ParentModel.Hub.Clients.Group(ParentModel.BindingId).receiveShowWindow(windowHtml);

        }

    }
}