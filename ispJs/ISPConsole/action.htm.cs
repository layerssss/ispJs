using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if DEBUG
namespace ispJs.ISPConsole
{
    class action : IISPRenderer, IISPAC
    {
        #region IISPRenderer 成员

        public void Page_Load(Dictionary<string, object> locals)
        {

            var str = (locals["$subPage"] as string).Replace('_', '/');
            locals.Add("action", WebApplication.Info.ActionsPI.First(tp => tp.Name == str));
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
            WebApplication.OnConsolePageReading("/ISPConsole/action.htm.isp.js", subPage.Replace('_', '/'));
        }

        #endregion
    }
}

#endif