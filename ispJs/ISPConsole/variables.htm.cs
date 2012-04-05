using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if DEBUG
namespace ispJs.ISPConsole
{
    class variables : IISPRenderer, IISPAC
    {
        #region IISPRenderer 成员

        public void Page_Load(Dictionary<string, object> locals)
        {
            var dic = new Dictionary<string, string>();
            foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
            {
                dic.Add(key, System.Web.HttpContext.Current.Request.ServerVariables[key]);
            }
            locals.Add("variables", dic);
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
            WebApplication.OnConsolePageReading("/ISPConsole/variables.htm.isp.js", null);
        }

        #endregion
    }
}

#endif