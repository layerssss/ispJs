using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if DEBUG

namespace ispJs.ISPDebug
{
    class Default
    {
        [Action]
        public void _(string file, string command, out object status)
        {
            var jint = new Jint.JintEngine()
                .SetFunction("debug", new Func<ispJs.DebugProvider>(() =>
                {
                    return ispJs.DebugProvider.Get(file);
                }));
            jint.Run(command);
            try
            {
                status = ispJs.WebApplication.Info.JSMLDebugProviders[file];
            }
            catch (KeyNotFoundException)
            {
                var str = "";
                foreach (var key in ispJs.WebApplication.Info.JSMLDebugProviders.Keys)
                {
                    str += key + ';';
                }
                throw (new Exception("Debug infomation not found...(" + str + ')'));
            }
            //var d = de.d.Locals.;

        }
    }
}

#endif