using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if DEBUG
namespace ispJs.ISPConsole
{
    class dependencies : IISPRenderer, IISPAC
    {

        #region IISPRenderer 成员

        public void Page_Load(Dictionary<string, object> locals)
        {
            locals.Add("dependencies", WebApplication.Info.JSMLSourceCodeMapping);
            locals.Add("rendered", WebApplication.Info.JSMLRendered);
            var dic = new Dictionary<string, DateTime>();
            foreach (var l in WebApplication.Info.JSMLSourceCodeMapping.Values)
            {
                foreach (var f in l)
                {
                    dic[f] = System.IO.File.GetLastWriteTime(WebApplication.Info.Root + f.Replace('/', Utility.PathSymbol));
                }
            }
            locals.Add("version", dic);
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
            WebApplication.OnConsolePageReading("/ISPConsole/dependencies.htm.isp.js", null);
        }

        #endregion
    }
}

#endif