﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public partial class AppServer
    {
        public static ConcurrentDictionary<string, AppNavBarModel> AppNavBars = new ConcurrentDictionary<string, AppNavBarModel>();
        public static ConcurrentDictionary<string, ChromeWindow> ChromeWindows = new ConcurrentDictionary<string, ChromeWindow>();

        [ThreadStatic]
        public static AppNavBarModel CurrentAppNavBarModel;
        [ThreadStatic]
        public static ChromeWindow CurrentChromeWindow;


        public static void SetCurrentAppNavBarModel(string sessionId, Controller controller)
        {
            CurrentAppNavBarModel = AppNavBars.GetOrAdd(sessionId, (_sessionId) =>
            {
                var navBar = new AppNavBarModel(controller, null);
                navBar.ChromeSessionId = sessionId;
                return navBar;
            });
            CurrentAppNavBarModel.LastRequestTime = DateTime.Now;

        }
        public static void SetCurrentAppNavBarModel(string sessionId)
        {

            if (AppNavBars.TryGetValue(sessionId, out CurrentAppNavBarModel))
                CurrentAppNavBarModel.LastRequestTime = DateTime.Now;
            else
                CurrentAppNavBarModel = null;

        }

        public static void RegisterNewChromeWindow()
        {
            var win = new ChromeWindow();

            win.ChromeSessionId = CurrentAppNavBarModel.ChromeSessionId;
            win.Name = "win-" + Guid.NewGuid().ToString();
            CurrentAppNavBarModel.ChromeWindows.TryAdd(win.Name, win);
            CurrentChromeWindow = win;
        }


        public static MvcHtmlString GetDebugInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div>");
            sb.AppendLine("current sessionId: " + CurrentAppNavBarModel.ChromeSessionId + "<br>");
            sb.AppendLine("current window: " + CurrentChromeWindow.Name + "<br>");
            sb.AppendLine("<br>");

            foreach (var navBar in AppServer.AppNavBars.Values)
            {
                sb.AppendLine("sessionId: " + navBar.ChromeSessionId + "  last: " + navBar.LastRequestTime.ToLongTimeString() + "<br>");
                foreach (var win in navBar.ChromeWindows.Values)
                {
                    sb.AppendLine("  window.name: " + win.Name + "<br>");
                    //if (win.SignalrCaller != null)
                      //  sb.AppendLine("SignalrCaller: " + win.SignalrCaller.ToGuidOrNull() + "<br>");
                }
            }

            sb.AppendLine("</div>");
            return new MvcHtmlString(sb.ToString());
        }
    }

    public class ChromeWindow
    {
        public dynamic SignalrCaller;
        public String ChromeSessionId;
        public String Name;
        public string ModelBindingId;
        public string RecordId;
        public DateTime CreateDate;

        public void ExecuteJavaScript(string script)
        {
            Thread.Sleep(1); // не удалять, иначе все глючит !!!
            SignalrCaller.receiveScript(script);
        }

        public void SetFocused()
        {
            ExecuteJavaScript("buhta.setBrowserTabFocused();");
        }

    }

    public class AppNavBarModel : BaseModel
    {
        public Guid UserID;
        public string ChromeSessionId;

        public DateTime LastRequestTime;

        public ConcurrentDictionary<string, ChromeWindow> ChromeWindows = new ConcurrentDictionary<string, ChromeWindow>();
        public ChromeWindow FocusedWindow;

        public AppNavBarModel(Controller controller, BaseModel parentModel) : base(controller, parentModel)
        {
            ShareMode = ModelShareMode.Session;
        }

        public void DestroyChromeWindow(string chromeWindowName)
        {
            ChromeWindow fakeWin;
            ChromeWindows.TryRemove(chromeWindowName, out fakeWin);
            Update();
        }

        bool okClicked;

        public virtual void CancelButtonClick(dynamic args)
        {
            Modal.Close();
        }

        public string GetOpenWindowsLabel()
        {
            var countStr = "";
            if (ChromeWindows.Count > 1)
                countStr = " (" + ChromeWindows.Count + ")";
            return "@открытые<br>окна" + countStr;
        }

        public void OpenSchemaDesigner(dynamic args)
        {
            foreach (var win in ChromeWindows.Values)
            {
                if (!string.IsNullOrEmpty(win.ModelBindingId))
                {
                    var model = AppServer.BindingModelList[win.ModelBindingId];
                    if (model is SchemaDesignerModel)
                    {
                        win.SetFocused();
                        return;
                    }
                }
            }

            var action = new OpenChildWindowAction();
            action.Url = "/Buhta/SchemaDesigner";
            ExecuteJavaScript(action.GetJsCode());

        }

        public void UpdateSchemaDesignerWindows(dynamic args)
        {
            foreach (var win in ChromeWindows.Values)
            {
                if (!string.IsNullOrEmpty(win.ModelBindingId))
                {
                    var model = AppServer.BindingModelList[win.ModelBindingId];
                    if (model is SchemaDesignerModel)
                    {
                        model.UpdateDatasets();
                    }
                }
            }
        }

        public void GotoNextWindow(dynamic args)
        {
            var wins = ChromeWindows.Values.OrderBy((w)=>w.CreateDate).ToList();

            for (int i=0; i<wins.Count;i++)
            {
                if (wins[i]==FocusedWindow)
                {
                    if (i < wins.Count - 1)
                        wins[i + 1].SetFocused();
                    else
                        wins[0].SetFocused();
                }
            }

        }

        public void GotoPrevWindow(dynamic args)
        {
            var wins = ChromeWindows.Values.OrderByDescending((w) => w.CreateDate).ToList();

            for (int i = 0; i < wins.Count; i++)
            {
                if (wins[i] == FocusedWindow)
                {
                    if (i < wins.Count - 1)
                        wins[i + 1].SetFocused();
                    else
                        wins[0].SetFocused();
                }
            }

        }
    }
}
