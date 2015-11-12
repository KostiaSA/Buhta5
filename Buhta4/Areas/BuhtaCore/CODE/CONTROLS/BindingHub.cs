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

        public void SendBindedValueChanged(string modelBindingID, string propertyName, string newValue)
        {

            try
            {
                BaseModel obj = App.BindingModelList[modelBindingID];
                obj.Hub = this;
                Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);

                obj.SetPropertyValue(propertyName, newValue);
                obj.FireOnChangeByBrowser(obj, propertyName, newValue);
                obj.Update();
            }
            catch (Exception e)
            {
                Clients.Caller.receiveServerError(e.GetFullMessage());
            }

        }

        public void SendEvent(string modelBindingID, string funcName, dynamic args)
        {
            try
            {
                BaseModel obj = App.BindingModelList[modelBindingID];
                obj.Hub = this;
                Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);

                obj.InvokeMethod(funcName, args);
                obj.Update();
            }
            catch (Exception e)
            {
                Clients.Caller.receiveServerError(e.GetFullMessage());
            }
        }

        public void SendGridDataSourceRequest(string modelBindingID, string propName, string fieldNames)
        {
            try
            {
                BaseModel obj = App.BindingModelList[modelBindingID];
                obj.Hub = this;
                Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);

                obj.UpdateCollection(propName, fieldNames);
            }
            catch (Exception e)
            {
                Clients.Caller.receiveServerError(e.GetFullMessage());
            }
        }

        public void SubscribeBindedValueChanged(string modelBindingID, string propertyName)
        {
            try
            {
                BaseModel obj = App.BindingModelList[modelBindingID];
                obj.Hub = this;
                Groups.Add(Context.ConnectionId, modelBindingID /*это groupName*/);
            }
            catch (Exception e)
            {
                Clients.Caller.receiveServerError(e.GetFullMessage());
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