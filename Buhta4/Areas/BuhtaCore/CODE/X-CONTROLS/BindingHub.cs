using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace Buhta
{

    public class BindingHub : Hub
    {
        public static Dictionary<string, BaseModel> BindingModelList = new Dictionary<string, BaseModel>();

        public void SendBindedValueChanged(string modelBindingID, string propertyName, string newValue)
        {

            try
            {
                BaseModel obj = BindingModelList[modelBindingID];
                obj.Hub = this;
                Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);

                obj.SetPropertyValue(propertyName, newValue);
                obj.FireOnChangeByBrowser(obj, propertyName, newValue);
                obj.Update();
            }
            catch (Exception e)
            {
                var obj = BindingModelList[modelBindingID];
                if (obj == null)
                    Clients.Caller.receiveServerError("не найден " + nameof(modelBindingID) + " = " + modelBindingID);
                else
                    Clients.Caller.receiveServerError("модель '" + obj.GetType().FullName + "', свойство '" + propertyName + "':\n" + e.GetFullMessage());

            }

        }

        public void SendEvent(string modelBindingID, string funcName, dynamic args)
        {
            try
            {
                BaseModel obj = BindingModelList[modelBindingID];
                obj.Hub = this;
                Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);

                obj.InvokeMethod(funcName, args);
                obj.Update();
            }
            catch (Exception e)
            {
                var obj = BindingModelList[modelBindingID];
                if (obj == null)
                    Clients.Caller.receiveServerError("не найден " + nameof(modelBindingID) + " = " + modelBindingID + " для метода '" + funcName + "'");
                else
                    Clients.Caller.receiveServerError("вызов '" + obj.GetType().FullName + "', метод '" + funcName + "':\n" + e.GetFullMessage());
            }
        }

        public void SendGridDataSourceRequest(string modelBindingID, string propName, string fieldNames)
        {
            try
            {
                BaseModel obj = BindingModelList[modelBindingID];
                obj.Hub = this;
                Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);

                obj.UpdateCollection(propName, fieldNames);
            }
            catch (Exception e)
            {
                var obj = BindingModelList[modelBindingID];
                if (obj == null)
                    Clients.Caller.receiveServerError(nameof(SendGridDataSourceRequest) + ": не найден " + nameof(modelBindingID) + " = " + modelBindingID);
                else
                    Clients.Caller.receiveServerError(nameof(SendGridDataSourceRequest) + " в '" + obj.GetType().FullName + "'\n" + e.GetFullMessage());
            }
        }

        public void SubscribeBindedValueChanged(string modelBindingID, string propertyName)
        {
            try
            {
                BaseModel obj = BindingModelList[modelBindingID];
                obj.Hub = this;
                Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);
            }
            catch (Exception e)
            {
                var obj = BindingModelList[modelBindingID];
                if (obj == null)
                    Clients.Caller.receiveServerError(nameof(SubscribeBindedValueChanged) + ": не найден " + nameof(modelBindingID) + " = " + modelBindingID);
                else
                    Clients.Caller.receiveServerError(nameof(SubscribeBindedValueChanged) + " в '" + obj.GetType().FullName + "', свойство '" + propertyName + "'\n" + e.GetFullMessage());
                //Clients.Caller.receiveServerError( "свойство '" + propertyName + "':\n" + e.GetFullMessage());
            }

        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
        public override Task OnConnected()
        {
            return base.OnConnected();
        }
    }
}