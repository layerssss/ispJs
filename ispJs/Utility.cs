using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ispJs
{
    /// <summary>
    /// 
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Gets the path symbol(Linux/Windows).
        /// </summary>
        /// <value>The path symbol.</value>
        public static char PathSymbol
        {
            get{
                return pathSymbol.Value;
            }
        }
        private static Lazy<char> pathSymbol=new Lazy<char>(()=>{
            return AppDomain.CurrentDomain.BaseDirectory.StartsWith("/")?'/':'\\';
        },true);
        /// <summary>
        /// Trim the head if the string starts with the head.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="head">The head.</param>
        /// <returns></returns>
        public static string StringTrimStart(string origin, string head)
        {
            if (origin == null)
            {
                return "";
            }
            return origin.StartsWith(head) ?
                origin.Substring(head.Length)
                : origin;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        public class LazyDictionary<TKey, TValue> : Dictionary<TKey, TValue>
        {
            /// <summary>
            /// Lazies the get.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="fetching">The fetching.</param>
            /// <returns></returns>
            public TValue LazyGet(TKey key,Func<TValue> fetching){
                return this.ContainsKey(key) ? this[key] : fetching();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        public class EazyMap<TKey, TValue> : Dictionary<TKey, List<TValue>>
        {
            Func<TKey,List<TValue>> generator=(key)=>new List<TValue>();
            /// <summary>
            /// Gets or sets the <see cref="System.Collections.Generic.List&lt;TValue&gt;"/> with the specified key.
            /// </summary>
            /// <value></value>
            public new List<TValue> this[TKey key]
            {
                get
                {
                    return base.ContainsKey(key) ? base[key] : generator(key);
                }
                set
                {
                    base[key] = value;
                }
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="EazyMap&lt;TKey, TValue&gt;"/> class.
            /// </summary>
            /// <param name="generator">The generator.</param>
            public EazyMap(Func<TKey, List<TValue>> generator)
            {
                this.generator = generator;
            }
        }
        /// <summary>
        /// Simple string parser.
        /// </summary>
        public class StringFetcher
        {
            private string buffer;
            /// <summary>
            /// Current parsing position.
            /// </summary>
            public int Position;
            StringFetcher()
            {
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="StringFetcher"/> class.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            public StringFetcher(string buffer)
            {
                this.buffer = buffer;
                this.Position = 0;
            }
            /// <summary>
            /// Fetches the specified target.
            /// </summary>
            /// <param name="target">The target.</param>
            /// <param name="padding">if set to <c>true</c>, will also parse the target.</param>
            /// <returns></returns>
            public string Fetch(string target, bool padding)
            {
                var i = buffer.IndexOf(target, this.Position);
                if (i == -1)
                {
                    return null;
                }
                var oldp = Position;
                this.Position = i;
                if (padding)
                {
                    this.Position += target.Length;
                }
                return this.buffer.Substring(oldp, this.Position - oldp);
            }
            /// <summary>
            /// Gets the string left.
            /// </summary>
            /// <value>The left.</value>
            public string Left
            {
                get
                {
                    return this.buffer.Substring(this.Position);
                }
            }
        }

        /// <summary>
        /// Creates the folder for.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void CreateFolderFor(string file)
        {
            CreateFolderFor(new System.IO.DirectoryInfo(file.Remove(file.LastIndexOf(Utility.PathSymbol))));
            
        }
        /// <summary>
        /// Creates the folder for.
        /// </summary>
        /// <param name="di">The di.</param>
        public static void CreateFolderFor(System.IO.DirectoryInfo di)
        {
            if (!di.Parent.Exists)
            {
                CreateFolderFor(di.Parent);
            }
            di.Create();
        }
    }
}
