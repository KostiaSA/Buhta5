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
        public string Message;

        public BinderEventMethod OkEventMethod;
        public BinderEventMethod CancelEventMethod;

        bool okClicked;
        public void OkButtonClick(dynamic args)
        {
            okClicked = true;
            Modal.Close();
            if (OkEventMethod != null)
                OkEventMethod(args);
        }

        public void CancelButtonClick(dynamic args)
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
