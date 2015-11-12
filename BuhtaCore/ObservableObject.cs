using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buhta
{

    //[Serializable]
    //public class ChangeNotifyAttribute : LocationInterceptionAspect
    //{
    //    public override void OnSetValue(LocationInterceptionArgs args)
    //    {
    //        ObservableObject obj = (ObservableObject)args.Instance;
    //        obj.FireOnChange(obj, args.LocationName, args.Value);
    //        base.OnSetValue(args);
    //    }
    //}

    public delegate void ObservableObjectOnChangeEventHandler(ObservableObject sender, string propertyName, object newValue);

    public class ObservableObject 
    {


        public ObservableObject()
        {
        }

        public event ObservableObjectOnChangeEventHandler OnChangeByBrowser;

        public void FireOnChangeByBrowser(ObservableObject sender, string propertyName, object newValue)
        {
            if (OnChangeByBrowser != null)
                OnChangeByBrowser(sender, propertyName, newValue);
        }

        public event ObservableObjectOnChangeEventHandler OnChange;
        public void FireOnChange(ObservableObject sender, string propertyName, object newValue)
        {
            if (OnChange != null)
                OnChange(sender, propertyName, newValue);
        }
    }
}
