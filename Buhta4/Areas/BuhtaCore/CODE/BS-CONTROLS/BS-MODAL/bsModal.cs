using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class bsModal : bsControl
    {
        public Controller Controller;
        public string ViewName;
        public BaseModel ViewModel;

        public bsModal(BaseModel model) : base(model) { }

        public void Bind_OkClick(BinderEventMethod eventMethod)
        {
            AddBinder(new EventBinder()
            {
                ModelEventMethod = eventMethod,
                jsEventName = "click"
            });
        }

        public void Bind_OkClick(string modelEventMethodName)
        {
            AddBinder(new EventBinder()
            {
                ModelEventMethodName = modelEventMethodName,
                jsEventName = "click"
            });
        }

        public void Bind_CancelClick(BinderEventMethod eventMethod)
        {
            AddBinder(new EventBinder()
            {
                ModelEventMethod = eventMethod,
                jsEventName = "click"
            });
        }

        public void Bind_CancelClick(string modelEventMethodName)
        {
            AddBinder(new EventBinder()
            {
                ModelEventMethodName = modelEventMethodName,
                jsEventName = "click"
            });
        }

        public void Bind_CancelCloseQuery(string modelEventMethodName)
        {
            AddBinder(new EventBinder()
            {
                ModelEventMethodName = modelEventMethodName,
                jsEventName = "hide"
            });
        }


        //        public event EventHandler OnClose;


        public void Close()
        {
            Thread.Sleep(1); // не удалять, иначе все глючит !!!
            Model.Hub.Clients.Group(Model.BindingId).receiveScript("$('#" + UniqueId + "').modal('hide');");
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
            EmitBinders(script);
            script.Append("var modal = $(" + windowHtml.AsJavaScript() + ");");
            script.Append("modal.attr('id','" + UniqueId + "');");

            script.Append("$('body').append(modal);");

            if (ViewModel is MessageDialogModel)
            {
                script.Append("modal.on('hidden.bs.modal', function (e) {");
                script.Append(" bindingHub.server.sendEvent(localStorage.ChromeSessionId,'" + ViewModel.BindingId + "','" + nameof(MessageDialogModel.ClosedByEsc) + "', {} );");
                script.Append("}); ");

            }

            script.Append("modal.modal(" + init.AsJavaScript() + ");");
            script.Append("modal.off('keyup.dismiss.modal');");
            script.Append("modal.on('keyup.esc.modal', function(e) {if (e.which == 27) { modal.find('.modal-cancel-button').trigger('click')}; });");

            //if (OnClose_Bind != null)
            //{
            //    script.AppendLine("tag.on('close',function(event){");
            //    script.AppendLine(" var args={}; if (event) {args=event.args || {}};");
            //    script.AppendLine(" bindingHub.server.sendEvent('" + ParentModel.BindingId + "','" + OnClose_Bind + "', args );");
            //    script.AppendLine("});");

            //}

            Thread.Sleep(1); // не удалять, иначе все глючит !!!
            Model.Hub.Clients.Group(Model.BindingId).receiveScript(script.ToString());

        }

    }
}