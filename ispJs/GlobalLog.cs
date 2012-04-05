using System;

namespace ispJs
{
    /// <summary>
    /// Provide a thread-safe log dumping.
    /// </summary>
	public static class GlobalLog
	{
        /// <summary>
        /// Fires the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
		public static void Fire (object obj, string format, params object[] args)
		{
			if (Fired != null) {
                lock (locker)
                {
                    Fired(string.Format("[{0}] {1}", DateTime.Now, string.Format(format, args)), obj);
                }
			}
		}
        static object locker = new object();
        /// <summary>
        /// Occurs when [fired].
        /// </summary>
		public static event FiredEventHandler Fired;
        /// <summary>
        /// 
        /// </summary>
		public delegate void FiredEventHandler (string msg,object obj);
	}
}

