using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ispJs
{
    /// <summary>
    /// 
    /// </summary>
    public class BullyFerry:Ferry
    {
        /// <summary>
        /// Calls the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        public override bool Call(string line)
        {
            while (true)
            {
                lock (this.processing)
                {
                    if(!this.isBullying(line))
                    {
                        break;
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
            return base.Call(line);
        }
        /// <summary>
        /// Bullies the specified line.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="suffix">The suffix.</param>
        public void Bully(string prefix, string suffix)
        {
            while (true)
            {
                lock (this.processing)
                {
                    if (!this.processing.Any(tp=>tp.StartsWith(prefix)&&tp.EndsWith(suffix))&&!
                        isBullying(prefix+suffix))
                    {
                        this.bullying.Add(new[] { prefix, suffix });
                        return;
                    }
                }

                System.Threading.Thread.Sleep(100);
            }
        }
        /// <summary>
        /// Finishes the bullying.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="suffix">The suffix.</param>
        public void FinishBullying(string prefix, string suffix)
        {
            lock (this.processing)
            {
                this.bullying.Remove(this.bullying.First(tb => tb[0] == prefix && tb[1] == suffix));
            }
        }
        List<string[]> bullying = new List<string[]>();
        bool isBullying(string line)
        {
            return this.bullying.Any(tb => line.StartsWith(tb[0])&&line.EndsWith(tb[1]));
        }
    }
}
