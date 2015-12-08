using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public class MessageDialogModel : BaseModel
    {
        public MessageDialogModel(Controller controller, BaseModel parentModel) : base(controller, parentModel) { }

        public string Title;
        public MvcHtmlString TitleHtml = MvcHtmlString.Empty;

        public string Message;
        public MvcHtmlString MessageHtml = MvcHtmlString.Empty;

        public BinderEventMethod OkEventMethod;
        public BinderEventMethod CancelEventMethod;

        bool okClicked;
        public virtual void OkButtonClick(dynamic args)
        {
            okClicked = true;
            Modal.Close();
            if (OkEventMethod != null)
                OkEventMethod(args);
        }

        public virtual void CancelButtonClick(dynamic args)
        {
            Modal.Close();
        }

        public void ClosedByEsc(dynamic args)
        {
            if (!okClicked && CancelEventMethod != null)
                CancelEventMethod(args);
        }
    }
}
