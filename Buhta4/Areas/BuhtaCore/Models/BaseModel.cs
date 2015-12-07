using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Buhta
{
    public class BaseModel : ObservableObject
    {
        public BaseModel ParentModel;
        public Controller Controller;
        public bsModal Modal;  // если это модель диалога
        public HtmlHelper Helper;
        public BindingHub Hub;
        public Dictionary<string, object> BindedProps = new Dictionary<string, object>();
        //List<OldBaseBinder> OldBindedBinders = new List<OldBaseBinder>();
        public Dictionary<string, object> BindedCollections = new Dictionary<string, object>();
        Dictionary<string, BaseBinder> BindedBinders = new Dictionary<string, BaseBinder>();

        //public void OldRegisterBinder(OldBaseBinder binder)
        //{
        //    binder.Model = this;
        //    OldBindedBinders.Add(binder);
        //}

        public void RegisterBinder(BaseBinder binder)
        {
            BindedBinders.Add(binder.UniqueId, binder);
        }

        public void BinderCallEvent(string binderId, dynamic args)
        {
            (BindedBinders[binderId] as EventBinder).ModelEventMethod(args);
        }

        public void BinderSetValue(string binderId, string value)
        {
            var binder = BindedBinders[binderId];
            if (binder.ValueType == typeof(string))
                (binder as TwoWayBinder<string>).ModelSetMethod(value);
            else
            if (binder.ValueType == typeof(int))
                (binder as TwoWayBinder<int>).ModelSetMethod(int.Parse(value));
            else
            if (binder.ValueType == typeof(Guid))
                (binder as TwoWayBinder<Guid>).ModelSetMethod(Guid.Parse(value));
            else
            if (binder.ValueType == typeof(bool))
                (binder as TwoWayBinder<bool>).ModelSetMethod(bool.Parse(value));
            else
            if (binder.ValueType == typeof(DateTime))
                (binder as TwoWayBinder<DateTime>).ModelSetMethod(DateTime.Parse(value));
            else
            if (binder.ValueType == typeof(decimal))
                (binder as TwoWayBinder<decimal>).ModelSetMethod(decimal.Parse(value));
            else
            if (binder.ValueType == typeof(float))
                (binder as TwoWayBinder<float>).ModelSetMethod(float.Parse(value));
            else
                throw new Exception("model." + nameof(BinderSetValue) + ": неизвестный тип '" + binder.ValueType.FullName + "'");

        }

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
                    if (_prop.PropertyType == typeof(string))
                        _prop.SetValue(obj, value.ToString(), null);
                    else
                    if (_prop.PropertyType == typeof(Boolean))
                        _prop.SetValue(obj, Boolean.Parse(value.ToString()), null);
                    else
                    if (_prop.PropertyType == typeof(Guid))
                        _prop.SetValue(obj, Guid.Parse(value.ToString()), null);
                    else
                    if (_prop.PropertyType == typeof(int))
                        _prop.SetValue(obj, int.Parse(value.ToString()), null);
                    else
                    if (_prop.PropertyType == typeof(Decimal))
                        _prop.SetValue(obj, Decimal.Parse(value.ToString()), null);
                    else
                    if (_prop.PropertyType == typeof(float))
                        _prop.SetValue(obj, float.Parse(value.ToString()), null);
                    else
                    if (_prop.PropertyType == typeof(DateTime))
                        _prop.SetValue(obj, DateTime.Parse(value.ToString()), null);
                    else
                        throw new Exception("model." + nameof(SetPropertyValue) + ": неизвестный тип '" + _prop.PropertyType.FullName + "'");
                }

            }
        }

        public BaseModel(Controller controller, BaseModel parentModel)
        {
            Controller = controller;
            ParentModel = parentModel;
        }

        public virtual string PageTitle { get { return "PageTitle"; } }

        public void UpdateNewNew()
        {
        }

        public void Update(bool includeDatasets = false)
        {
            Debug.WriteLine("");
            Debug.WriteLine("");
            var toSend = new StringBuilder();

            foreach (var binder in BindedBinders.Values.ToList().Where((a)=>a.IsActive).OrderBy((a) => a.UpdatePriority))
            {
                if (!binder.IsNotAutoUpdate || includeDatasets)
                {
                    var newText = binder.GetJsForSettingProperty();
                    //Debug.WriteLine("newText: "+ newText);
                    if (binder.LastSendedText != newText)
                    {
                        Debug.WriteLine("add ok: "+ newText);
                        toSend.AppendLine(newText);
                        binder.LastSendedText = newText;
                    }
                }

            }

            if (toSend.Length > 0)
            {
                Thread.Sleep(1); // не удалять, иначе все глючит !!!
                Hub.Clients.Group(BindingId).receiveScript(toSend.ToString());
                //Debug.Print(toSend.ToString());
            }
            if (ParentModel != null)
                ParentModel.Update();
        }

        public void UpdateDatasets()
        {
            var toSend = new StringBuilder();

            foreach (var binder in BindedBinders.Values.ToList().Where((a) => a.IsActive).OrderBy((a) => a.UpdatePriority))
            {
                if (binder.IsNotAutoUpdate)
                {
                    var newText = binder.GetJsForSettingProperty();
                    if (binder.LastSendedText != newText)
                    {
                        toSend.AppendLine(newText);
                        binder.LastSendedText = newText;
                    }
                }

            }

            if (toSend.Length > 0)
            {
                Thread.Sleep(1); // не удалять, иначе все глючит !!!
                Hub.Clients.Group(BindingId).receiveScript(toSend.ToString());
            }
            if (ParentModel != null)
                ParentModel.Update();
        }

        public void ExecuteJavaScript(string script)
        {
            Thread.Sleep(1); // не удалять, иначе все глючит !!!
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

            Thread.Sleep(1); // не удалять, иначе все глючит !!!
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
                    throw new Exception("model '" + this.GetType().FullName + "'." + nameof(GetPropertyValue) + ": не найдено свойство '" + names[i] + "' в '" + propName + "'");
                obj = _prop.GetValue(obj);
                if (obj == null)
                    return null;
                if (i == names.Length - 1)
                    return obj;
            }
            return null;
        }

        public T GetPropertyValue<T>(string propName) where T : class
        {
            var obj = GetPropertyValue(propName);
            if (obj == null)
                return null;
            if (obj is T)
                return (T)obj;
            throw new Exception("У модели '" + this.GetType().FullName + "' свойство '" + propName + "' должно быть типа ''" + typeof(T).FullName + "'");
        }

        //public string GetPropertyDisplayText(OldBaseBinder binder)
        //{
        //    return "binder.GetDisplayText(GetPropertyValue(binder.PropertyName))";
        //}

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

        //public void SetPropertyValue(string propName, object value)
        //{
        //    var names = propName.Split('.');
        //    object obj = this;
        //    for (int i = 0; i < names.Length; i++)
        //    {
        //        Type _type = obj.GetType();
        //        PropertyInfo _prop = _type.GetProperty(names[i]);
        //        if (_prop == null)
        //            throw new Exception("model." + nameof(SetPropertyValue) + ": не найдено свойство '" + names[i] + "' в '" + propName + "'");
        //        if (i < names.Length - 1)
        //        {
        //            obj = _prop.GetValue(obj);
        //            if (obj == null)
        //                throw new Exception("model." + nameof(SetPropertyValue) + ": объект '" + names[i] + "'==null в '" + propName + "'");
        //        }
        //        else
        //        {
        //            if (_prop.PropertyType == typeof(Boolean))
        //                _prop.SetValue(obj, Boolean.Parse(value.ToString()), null);
        //            else
        //            if (_prop.PropertyType == typeof(int))
        //                _prop.SetValue(obj, int.Parse(value.ToString()), null);
        //            else
        //            if (_prop.PropertyType == typeof(Decimal))
        //                _prop.SetValue(obj, Decimal.Parse(value.ToString()), null);
        //            else
        //            if (_prop.PropertyType == typeof(DateTime))
        //                _prop.SetValue(obj, DateTime.Parse(value.ToString()), null);
        //            else
        //                _prop.SetValue(obj, value, null);
        //        }

        //    }
        //}

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


        public bsModal CreateModal(string viewName = null, BaseModel model = null)
        {
            var modal = new bsModal(this);
            modal.Controller = Controller;
            modal.ViewName = viewName;
            modal.ViewModel = model;
            model.Modal = modal;
            return modal;
        }

        public void ShowInfoMessageDialog(string title, string message, BinderEventMethod closeEventMethod = null)
        {
            var model = new MessageDialogModel(Controller, this);
            model.Title = title;
            model.Message = message;
            model.CancelEventMethod = closeEventMethod;
            var modal = CreateModal(@"~\Areas\BuhtaCore\Views\InfoMessageDialog.cshtml", model);
            modal.Show();
        }

        public void ShowErrorMessageDialog(string title, string message, BinderEventMethod closeEventMethod = null)
        {
            var model = new MessageDialogModel(Controller, this);
            model.Title = title;
            model.Message = message;
            model.CancelEventMethod = closeEventMethod;
            var modal = CreateModal(@"~\Areas\BuhtaCore\Views\ErrorMessageDialog.cshtml", model);
            modal.Show();
        }

        public void ShowConfirmationMessageDialog(string title, string message, BinderEventMethod okEventMethod, BinderEventMethod cancelEventMethod = null)
        {
            var model = new MessageDialogModel(Controller, this);
            model.Title = title;
            model.Message = message;
            model.OkEventMethod = okEventMethod;
            model.CancelEventMethod = cancelEventMethod;
            var modal = CreateModal(@"~\Areas\BuhtaCore\Views\ConfirmationMessageDialog.cshtml", model);
            modal.Show();
        }

        public void ShowDeleteConfirmationMessageDialog(string title, string message, BinderEventMethod okEventMethod, BinderEventMethod cancelEventMethod = null)
        {
            var model = new MessageDialogModel(Controller, this);
            model.Title = title;
            model.Message = message;
            model.OkEventMethod = okEventMethod;
            model.CancelEventMethod = cancelEventMethod;
            var modal = CreateModal(@"~\Areas\BuhtaCore\Views\DeleteConfirmationMessageDialog.cshtml", model);
            modal.Show();
        }

        //public void ShowWindow(string viewName, object model)
        //{
        //    var windowHtml = R.RenderViewToString(Controller, @"~\Areas\BuhtaCore\Views\TableColumnEditorWindow.cshtml", model); //-это работает
        //    Hub.Clients.Group(BindingId).receiveShowWindow(windowHtml);

        //}

        //public virtual void SaveButtonClick(dynamic args)
        //{


        //}

        //public virtual void CancelButtonClick(dynamic args)
        //{


        //}

    }
}