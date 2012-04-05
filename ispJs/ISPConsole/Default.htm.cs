using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if DEBUG
namespace ispJs.ISPConsole
{
    class Default : IISPRenderer, IISPAC
    {

        #region IISPRenderer 成员

        public void Page_Load(Dictionary<string, object> locals)
        {
            locals.Add("subPages", WebApplication.Info.SubPages.Where(tk => !tk.Folder.StartsWith("ISPConsole")).ToArray());
            locals.Add("actions", WebApplication.Info.Actions.Keys.Where(tk => !tk.StartsWith("ISPConsole")).ToArray());
            locals.Add("renderers", WebApplication.Info.Pages.Keys.Where(tk => !tk.StartsWith("ISPConsole")).ToArray());
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
        }

        #endregion

        #region IISPAC 成员

        public void Page_Read(string subPage)
        {
            WebApplication.OnConsolePageReading("/ISPConsole/Default.htm.isp.js", null);
        }

        #endregion
    }
}

#endif