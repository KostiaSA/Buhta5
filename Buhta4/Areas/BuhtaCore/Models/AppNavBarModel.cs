using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public class AppNavBarModel : BaseModel
    {
        public Guid UserID;

        public AppNavBarModel(Controller controller, BaseModel parentModel) : base(controller, parentModel) { }

        bool okClicked;

        public virtual void CancelButtonClick(dynamic args)
        {
            Modal.Close();
        }

    }
}
