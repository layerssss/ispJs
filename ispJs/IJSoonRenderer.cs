using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ispJs
{
    /// <summary>
    /// Defining the pre-initialization of a ISP page.
    /// </summary>
    public interface IISPRenderer : IDisposable
    {
        /// <summary>
        /// Page_s the load.
        /// </summary>
        /// <param name="locals">The locals.</param>
        void Page_Load(Dictionary<string, object> locals);
    }
}
