using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buhta
{

    public partial class AppServer
    {
        public static ConcurrentDictionary<string, BaseSessionStateObject> SessionStates = new ConcurrentDictionary<string, BaseSessionStateObject>();


        public static void SaveStateObject(BaseSessionStateObject stateObject)
        {
            var state_key = AppServer.CurrentAppNavBarModel.ChromeSessionId + "-" + stateObject.Id;
            SessionStates.AddOrUpdate(state_key, stateObject, (key, value) => stateObject);
        }

        public static T GetStateObject<T>(string stateObjectId) where T : BaseSessionStateObject
        {
            var state_key = AppServer.CurrentAppNavBarModel.ChromeSessionId + "-" + stateObjectId;
            BaseSessionStateObject _retObject = null;
            if (SessionStates.TryGetValue(state_key, out _retObject) && _retObject is T)
                return _retObject as T;
            else
                return null;
        }

    }

    public class BaseSessionStateObject
    {
        public string Id;

        [JsonIgnore]
        public string LastSavedToSqlJson;

        public virtual void Save()
        {
            AppServer.SaveStateObject(this);
        }

    }
}