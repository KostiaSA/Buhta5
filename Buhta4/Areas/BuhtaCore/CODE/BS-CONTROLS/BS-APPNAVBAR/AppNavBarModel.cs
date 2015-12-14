using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    if (win.SignalrCaller != null)
                        sb.AppendLine("SignalrCaller: " + win.SignalrCaller.ToGuidOrNull() + "<br>");
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
    }

    public class AppNavBarModel : BaseModel
    {
        public Guid UserID;
        public string ChromeSessionId;

        public DateTime LastRequestTime;

        public ConcurrentDictionary<string, ChromeWindow> ChromeWindows = new ConcurrentDictionary<string, ChromeWindow>();

        public AppNavBarModel(Controller controller, BaseModel parentModel) : base(controller, parentModel) { }

        public void DestroyChromeWindow(string chromeWindowName)
        {
            ChromeWindow fakeWin;
            ChromeWindows.TryRemove(chromeWindowName, out fakeWin);
        }

        bool okClicked;

        public virtual void CancelButtonClick(dynamic args)
        {
            Modal.Close();
        }

    }
}
