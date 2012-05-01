using System;
using System.Reflection;
using System.Collections.Generic;
using System.Web;
namespace ispJs
{
    /// <summary>
    /// 
    /// </summary>
	internal class WebAppInfo
	{
		public WebAppInfo ()
		{
			
		}
        /// <summary>
        /// The root path.
        /// </summary>
		public string Root;
        public Dictionary<string, Func<HttpRequest, Dictionary<string, object>>> Actions = new Dictionary<string, Func<HttpRequest, Dictionary<string, object>>>();
#if DEBUG
        public List<ActionInfo> ActionsPI = new List<ActionInfo>(); 
#endif
        public Utility.LazyDictionary<string, IISPRenderer> Pages = new Utility.LazyDictionary<string, IISPRenderer>();
        public Utility.LazyDictionary<string, DateTime> JSMLRendered = new Utility.LazyDictionary<string, DateTime>();
        public string JSMLScripts;
#if DEBUG
        public string JSMLScriptsForReferences;
#endif
        public string JSMLFootScripts;
        public BullyFerry JSMLFerry = new BullyFerry();
        /// <summary>
        /// 
        /// </summary>
        public string JSMLErrorPage;
        /// <summary>
        /// JSML->JSML[]
        /// </summary>
        public Utility.EazyMap<string, string> JSMLSourceCodeMapping = new Utility.EazyMap<string, string>((tpath) => { var l = new List<string>(); l.Add(tpath); return l; });
#if DEBUG
        /// <summary>
        /// JSML->
        /// </summary>
        public Utility.LazyDictionary<string, DebugProvider> JSMLDebugProviders = new Utility.LazyDictionary<string, DebugProvider>(); 
#endif
        /// <summary>
        /// JSML->
        /// </summary>
        public Utility.LazyDictionary<string, List<Jint.Debugger.BreakPoint>> JSMLDebugBreakPoints = new Utility.LazyDictionary<string, List<Jint.Debugger.BreakPoint>>();
        /// <summary>
        /// 
        /// </summary>
        public List<SubPage> SubPages = new List<SubPage>();
#if DEBUG
        public IEnumerable<string> JSMLSystemLocals; 
#endif
        public class SubPage
        {
            public string Folder;
            public string Extension;
            public bool Match(string path)
            {
                return path.StartsWith(this.Folder) 
                    && path.EndsWith('.' + this.Extension)
                    &&!this.GetValue(path).Contains("/");
            }
            public string GetValue(string path)
            {
                return path.Remove(path.Length - this.Extension.Length - 1).Substring(this.Folder.Length);
            }
        }
#if DEBUG
        public class ActionInfo
        {
            public string Name;
            public string Comment = "No document available...";
            public List<ParameterInfo> Parameters = new List<ParameterInfo>();
            public List<ParameterInfo> Outputs = new List<ParameterInfo>();
        }
        public class ParameterInfo
        {
            public string Type;
            public string Name;
            public string DefaultValue;
            public string Comment = "No document available...";
        } 
#endif
	}
}

