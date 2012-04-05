using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if DEBUG

namespace ispJs.ISPConsole
{
    class debuggers : IISPRenderer, IISPAC
    {
        #region IISPRenderer 成员

        public void Page_Load(Dictionary<string, object> locals)
        {
            locals.Add("debuggers", WebApplication.Info.JSMLDebugProviders);
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
            WebApplication.OnConsolePageReading("/ISPConsole/debuggers.htm.isp.js", null);
        }

        #endregion
    }
}

#endif