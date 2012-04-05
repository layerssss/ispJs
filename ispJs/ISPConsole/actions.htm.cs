using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if DEBUG
namespace ispJs.ISPConsole
{
    class actions : IISPRenderer, IISPAC
    {
        #region IISPRenderer 成员

        public void Page_Load(Dictionary<string, object> locals)
        {

            locals.Add("actions", WebApplication.Info.ActionsPI.Where(tk => !tk.Name.StartsWith("ISPConsole") && !tk.Name.StartsWith("ISPDebug")).ToArray());
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region IISPAC 成员

        public void Page_Read(string subPage)
        {
            WebApplication.OnConsolePageReading("/ISPConsole/actions.htm.isp.js", null);
        }

        #endregion
    }
}

#endif