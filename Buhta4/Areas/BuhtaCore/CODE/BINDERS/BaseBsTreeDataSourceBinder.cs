using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buhta
{
    public abstract class BaseBsTreeDataSourceBinder:BaseBinder
    {

        public BaseBsTreeDataSourceBinder(string propertyName) : base(propertyName) { }

        public abstract string GetJsonDataTreeSource(BaseModel model);

            //public virtual string GetDisplayText(object value)
            //{
            //    throw new Exception("метод " + nameof(GetDisplayText) + " не рализован");
            //}

            //public virtual object ParseDisplayText(string text)
            //{
            //    throw new Exception("метод "+nameof(ParseDisplayText)+" не рализован");
            //}


        }
    }