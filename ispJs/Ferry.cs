using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
namespace ispJs
{
    /// <summary>
    /// Ferry model.
    /// Provide a muti-channel ferry model.
    /// </summary>
    public class Ferry
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="Ferry"/> class.
        /// </summary>
		public Ferry ()
		{		
		}
        /// <summary>
        /// Calls the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
		public virtual bool Call(string line)
        {
            lock (this.processing)
            {
                if (!this.processing.Contains(line))
                {
                    this.processing.Add(line);
                    return true;
                }
            }
			do {
                lock (this.processing)
                {
                    if (!this.processing.Contains(line))
                    {
                        break;
                    }
                }
				Thread.Sleep (100);

			} while(true);
			return false;
		}
        /// <summary>
        /// Finishes the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
		public void Finish (string line)
		{
			lock (this.processing) {
				this.processing.Remove (line);
			}
		}
        /// <summary>
        /// 
        /// </summary>
        protected List<string> processing = new List<string>();
	}
}

