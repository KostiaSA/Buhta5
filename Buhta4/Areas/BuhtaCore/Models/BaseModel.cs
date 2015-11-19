using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Buhta
{
    public class BaseModel : ObservableObject
    {
        protected Controller Controller;
        public HtmlHelper Helper;
        public BindingHub Hub;
        public Dictionary<string, object> BindedProps = new Dictionary<string, object>();
        public List<BaseBinder> BindedBinders = new List<BaseBinder>();
        public Dictionary<string, object> BindedCollections = new Dictionary<string, object>();

        public BaseModel(Controller controller)
        {
            Controller = controller;
        }

        public virtual string PageTitle { get { return "PageTitle"; } }

        public void UpdateNew()
        {
            var toSend = new Dictionary<string, string>();

            foreach (var binder in BindedBinders.ToList())
            {
                if (!toSend.ContainsKey(binder.PropertyName))
                {
                    var newText = GetPropertyDisplayText(binder);
                    if (binder.LastSendedText != newText)
                    {
                        toSend.Add(binder.PropertyName, newText);
                        binder.LastSendedText = newText;
                    }
                }
            }

            if (toSend.Keys.Count > 0)
                Hub.Clients.Group(BindingId).receiveBindedValuesChanged(BindingId, toSend);
        }

        public void Update()
        {
            UpdateNew();

            var toSend = new Dictionary<string, object>();
            foreach (var KeyVP in BindedProps.ToList())
            {
                var propName = KeyVP.Key;
                var oldValue = KeyVP.Value;

                if (oldValue is IEnumerable<object> ||
                    oldValue is DataView)
                    continue;


                var newValue = GetPropertyValue(propName);
                if (oldValue.AsJavaScript() != newValue.AsJavaScript())
                    toSend.Add(propName, newValue);
            }

            foreach (var KeyVP in toSend)
                BindedProps[KeyVP.Key] = KeyVP.Value;

            if (toSend.Keys.Count > 0)
                Hub.Clients.Group(BindingId).receiveBindedValuesChanged( BindingId, toSend);
        }

        public void ExecuteJavaScript(string script)
        {
            Hub.Clients.Group(BindingId).receiveScript(script);
        }


        public List<object[]> EnumerableToJSArray(IEnumerable<object> source, string _fieldNames)
        {
            var list = new List<object[]>();
            var fieldNames = _fieldNames.Split(',');

            foreach (var row in source)
            {
                var row_array = new object[fieldNames.Length];
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    row_array[i] = row.EvalPropertyValue(fieldNames[i]);
                }
                list.Add(row_array);
            }

            return list;
        }

        public List<object[]> DataViewToJSArray(DataView dataView, string _fieldNames)
        {
            var list = new List<object[]>();
            var fieldNames = _fieldNames.Split(',');

            foreach (DataRowView rowView in dataView)
            {
                DataRow row = rowView.Row;
                // Do something //
                var row_array = new object[fieldNames.Length];
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    row_array[i] = row[fieldNames[i]];
                }
                list.Add(row_array);
            }

            //foreach (var row in source)
            //{
            //    var row_array = new object[fieldNames.Length];
            //    for (int i = 0; i < fieldNames.Length; i++)
            //    {
            //        row_array[i] = row.EvalPropertyValue(fieldNames[i]);
            //    }
            //    list.Add(row_array);
            //}

            return list;
        }

        // используется при изменении "collection"
        public void UpdateCollection(string propName)
        {
            var result = false;
            foreach (var key in BindedCollections.Keys.ToList())
            {
                var param = key.Split('\t');
                if (propName == param[0])
                {
                    var fieldNames = param[1];
                    UpdateCollection(propName, fieldNames);
                    result = true;
                }
            }
            if (!result)
                throw new Exception("Model." + nameof(UpdateCollection) + ": не найден набор данных '" + propName + "'");
        }


        // используется при первой загрузке "collection" в grid-у
        public void UpdateCollection(string propName, string fieldNames)
        {

            object newValue = GetPropertyValue(propName);
            List<object[]> toSend;


            if (newValue is IEnumerable<object>)
                toSend = EnumerableToJSArray((IEnumerable<object>)newValue, fieldNames);
            else
            if (newValue is DataView)
                toSend = DataViewToJSArray((DataView)newValue, fieldNames);
            else
                throw new Exception(nameof(UpdateCollection) + ": " + propName + " должен быть IEnumerable или DataView");

            Hub.Clients.Group(BindingId).receiveBindedValueChanged(BindingId, propName, toSend);

            if (BindedCollections.ContainsKey(propName + "\t" + fieldNames))
                BindedCollections[propName + "\t" + fieldNames] = newValue;
            else
                BindedCollections.Add(propName + "\t" + fieldNames, newValue);

        }


        //public void UpdateGridDataSource(xGridSettings grid)
        //{
        //    var propName = grid.DataSource_Bind;
        //    object oldValue;
        //    if (BindedProps.ContainsKey(propName))
        //        oldValue = BindedProps[propName];

        //    object newValue = GetPropertyValue(propName);
        //    if (!(newValue is IEnumerable<object>))
        //        throw new Exception(nameof(UpdateGridDataSource)+": "+ propName+" должен быть IEnumerable");

        //    List<string> fieldNames = new List<string>();
        //    foreach (var col in grid.Columns)
        //        fieldNames.Add(col.Field_Bind);

        //    var toSend = EnumerableToJSArray((IEnumerable<object>)newValue, fieldNames);

        //    Hub.Clients.Group(BindingId).receiveBindedValuesChanged(BindingId, toSend);

        //    if (BindedProps.ContainsKey(propName))
        //        BindedProps[propName] = newValue;
        //    else
        //        BindedProps.Add(propName, newValue);

        //}

        string bindingId;
        public string BindingId
        {
            get
            {
                if (bindingId == null)
                {
                    bindingId = Guid.NewGuid().ToString();
                    BindingHub.BindingModelList.Add(bindingId, this);
                }
                return bindingId;
            }
        }

        public PropertyInfo GetProperty(string propName)
        {
            var names = propName.Split('.');
            object obj = this;
            if (names.Length > 1)
            {
                for (int i = 0; i < names.Length - 1; i++)
                {
                    Type _type = obj.GetType();
                    PropertyInfo _prop = _type.GetProperty(names[i]);
                    obj = _prop.GetValue(obj);
                    if (obj == null)
                        return null;
                }
            }
            Type type = obj.GetType();
            PropertyInfo prop = type.GetProperty(names.Last());
            return prop;
        }

        public object GetPropertyValue(string propName)
        {
            var names = propName.Split('.');
            object obj = this;
            for (int i = 0; i < names.Length; i++)
            {
                Type _type = obj.GetType();
                PropertyInfo _prop = _type.GetProperty(names[i]);
                if (_prop == null)
                    throw new Exception("model." + nameof(GetPropertyValue) + ": не найдено свойство '" + names[i] + "' в '" + propName + "'");
                obj = _prop.GetValue(obj);
                if (obj == null)
                    return null;
                if (i == names.Length - 1)
                    return obj;
            }
            return null;
        }

        public string GetPropertyDisplayText(BaseBinder binder)
        {
            return binder.GetDisplayText(GetPropertyValue(binder.PropertyName));
        }

        //public ObservableObject GetPropertyObject(string propName)
        //{
        //    var names = propName.Split('.');
        //    if (names.Length == 1)
        //        return this;
        //    object obj = this;
        //    for (int i = 0; i < names.Length; i++)
        //    {
        //        Type _type = obj.GetType();
        //        PropertyInfo _prop = _type.GetProperty(names[i]);
        //        if (_prop == null)
        //            throw new Exception("model." + nameof(GetPropertyObject) + ": не найдено свойство '" + names[i] + "' в '" + propName + "'");
        //        obj = _prop.GetValue(obj);
        //        if (obj == null)
        //            return null;
        //        if (i == names.Length - 2)
        //        {
        //            if (obj is ObservableObject)
        //                return obj as ObservableObject;
        //            else
        //                throw new Exception("model." + nameof(GetPropertyObject) + ": объект должен быть типа " + nameof(ObservableObject) + " в '" + propName + "'");
        //        }
        //    }
        //    return null;
        //}

        public void SetPropertyValue(string propName, object value)
        {
            var names = propName.Split('.');
            object obj = this;
            for (int i = 0; i < names.Length; i++)
            {
                Type _type = obj.GetType();
                PropertyInfo _prop = _type.GetProperty(names[i]);
                if (_prop == null)
                    throw new Exception("model." + nameof(SetPropertyValue) + ": не найдено свойство '" + names[i] + "' в '" + propName + "'");
                if (i < names.Length - 1)
                {
                    obj = _prop.GetValue(obj);
                    if (obj == null)
                        throw new Exception("model." + nameof(SetPropertyValue) + ": объект '" + names[i] + "'==null в '" + propName + "'");
                }
                else
                {
                    if (_prop.PropertyType == typeof(Boolean))
                        _prop.SetValue(obj, Boolean.Parse(value.ToString()), null);
                    else
                    if (_prop.PropertyType == typeof(int))
                        _prop.SetValue(obj, int.Parse(value.ToString()), null);
                    else
                    if (_prop.PropertyType == typeof(Decimal))
                        _prop.SetValue(obj, Decimal.Parse(value.ToString()), null);
                    else
                    if (_prop.PropertyType == typeof(DateTime))
                        _prop.SetValue(obj, DateTime.Parse(value.ToString()), null);
                    else
                        _prop.SetValue(obj, value, null);
                }

            }
        }

        public void InvokeMethod(string propName, dynamic args)
        {
            var names = propName.Split('.');
            object obj = this;
            for (int i = 0; i < names.Length; i++)
            {
                if (i < names.Length - 1)
                {
                    Type _type = obj.GetType();
                    PropertyInfo _prop = _type.GetProperty(names[i]);
                    if (_prop == null)
                        throw new Exception("model." + nameof(InvokeMethod) + ": не найдено свойство '" + names[i] + "' в '" + propName + "'");
                    obj = _prop.GetValue(obj);
                    if (obj == null)
                        throw new Exception("model." + nameof(InvokeMethod) + ": объект '" + names[i] + "'==null в '" + propName + "'");
                }
                else
                {
                    Type _type = obj.GetType();
                    MethodInfo _method = _type.GetMethod(names[i]);
                    if (_method == null)
                        throw new Exception("model." + nameof(InvokeMethod) + ": не найден метод '" + names[i] + "' в '" + propName + "'");

                    _method.Invoke(obj, new dynamic[] { args });
                    return;

                }

            }
        }


        public xWindow CreateWindow( string viewName = null, BaseModel model = null)
        {
            var win = new xWindow();
            win.ParentModel = this;
            win.Controller = Controller;
            win.ViewName = viewName;
            win.ViewModel = model;
            return win;
        }

        //public void ShowWindow(string viewName, object model)
        //{
        //    var windowHtml = R.RenderViewToString(Controller, @"~\Areas\BuhtaCore\Views\TableColumnEditorWindow.cshtml", model); //-это работает
        //    Hub.Clients.Group(BindingId).receiveShowWindow(windowHtml);

        //}

        public virtual void SaveButtonClick(dynamic args)
        {


        }

        public virtual void CancelButtonClick( dynamic args)
        {


        }

    }
}