using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint;
#if DEBUG
namespace ispJs
{
    /// <summary>
    /// Provide Javascript access to debugger.
    /// </summary>
    public class DebugProvider
    {
        /// <summary>
        /// Gets the debugger og specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static DebugProvider Get(string file)
        {
            if (!ispJs.WebApplication.Info.JSMLDebugProviders.ContainsKey(file))
            {
                ispJs.WebApplication.Info.JSMLDebugProviders[file] = new ispJs.DebugProvider();
            }
            return ispJs.WebApplication.Info.JSMLDebugProviders[file];
        }
        private JintEngine jint;
        internal DebugProvider()
        {
        }
        bool running = true;
        object runningLocker = new object();
        private Jint.Debugger.DebugInformation d;
        private bool _Running
        {
            get
            {
                lock (this.runningLocker)
                {
                    return this.running;
                }
            }
        }
        void jint_Break(object sender, Jint.Debugger.DebugInformation e)
        {
            lock (this.runningLocker)
            {
                this.running = false;
                this.d = e;
            }
            do
            {
                System.Threading.Thread.Sleep(200);
            } while (!this._Running);
        }
        /// <summary>
        /// Starts the debugger on specific Jin engine.
        /// </summary>
        /// <param name="jint">The jint.</param>
        /// <param name="charmapping">The charmapping.</param>
        /// <param name="source">The source.</param>
        public void Start(JintEngine jint, Dictionary<int, int> charmapping, string source)
        {
            if (jint == null)//Only server-side can use it.
            {
                return;
            }
            if (this.jint != null)
            {
                this.breakPoints = this.jint.BreakPoints;
            }

            this.jint = jint;
            this.jint.Break += new EventHandler<Jint.Debugger.DebugInformation>(jint_Break);
            this.jint.BreakPoints.AddRange(this.breakPoints);
            this.charmapping = charmapping;
            this.Source = source;
        }
        /// <summary>
        /// Resumes this instance.
        /// </summary>
        public void Resume()
        {
            lock (this.runningLocker)
            {
                this.running = true;
            }
        }
        private List<Jint.Debugger.BreakPoint> getJintBreakPoints()
        {
            if (this.jint == null)
            {
                return this.breakPoints;
            }
            return this.jint.BreakPoints;
        }
        /// <summary>
        /// Gets the locals.
        /// </summary>
        /// <value>The locals.</value>
        public LocalVariable[] Locals
        {
            get
            {
                var l = new List<LocalVariable>();
                if (this._Running)
                {
                    return l.ToArray();
                }
                foreach (var kvp in this.d.Locals.Where(ts => !WebApplication.Info.JSMLSystemLocals.Contains(ts.Key)))
                {
                    try
                    {
                        l.Add(new LocalVariable()
                        {
                            Name = kvp.Key,
                            Value = this.jint.Run("return JSON.stringify(" + kvp.Key + ",null,2);") as string,
                            Type = kvp.Value.Type,
                            Class = kvp.Value.Class
                        });
                    }
                    catch { }
                }
                return l.OrderBy(tl => tl.Name).ToArray();
            }
        }
        /// <summary>
        /// Gets the locals.
        /// </summary>
        /// <value>The locals.</value>
        public LocalVariable[] SystemLocals
        {
            get
            {
                var l = new List<LocalVariable>();
                if (this._Running)
                {
                    return l.ToArray();
                }
                foreach (var kvp in this.d.Locals.Where(ts => WebApplication.Info.JSMLSystemLocals.Contains(ts.Key)))
                {
                    l.Add(new LocalVariable()
                    {
                        Name = kvp.Key,
                        Value = (kvp.Value.Value as string ?? "") + "(" + kvp.Value.Type + ")",
                        Type = kvp.Value.Type,
                        Class = kvp.Value.Class
                    });
                }
                return l.OrderBy(tl => tl.Name).ToArray();
            }
        }
        /// <summary>
        /// Gets the break points.
        /// </summary>
        /// <value>The break points.</value>
        public ISPBreakPoint[] BreakPoints
        {
            get
            {
                return this.charmapping.Select(kvp =>
                {
                    return new ISPBreakPoint(kvp, this.getJintBreakPoints());
                }).ToArray();
            }
        }
        /// <summary>
        /// Gets the current position.
        /// </summary>
        /// <value>The current position.</value>
        public int CurrentPosition
        {
            get
            {
                if (this._Running)
                {
                    return -1;
                }
                if (this.d == null)
                {
                    return -1;
                }
                return this.BreakPoints.First(tjbp => tjbp.jintLine == d.CurrentStatement.Source.Start.Line).Position;
            }
        }
        /// <summary>
        /// Sets the break point condition.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="condition">The condition.</param>
        public void SetBreakPointCondition(int index, string condition)
        {
            this.BreakPoints[index].Condition = condition;
        }
        List<Jint.Debugger.BreakPoint> breakPoints = new List<Jint.Debugger.BreakPoint>();
        private Dictionary<int, int> charmapping;
        /// <summary>
        /// 
        /// </summary>
        public string Source;
        /// <summary>
        /// Provide Javascript access for locals.
        /// </summary>
        public class LocalVariable
        {
            /// <summary>
            /// 
            /// </summary>
            public string Name;
            /// <summary>
            /// 
            /// </summary>
            public string Value;
            /// <summary>
            /// 
            /// </summary>
            public string Type;
            /// <summary>
            /// 
            /// </summary>
            public string Class;
            /// <summary>
            /// Gets the short value.
            /// </summary>
            /// <value>The short value.</value>
            public string ShortValue
            {
                get
                {
                    var str = this.Value;
                    if (str == null)
                    {
                        return null;
                    }
                    if (str.Length > 10)
                    {
                        str = str.Remove(10) + "...";
                    }
                    return str;
                }
            }
            /// <summary>
            /// Gets the long value.
            /// </summary>
            /// <value>The long value.</value>
            public string LongValue
            {
                get
                {
                    if (this.Value == null)
                    {
                        return "null";
                    }
                    return this.Value.Replace("\n", "<br/>").Replace(" ", "&nbsp;");
                }
            }
        }
        /// <summary>
        /// Provide Javascript access for breakpoints in Jint engine.
        /// </summary>
        public class ISPBreakPoint
        {
            /// <summary>
            /// 
            /// </summary>
            public int Position;
            /// <summary>
            /// Gets or sets the condition.
            /// </summary>
            /// <value>The condition.</value>
            public string Condition
            {
                get
                {
                    var jintBreakPoint = this.b.FirstOrDefault(tb => tb.Line == this.jintLine);
                    if (jintBreakPoint == null)
                    {
                        return null;
                    }
                    if (jintBreakPoint.Condition == null)
                    {
                        return "true;";
                    }
                    return jintBreakPoint.Condition;
                }
                set
                {
                    this.b.RemoveAll(tb => tb.Line == this.jintLine);
                    if (value != null)
                    {
                        if (value == "true;")
                        {
                            this.b.Add(new Jint.Debugger.BreakPoint(this.jintLine, 1));
                        }
                        else
                        {
                            this.b.Add(new Jint.Debugger.BreakPoint(this.jintLine, 1, value));
                        }
                    }
                }
            }
            internal int jintLine;
            private List<Jint.Debugger.BreakPoint> b;
            internal ISPBreakPoint(KeyValuePair<int, int> charmap, List<Jint.Debugger.BreakPoint> jintBreakPoints)
            {
                this.jintLine = charmap.Value;
                this.Position = charmap.Key;
                this.b = jintBreakPoints;
            }
        }
    }
} 
#endif
