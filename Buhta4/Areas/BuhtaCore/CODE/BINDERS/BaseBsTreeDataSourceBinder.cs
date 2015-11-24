﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace Buhta
{
    public abstract class BaseBsTreeDataSourceBinder:BaseBinder
    {
        public bsTree Tree;

        public BaseBsTreeDataSourceBinder(string propertyName) : base(propertyName) { }

        public abstract string GetJsonDataTreeSource(BaseModel model, ObservableCollection<string> selectedRows);

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