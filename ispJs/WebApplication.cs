using System;
using System.Web;
using System.Threading;
using System.Collections.Generic;
using Jint;
using System.IO;
using System.Reflection;
using System.Linq;

namespace ispJs
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebApplication
    {

        /// <summary>
        /// Handles the Application_RequestBegin event.
        /// </summary>
        public static void HandleBeginRequest()
        {
            while (!started)
            {
                Thread.Sleep(200);
            }
            var context = HttpContext.Current;
            var server = context.Server;
            var request = context.Request;
            var response = context.Response;
            var path = PathResolving(request.Path).TrimStart('/');
            var refere = request.UrlReferrer;
            #region Actions
            if (Info.Actions.ContainsKey(path))
            {
                Dictionary<string, object> results;
                var req = request["redirect"];
                if (req != null && req.Trim() == "")
                {
                    req = request.UrlReferrer.ToString();
                    if (req == "")
                    {
                        req = "noref";
                    }
                }
                try
                {
                    results = Info.Actions[path](request);
                    if (req == null)
                    {
                        response.StatusCode = 200;
                    }
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException)
                    {
                        ex = (ex as TargetInvocationException).InnerException;
                    }
                    if (ex is System.Threading.ThreadAbortException)
                    {
                        return;
                    }
                    response.TrySkipIisCustomErrors = true;
                    if (req != null)
                    {
                        response.Redirect(ErrorPath + "?message=" + server.UrlEncode(ex.Message) + "&action=" + server.UrlEncode(path) + (refere != null ? "&from=" + server.UrlEncode(refere.ToString()) : ""));
                        return;
                    }
                    if (request["callback"] != null)
                    {
                        response.ContentType = "text/javascript";
                        response.Write(request["callback"] + "({'error':" +
                            fastJSON.JSON.Instance.ToJSON(ex.Message) + "});");
                        response.StatusCode = 200;
                    }
                    else
                    {
                        response.StatusCode = 500;
                        response.ContentType = "text/plain";
                        response.Write(ex.Message);
                    }
                    //response.Write(ex.InnerException.ToString());
                    response.End();
                    return;
                }
                if (req != null)
                {
                    foreach (var key in results.Keys)
                    {
                        req = req.Replace("{" + key + "}",HttpUtility.UrlEncode( results[key].ToString()));
                    }
                    if (req == "noref")
                    {
                        response.StatusCode = 200;
                        response.ContentType = "text/html";
                        response.Write("<script type=\"text/javascript \">window.location.reload(history.go(-1));</script>");
                        response.End();
                    }
                    else
                    {
                        response.Clear();
                        response.Redirect(req,true);
                    }
                    return;
                }
                if (request.AcceptTypes!=null&&request.AcceptTypes.Contains("text/html"))
                {

                    fastJSON.JSON.Instance.IndentOutput = true;
                    response.ContentType = "text/html";
                    response.Write(server.HtmlEncode(fastJSON.JSON.Instance.ToJSON(results)).Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;").Replace("\r\n", "<br />"));
                }
                else
                {
                    if (request["callback"]!=null)//JSONP
                    {
                        response.ContentType = "text/javascript";
                        response.Write(request["callback"]);
                        response.Write('(');
                        response.Write(fastJSON.JSON.Instance.ToJSON(results));
                        response.Write(");");
                    }
                    else
                    {
                        response.ContentType = "application/json";
                        response.Write(fastJSON.JSON.Instance.ToJSON(results));
                    }
                    
                }
                fastJSON.JSON.Instance.IndentOutput = false;
                response.End();
            }
            #endregion
            var browsingFolder = false;
            if (path.EndsWith("default.htm"))
            {
                path = path.Remove(path.Length - 11) + "Default.htm";
            }
            if (path.EndsWith("/") || path == "")
            {
                path += "Default.htm";
                browsingFolder = true;

            }
            #region ISPHTML
            var ispFile = new FileInfo(Info.Root + path.Replace('/', Utility.PathSymbol) + ".isp.js");
            var originFile = new FileInfo(Info.Root + path.Replace('/', Utility.PathSymbol));

            #region SubPages
            var subPage = Info.SubPages.FirstOrDefault(tkvp => path.StartsWith(tkvp.Folder) && path.EndsWith('.' + tkvp.Extension));
            string subPagev=null;
            var ispPath = path + ".isp.js";
            if (!ispFile.Exists && subPage != null)
            {
                ispPath = subPage.Folder + subPage.Extension + ".isp.js";
                ispFile = new FileInfo(Info.Root + subPage.Folder.Replace('/', Utility.PathSymbol) + subPage.Extension + ".isp.js");
                originFile = new FileInfo(Info.Root + path.Replace('/', Utility.PathSymbol));
                subPagev = path.Remove(path.Length - subPage.Extension.Length - 1).Substring(subPage.Folder.Length);
            }
            else
            {
                subPage = null;
            }
            #endregion

            if (ispFile.Exists)
            {
                var updated = false;
                var handler = Info.Pages.LazyGet(ispPath, () => null);
                IISPAC ac=null;
                try
                {
                    ac = (IISPAC)handler;
                }
                catch (InvalidCastException)
                {
                }
                if (ac != null)
                {
                    try
                    {
                        ac.Page_Read(subPagev);
                    }
                    catch (NotImplementedException)
                    {
                    }
                    catch(AccessDeniedException ex)
                    {
                        if (ex.Redirect == null)
                        {
                            response.Redirect(ErrorPath + "?message=" + server.UrlEncode(ex.Message) + (refere != null ? "&from=" + server.UrlEncode(refere.AbsolutePath) : ""));
                        }
                        else
                        {
                            response.Redirect(ErrorPath + "?message=" + server.UrlEncode(ex.Message) + (refere != null ? "&from=" + server.UrlEncode(ex.Redirect) : ""));
                        }
                        return;
                    }
                    
                }
#if DEBUG
#else

                if ((updated = Info.JSMLSourceCodeMapping[ispPath].Any(str =>//Source code updated
                        Info.JSMLRendered.LazyGet(str, () => DateTime.MinValue) != File.GetLastWriteTime(Info.Root + str.Replace('/', Utility.PathSymbol))))
                    || !originFile.Exists
                    )
#endif

                {
                    if (Info.JSMLFerry.Call(path))
                    {
                        if (originFile.Exists)
                        {
                            originFile.Delete();
                        }
                        var initLocals = new Dictionary<string, object>();
                        initLocals.Add("$_native_web_Root", Info.Root);
                        initLocals.Add("$_native_loadedJS", ispPath);
                        initLocals.Add("$cur", ispPath);
                        if (subPage != null)
                        {
                            initLocals.Add("$subPage", subPagev);
                            if (updated)
                            {
                                foreach (var file in Directory.GetFiles(Info.Root + subPage.Folder.Replace('/', Utility.PathSymbol), "*." + subPage.Extension))
                                {
                                    File.Delete(file);
                                }
                            }
                        }
                        if (handler != null)
                        {
                            try
                            {
                                handler.Page_Load(initLocals);
                            }
#if DEBUG
#else
                            catch
                            {
                                Info.JSMLFerry.Finish(path);
                                return;
                            } 
#endif
                            finally
                            {
                                Info.JSMLFerry.Finish(path);//Must release the FERRY.
                            }
                        }

                        Dictionary<int, int> charmapping = new Dictionary<int, int>();
                        string source = "";
                        string js;
                        try
                        {
                            js = LoadJS(ispPath, out charmapping, out source);
                        }
                        catch (Exception ex)
                        {
                            js = "$('" + server.HtmlEncode(ex.Message) + "')";
                        }
                        StreamWriter writer;
                        for (var i = 0;; i++)
                        {
                            try
                            {
                                writer = File.CreateText(originFile.FullName);
                                break;
                            }
                            catch(Exception ex)
                            {
                                if (i > 10)
                                {
                                    throw (ex);
                                }
                                Thread.Sleep(400);
                            }
                        }
                        var jint = new JintEngine()
                            .AddPermission(new System.Security.Permissions.FileIOPermission(System.Security.Permissions.PermissionState.Unrestricted))
                            .SetFunction("$_native_loadJS", new Func<string, string>((str) =>
                            {
                                Dictionary<int, int> dummyDic;
                                string dummySource;
                                return LoadJS(str, out dummyDic, out dummySource);
                            }))
                            .SetFunction("urlEncode", new Func<string, string>(server.UrlEncode))
                            .SetFunction("urlDecode", new Func<string, string>(server.UrlDecode))
                            .SetFunction("htmlEncode", new Func<string, string>((str)=>server.HtmlEncode(str).Replace("\r\n","<br/>")))
                            .SetFunction("htmlDecode", new Func<string, string>((str)=>server.HtmlDecode(str).Replace("<br/>","\r\n")))
                            .SetFunction("$_native_writer_write", new Action<string>((str) => { writer.Write(str); }));
#if DEBUG
                        var reference = Info.Root + "ISPReferences" + Utility.PathSymbol + ispPath.Replace('/', Utility.PathSymbol);
                        Utility.CreateFolderFor(reference);
                        File.WriteAllText(reference, Info.JSMLScriptsForReferences);
#endif
                        foreach (var key in initLocals.Keys)
                        {
                            var assign = "var " + key + "=" +
                                fastJSON.JSON.Instance.ToJSON(initLocals[key]) + ";";
#if DEBUG

                            File.AppendAllText(reference, "\r\n" + assign, System.Text.Encoding.UTF8);
#endif
                            jint.Run(assign);
                        }
#if DEBUG
                        DebugProvider.Get(ispPath).Start(jint, charmapping, source);
#endif
                        try
                        {
                            jint.Run(Info.JSMLScripts);
#if DEBUG
                            jint = jint.SetDebugMode(true);
#endif
                            jint.Run(js);
#if DEBUG
                            jint = jint.SetDebugMode(false);
#endif
                            jint.Run(Info.JSMLFootScripts);
                            writer.Flush();
                            writer.Close();
                        }
                        catch (Exception ex)
                        {
                            writer.Close();
                            var jsEx = (ex is JintException) ? ex.InnerException as Jint.Native.JsException : ex;
#if DEBUG
                            File.WriteAllText(originFile.FullName,
                                string.Format(Info.JSMLErrorPage, server.HtmlEncode(ex.Message).Replace("\r\n", "<br />"), server.HtmlEncode(js).Replace("\r\n", "<br />")), System.Text.Encoding.UTF8);
#else
                            File.Delete(originFile.FullName);
#endif
                        }
                        var mappeds = (jint.Run("$_native_loadedJS") as string).Split('|').ToList();
                        Info.JSMLSourceCodeMapping[ispPath] = mappeds;
                        foreach (var mapped in mappeds)
                        {
                            Info.JSMLRendered[mapped] = File.GetLastWriteTime(Info.Root + mapped.Replace('/', Utility.PathSymbol));
                        }
                        Info.JSMLFerry.Finish(path);
                    }
                    if (browsingFolder)
                    {
                        response.ContentType = "text/html";
                        response.WriteFile(originFile.FullName);
                        response.End();
                    }
                }
            }
            #endregion
        }
        private static string LoadJS(string ispPath, out Dictionary<int, int> charmapping, out string source)
        {
            var sb = new System.Text.StringBuilder("\r\n\r\n");
            source = File.ReadAllText(Info.Root + ispPath.Replace('/', Utility.PathSymbol), System.Text.Encoding.UTF8);
            var parser = new Utility.StringFetcher(source);
            string tmp;
            charmapping = new Dictionary<int, int>();

            tmp = parser.Fetch("/*<!--*/", true);


#if DEBUG
            var line = 3 + tmp.Count(tc => tc == '\n');

            var p = parser.Position;
#endif
            while ((tmp = parser.Fetch("/*-->", false)) != null)
            {
                parser.Position += 5;
                sb.Append(tmp);
#if DEBUG
                {
                    var i = 0;
                    do
                    {
                        charmapping.Add(p + i, line); line++;
                    } while ((i = tmp.IndexOf("\r\n", i) + 2) != 1);
                }
#endif


                tmp = parser.Fetch("<!--*/", false);
                parser.Position += 6;
                sb.Append("$(");
                sb.Append(fastJSON.JSON.Instance.ToJSON(tmp).Replace("{$","\");$(").Replace("$}",");$(\""));
                sb.Append(");\r\n");

#if DEBUG
                p = parser.Position;
#endif
            }


            tmp = parser.Fetch("//-->", false);
            if (tmp == null)
            {
                throw (new JSMLParsingException("File should ends with \"//-->\""));
            }

            parser.Position += 5;
            sb.Append(tmp);
#if DEBUG
            {
                var i = 0;
                do
                {
                    charmapping.Add(p + i, line); line++;
                } while ((i = tmp.IndexOf("\r\n", i) + 2) != 1);
            }
#endif

            if (parser.Left.Trim().Length != 0)
            {
                throw (new JSMLParsingException("File should ends with \"//-->\""));
            }

            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        internal class JSMLParsingException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="JSMLParsingException"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            public JSMLParsingException(string message)
                : base(message)
            {
            }
        }
        /// <summary>
        /// Registers an new ACTION.
        /// </summary>
        /// <param name="webNamespace">The web namespace.</param>
        /// <param name="type">The type of ACTION.</param>
        public static void RegisterActions(string webNamespace, System.Type type)
        {
            webNamespace = webNamespace.TrimEnd('.');
#if DEBUG
            System.Xml.Linq.XDocument xmlDoc = null;
            try
            {
                xmlDoc = System.Xml.Linq.XDocument.Load(ActionCommentsXmlPath);
            }
            catch { } 
#endif
            var instance = new Lazy<object>(() =>
            {
                return type.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            }, true);
            foreach (var m in type.GetMethods())
            {
                if (m.IsStatic||!m.GetCustomAttributes(false).Any(to=>{
                    return to is ActionAttribute;
                }))
                {
                    continue;
                }
                var method = m;
                var pars = method.GetParameters();
                var action = (Utility.StringTrimStart(type.Namespace, webNamespace).TrimEnd('.').Replace('.', '/') + '/'
                    + (type.Name == "Default" ? "" : type.Name + '/')
                     + method.Name.TrimStart('_')).TrimStart('/');

#if DEBUG

                var mi = new WebAppInfo.ActionInfo();
                mi.Name = action;
                System.Xml.Linq.XElement xmlM = null;
                if (xmlDoc != null)
                {
                    xmlM=xmlDoc.Descendants("doc").FirstOrDefault().Descendants("member").FirstOrDefault(tm => tm.Attribute("name").Value.StartsWith("M:" + type.FullName + "." + method.Name));
                    if (xmlM != null)
                    {
                        mi.Comment = xmlM.Element("summary").Value;
                    }

                }
                foreach (var p in pars)
                {
                    var pi = new WebAppInfo.ParameterInfo()
                    {
                        Name = p.Name,
                        DefaultValue = (p.DefaultValue ?? "").ToString(),
                        Type = p.ParameterType.ToString()
                    };
                    if (xmlM != null)
                    {
                        var xmlP = xmlM.Elements("param").FirstOrDefault(tp => tp.Attribute("name").Value == p.Name);
                        if (xmlP != null)
                        {
                            pi.Comment = xmlP.Value;
                        }
                    }
                    if (p.IsOut)
                    {
                        mi.Outputs.Add(pi);
                    
                    }else{
                        mi.Parameters.Add(pi);
                    }
                }
                Info.ActionsPI.Add(mi);
                

#endif


                Info.Actions.Add(action
                    ,
                    (request) =>
                    {
                        var args = new object[pars.Length];
                        for (var i = 0; i < pars.Length; i++)
                        {
                            if (!pars[i].IsOut)
                            {
                                if (pars[i].ParameterType == typeof(HttpPostedFile))
                                {
                                    args[i] = request.Files[pars[i].Name];
                                    continue;
                                }
                                var str = request[pars[i].Name] ?? (pars[i].IsOptional ? pars[i].DefaultValue : "") as string;
                                if (pars[i].ParameterType == typeof(Int32))
                                {
                                    args[i] = Convert.ToInt32(str==""?"0":str);
                                }
                                if (pars[i].ParameterType == typeof(Int64))
                                {
                                    args[i] = Convert.ToInt64(str == "" ? "0" : str);
                                }
                                if (pars[i].ParameterType == typeof(String))
                                {
                                    args[i] = str;
                                    if (request[pars[i].Name] == null)
                                    {
                                        args[i] = null;
                                    }
                                }
                                if (pars[i].ParameterType == typeof(Boolean))
                                {
                                    args[i] = str != "" && str.ToLower() != "false";
                                }
                                if (pars[i].ParameterType == typeof(Single))
                                {
                                    args[i] = Convert.ToSingle(str == "" ? "0" : str);
                                }
                                if (pars[i].ParameterType == typeof(Double))
                                {
                                    args[i] = Convert.ToDouble(str == "" ? "0" : str);
                                }
                            }
                        }
                        method.Invoke(instance.Value, args);
                        var results = new Dictionary<string, object>();
                        for (var i = 0; i < pars.Length; i++)
                        {
                            if (pars[i].IsOut)
                            {
                                results[pars[i].Name] = args[i];
                            }
                        }
                        return results;
                    });
            }
        }
        /// <summary>
        /// Gets the server.
        /// </summary>
        /// <value>The server.</value>
        public static HttpServerUtility Server
        {
            get
            {
                return HttpContext.Current.Server;
            }
        }
        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>The request.</value>
        public static HttpRequest Request
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>The response.</value>
        public static HttpResponse Response
        {
            get
            {
                return HttpContext.Current.Response;
            }
        }
        /// <summary>
        /// Registers a new SUBPAGE.
        /// </summary>
        /// <param name="ispFile">The isp file. Its name should be htm.isp or abc.isp(abc is its exitension).</param>
        public static void RegisterSubPage(string ispFile)
        {
            if (!ispFile.EndsWith(".isp.js"))
            {
                throw (new Exception("ISP file required!"));
            }
            var i = ispFile.LastIndexOf('/') + 1;
            Info.SubPages.Add(new WebAppInfo.SubPage()
            {
                Folder = ispFile.Remove(i),
                Extension = ispFile.Substring(i).Remove(ispFile.Length - i - ".isp.js".Length),
            });
        }
        /// <summary>
        /// Registers the page handler class.
        /// </summary>
        /// <param name="ispFile">The isp file.</param>
        /// <param name="handler">The handler.</param>
        public static void RegisterRenderer(string ispFile, IISPRenderer handler)
        {
            Info.Pages.Add(ispFile, handler);
        }
        /// <summary>
        /// Handles the Application_Start event.
        /// </summary>
        /// <param name="server">The Server helper object.</param>
        public static void HandleStart(HttpServerUtility server)
        {
            if (started)
            {
                return;
            }
            OnConsolePageReading += new Action<string, string>(WebApplication_OnConsolePageReading);
            
            GlobalLog.Fired -= onGlobalLogFired;
            GlobalLog.Fired += onGlobalLogFired;
            Info.Root = server.MapPath("~/").TrimEnd(Utility.PathSymbol) + Utility.PathSymbol;
            var sr = new StreamReader(Assembly.GetAssembly(typeof(WebApplication)).GetManifestResourceStream("ispJs.RuntimeScripts.isp_runtime.js"));
            Info.JSMLScripts = sr.ReadToEnd();
            sr.Close();
            sr = new StreamReader(Assembly.GetAssembly(typeof(WebApplication)).GetManifestResourceStream("ispJs.RuntimeScripts.isp_runtime_json.js"));
            Info.JSMLScripts += sr.ReadToEnd();
            sr.Close();


#if DEBUG
            Info.JSMLScriptsForReferences = Info.JSMLScripts;
#endif
            sr = new StreamReader(Assembly.GetAssembly(typeof(WebApplication)).GetManifestResourceStream("ispJs.RuntimeScripts.isp_runtime_foot.js"));
            Info.JSMLFootScripts = sr.ReadToEnd();
            sr.Close();
            sr = new StreamReader(Assembly.GetAssembly(typeof(WebApplication)).GetManifestResourceStream("ispJs.RuntimePages.JavascriptError.htm"));
            Info.JSMLErrorPage = sr.ReadToEnd();
            sr.Close();

#if DEBUG
            sr = new StreamReader(Assembly.GetAssembly(typeof(WebApplication)).GetManifestResourceStream("ispJs.RuntimeScripts.isp_runtime_systemLocals.txt"));
            Info.JSMLSystemLocals = sr.ReadToEnd().Split('\n').Select(ts => ts.Split(new[] { ' ', '\t' })[0]);
            sr.Close();
            if (ReloadConsole)
            {
                if (Directory.Exists(Info.Root + "ISPConsole"))
                {
                    Directory.Delete(Info.Root + "ISPConsole", true);
                }
                Directory.CreateDirectory(Info.Root + "ISPConsole");
                var assembly = Assembly.GetAssembly(typeof(WebApplication));
                foreach (var resName in assembly.GetManifestResourceNames().Where(str => str.StartsWith("ispJs.ISPConsole.")))
                {

                    var stream = assembly.GetManifestResourceStream(resName);
                    var target = File.Create(Info.Root + "ISPConsole" + Utility.PathSymbol + resName.Substring("ispJs.ISPConsole.".Length));
                    var buffer = new byte[1024];
                    var i = 0;
                    while ((i = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        target.Write(buffer, 0, i);
                    }
                    target.Flush();
                    target.Close();
                    stream.Close();
                }
            }

            RegisterActions("ispJs", typeof(ISPDebug.Default));
            RegisterRenderer("ISPConsole/Default.htm.isp.js", new ISPConsole.Default());
            RegisterRenderer("ISPConsole/debuggers.htm.isp.js", new ISPConsole.debuggers());
            RegisterSubPage("ISPConsole/debug.htm.isp.js");
            RegisterSubPage("ISPConsole/action.htm.isp.js");
            RegisterRenderer("ISPConsole/action.htm.isp.js", new ISPConsole.action());
            RegisterRenderer("ISPConsole/actions.htm.isp.js", new ISPConsole.actions());
            RegisterRenderer("ISPConsole/dependencies.htm.isp.js", new ISPConsole.dependencies());
            RegisterRenderer("ISPConsole/variables.htm.isp.js", new ISPConsole.variables());

#endif

            fastJSON.JSON.Instance.UseSerializerExtension = false;
            fastJSON.JSON.Instance.ShowReadOnlyProperties = true;
            started = true;
        }

        static void WebApplication_OnConsolePageReading(string arg1, string arg2)
        {
        }
        static bool started = false;
        static void onGlobalLogFired(string msg, object obj)
        {
            System.IO.File.AppendAllText(Info.Root + LogPath,  msg+"\r\n" );
        }
        internal static WebAppInfo Info = new WebAppInfo();
        /// <summary>
        /// Handles the Application_End event.
        /// </summary>
        public static void HandleEnd()
        {
            if (!started)
            {
                return;
            }
            started = false;
#if DEBUG
            if (ReloadConsole)
            {
                try
                {
                    Directory.Delete(Info.Root + "ISPConsole", true);
                }
                catch { }
            }
#endif
        }
        /// <summary>
        /// Safes the delete.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void SafeDelete(string file)
        {
            Info.JSMLFerry.Bully(file,"");
            try
            {
                for (var i = 0; ; i++)
                {
                    try
                    {
                        System.IO.File.Delete(Info.Root + file.Replace('/', Utility.PathSymbol));
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i > 10)
                        {
                            throw (ex);
                        }
                        Thread.Sleep(DeletionGap / 3);
                    }
                }
            }
            finally
            {
                Info.JSMLFerry.FinishBullying(file,"");
            }
            Thread.Sleep(DeletionGap);
        }
        /// <summary>
        /// Safes the delete.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="suffix">The suffix.</param>
        public static void SafeDelete(string folder, string suffix)
        {
            Info.JSMLFerry.Bully(folder, suffix);
            try
            {
                foreach (var file in Directory.GetFiles(Info.Root + folder.Replace('/', Utility.PathSymbol), '*' + suffix))
                {
                    for (var i = 0; ; i++)
                    {
                        try
                        {
                            System.IO.File.Delete(file);
                            break;
                        }
                        catch (Exception ex)
                        {
                            if (i > 10)
                            {
                                throw (ex);
                            }
                            Thread.Sleep(DeletionGap/3);
                        }
                    }
                }
            }
            finally
            {
                Info.JSMLFerry.FinishBullying(folder, suffix);
            }
            Thread.Sleep(DeletionGap);
        }
        /// <summary>
        /// 
        /// </summary>
        public static string LogPath;
        /// <summary>
        /// Make the webserver 'sleeps' certain miliseconds to prevent invalid cache, pretty smells...
        /// </summary>
        public static int DeletionGap;
        /// <summary>
        /// 
        /// </summary>
        public static string ErrorPath = "/Error.htm";
        /// <summary>
        /// 
        /// </summary>
        public static string ActionCommentsXmlPath = "";
        /// <summary>
        /// 
        /// </summary>
        public static bool ReloadConsole = true;
        /// <summary>
        /// 
        /// </summary>
        public static Action<string, string> OnConsolePageReading;

        /// <summary>
        /// 
        /// </summary>
        public static Func<string, string> PathResolving = (str) => str;
    }
}

