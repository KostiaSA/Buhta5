using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace Buhta
{
    public partial class AppServer
    {
        public static ConcurrentDictionary<string, BaseModel> BindingModelList = new ConcurrentDictionary<string, BaseModel>();
    }

    public class BindingHub : Hub
    {
        //public void UnloadChromeWindow(string sessionID, string chromeWindowName)
        //{
        //    AppServer.SetCurrentAppNavBarModel(sessionID);
        //    if (AppServer.CurrentAppNavBarModel != null)
        //        AppServer.CurrentAppNavBarModel.DestroyChromeWindow(chromeWindowName);
        //}

        public void RegisterChromeWindow(string sessionID, string chromeWindowName, string modelBindingId, string recordId)
        {
            try
            {
                AppServer.SetCurrentAppNavBarModel(sessionID);
                if (AppServer.CurrentAppNavBarModel.Hub == null)
                    AppServer.CurrentAppNavBarModel.Hub = this;

                Groups.Add(Context.ConnectionId, sessionID /*это groupName*/);

                ChromeWindow win;
                AppServer.CurrentAppNavBarModel.ChromeWindows.TryGetValue(chromeWindowName, out win);
                win.SignalrCaller = Clients.Caller;
                win.ModelBindingId = modelBindingId;
                win.RecordId = recordId;
                win.CreateDate = DateTime.Now;
                if (!AppServer.ChromeWindows.TryAdd(Context.ConnectionId, win))
                    throw new Exception("internal error");

            }
            catch (Exception e)
            {
                Clients.Caller.receiveServerError("ошибка '" + nameof(RegisterChromeWindow) + "': " + e.GetFullMessage());

            }
        }

        public void SendBindedValueChanged(string sessionID, string modelBindingID, string propertyName, string newValue)
        {
            //Debug.Print("SendBindedValueChanged: " + propertyName + ", " + newValue);
            try
            {
                AppServer.CurrentAppNavBarModel = null;
                AppServer.SetCurrentAppNavBarModel(sessionID);
                if (!AppServer.ChromeWindows.TryGetValue(Context.ConnectionId, out AppServer.CurrentAppNavBarModel.FocusedWindow))
                    throw new Exception("internal error AppServer.ChromeWindows.TryGetValue");

                BaseModel obj = AppServer.BindingModelList[modelBindingID];
                obj.Hub = this;
                Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);

                if (propertyName.StartsWith("binder:"))
                {
                    //Debug.Print("obj.BinderSetValue: " + propertyName + ", " + newValue);
                    obj.BinderSetValue(propertyName, newValue);
                    obj.Update();
                }
                else
                {
                    throw new Exception("internal error BindingHub.SendBindedValueChanged");
                    //obj.SetPropertyValue(propertyName, newValue);
                    //obj.FireOnChangeByBrowser(obj, propertyName, newValue);
                    //obj.Update();
                }
            }
            catch (Exception e)
            {
                var obj = AppServer.BindingModelList[modelBindingID];
                if (obj != null)
                {
                    BaseBinder binder;
                    if (obj.BindedBinders.TryGetValue(propertyName, out binder) && binder.GetPropertyNameForErrorMessage() != null)
                        propertyName = binder.GetPropertyNameForErrorMessage();
                    obj.ShowExceptionMessageDialog("Сервер вернул ошибку", "@модель '" + obj.GetType().FullName + "', свойство '" + propertyName + "':<br>" + e.GetFullMessage().Replace("\n", "<br>"));
                    //Clients.Caller.receiveServerError();
                }
                else
                if (AppServer.CurrentAppNavBarModel != null)
                {
                    AppServer.CurrentAppNavBarModel.ShowExceptionMessageDialog("Сервер вернул ошибку", e.GetFullMessage().Replace("\n", "<br>"));
                }
                else
                    Clients.Caller.receiveServerError(e.GetFullMessage().Replace("\n", "<br>"));

            }

        }

        public void SendBsTreeBindedEditableValueChanged(string sessionID, string modelBindingID, string propertyName, string newValue, string recordId)
        {
            //Debug.Print("SendBindedValueChanged: " + propertyName + ", " + newValue);
            try
            {
                AppServer.CurrentAppNavBarModel = null;
                AppServer.SetCurrentAppNavBarModel(sessionID);
                if (!AppServer.ChromeWindows.TryGetValue(Context.ConnectionId, out AppServer.CurrentAppNavBarModel.FocusedWindow))
                    throw new Exception("internal error AppServer.ChromeWindows.TryGetValue");

                BaseModel obj = AppServer.BindingModelList[modelBindingID];
                obj.Hub = this;
                Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);

                if (propertyName.StartsWith("binder:"))
                {
                    //Debug.Print("obj.BinderSetValue: " + propertyName + ", " + newValue);
                    obj.BinderSetValue(propertyName, newValue, recordId);
                    obj.Update();
                }
                else
                {
                    throw new Exception("internal error BindingHub.SendBindedValueChanged");
                    //obj.SetPropertyValue(propertyName, newValue);
                    //obj.FireOnChangeByBrowser(obj, propertyName, newValue);
                    //obj.Update();
                }
            }
            catch (Exception e)
            {
                var obj = AppServer.BindingModelList[modelBindingID];
                if (obj != null)
                {
                    BaseBinder binder;
                    if (obj.BindedBinders.TryGetValue(propertyName, out binder) && binder.GetPropertyNameForErrorMessage() != null)
                        propertyName = binder.GetPropertyNameForErrorMessage();
                    obj.ShowExceptionMessageDialog("Сервер вернул ошибку", "@модель '" + obj.GetType().FullName + "', свойство '" + propertyName + "':<br>" + e.GetFullMessage().Replace("\n", "<br>"));
                    //Clients.Caller.receiveServerError();
                }
                else
                if (AppServer.CurrentAppNavBarModel != null)
                {
                    AppServer.CurrentAppNavBarModel.ShowExceptionMessageDialog("Сервер вернул ошибку", e.GetFullMessage().Replace("\n", "<br>"));
                }
                else
                    Clients.Caller.receiveServerError(e.GetFullMessage().Replace("\n", "<br>"));

            }

        }

        //public void SendSelectedRowsChanged(string modelBindingID, string propertyName, string rowID, bool isSelected)
        //{
        //    try
        //    {
        //        BaseModel obj = BindingModelList[modelBindingID];
        //        obj.Hub = this;
        //        Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);

        //        var selectedList = obj.GetPropertyValue<ObservableCollection<string>>(propertyName);
        //        if (isSelected && !selectedList.Contains(rowID))
        //            selectedList.Add(rowID);
        //        if (!isSelected && selectedList.Contains(rowID))
        //            selectedList.Remove(rowID);
        //        obj.FireOnChangeByBrowser(obj, propertyName, rowID);
        //        obj.Update();
        //    }
        //    catch (Exception e)
        //    {
        //        var obj = BindingModelList[modelBindingID];
        //        if (obj == null)
        //            Clients.Caller.receiveServerError("не найден " + nameof(modelBindingID) + " = " + modelBindingID);
        //        else
        //            Clients.Caller.receiveServerError("модель '" + obj.GetType().FullName + "', свойство '" + propertyName + "':\n" + e.GetFullMessage());

        //    }

        //}

        public void SendEvent(string sessionID, string modelBindingID, string funcName, dynamic args)
        {
            try
            {
                AppServer.SetCurrentAppNavBarModel(sessionID);
                if (!AppServer.ChromeWindows.TryGetValue(Context.ConnectionId, out AppServer.CurrentAppNavBarModel.FocusedWindow))
                    throw new Exception("internal error AppServer.ChromeWindows.TryGetValue");
                BaseModel obj = AppServer.BindingModelList[modelBindingID];
                obj.Hub = this;
                Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);

                if (funcName.StartsWith("binder:"))
                {
                    obj.BinderCallEvent(funcName, args);
                    obj.Update();
                }
                else
                {
                    obj.InvokeMethod(funcName, args);
                    obj.Update();
                }
            }
            catch (Exception e)
            {
                var obj = AppServer.BindingModelList[modelBindingID];
                if (obj != null)
                {
                    BaseBinder binder;
                    if (obj.BindedBinders.TryGetValue(funcName, out binder) && binder.GetPropertyNameForErrorMessage() != null)
                        funcName = binder.GetPropertyNameForErrorMessage();
                    obj.ShowExceptionMessageDialog("Сервер вернул ошибку", "@модель '" + obj.GetType().FullName + "', свойство '" + funcName + "':<br>" + e.GetFullMessage().Replace("\n", "<br>"));
                    //Clients.Caller.receiveServerError();
                }
                else
                if (AppServer.CurrentAppNavBarModel != null)
                {
                    AppServer.CurrentAppNavBarModel.ShowExceptionMessageDialog("Сервер вернул ошибку", e.GetFullMessage().Replace("\n", "<br>"));
                }
                else
                    Clients.Caller.receiveServerError(e.GetFullMessage().Replace("\n", "<br>"));
            }
        }

        //public void SendGridDataSourceRequest(string modelBindingID, string propName, string fieldNames)
        //{
        //    try
        //    {
        //        BaseModel obj = BindingModelList[modelBindingID];
        //        obj.Hub = this;
        //        Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);

        //        obj.UpdateCollection(propName, fieldNames);
        //    }
        //    catch (Exception e)
        //    {
        //        var obj = BindingModelList[modelBindingID];
        //        if (obj == null)
        //            Clients.Caller.receiveServerError(nameof(SendGridDataSourceRequest) + ": не найден " + nameof(modelBindingID) + " = " + modelBindingID);
        //        else
        //            Clients.Caller.receiveServerError(nameof(SendGridDataSourceRequest) + " в '" + obj.GetType().FullName + "'\n" + e.GetFullMessage());
        //    }
        //}

        //public void SubscribeBindedValueChanged(string sessionID, string modelBindingID, string propertyName)
        //{
        //    try
        //    {
        //        AppServer.SetCurrentAppNavBarModel(sessionID);
        //        BaseModel obj = BindingModelList[modelBindingID];
        //        if (obj.Hub == null)
        //            obj.Hub = this;
        //        Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);
        //    }
        //    catch (Exception e)
        //    {
        //        var obj = BindingModelList[modelBindingID];
        //        if (obj == null)
        //            Clients.Caller.receiveServerError(nameof(SubscribeBindedValueChanged) + ": не найден " + nameof(modelBindingID) + " = " + modelBindingID);
        //        else
        //            Clients.Caller.receiveServerError(nameof(SubscribeBindedValueChanged) + " в '" + obj.GetType().FullName + "', свойство '" + propertyName + "'\n" + e.GetFullMessage());
        //        //Clients.Caller.receiveServerError( "свойство '" + propertyName + "':\n" + e.GetFullMessage());
        //    }

        //}

        public override Task OnDisconnected(bool stopCalled)
        {
            ChromeWindow win;
            if (!AppServer.ChromeWindows.TryGetValue(Context.ConnectionId, out win))
                throw new Exception(nameof(OnDisconnected) + ": internal error");

            AppServer.SetCurrentAppNavBarModel(win.ChromeSessionId);
            if (AppServer.CurrentAppNavBarModel != null)
                AppServer.CurrentAppNavBarModel.DestroyChromeWindow(win.Name);

            if (!AppServer.ChromeWindows.TryRemove(Context.ConnectionId, out win))
                throw new Exception("internal error");


            return base.OnDisconnected(stopCalled);
        }
        public override Task OnConnected()
        {
            return base.OnConnected();
        }
    }
}