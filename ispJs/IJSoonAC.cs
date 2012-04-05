using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ispJs
{
    /// <summary>
    /// 
    /// </summary>
    public interface IISPAC
    {
        /// <summary>
        /// Page_s the read.
        /// </summary>
        /// <param name="subPage">The sub page.</param>
        void Page_Read(string subPage);
    }
    /// <summary>
    /// 
    /// </summary>
    public class AccessDeniedException : System.Exception
    {
        internal string Redirect;
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDeniedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public AccessDeniedException(string message)
            : base(message)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDeniedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="redirect">The redirect.</param>
        public AccessDeniedException(string message, string redirect)
            :this(message)
        {
            this.Redirect = redirect;
        }
    }
}
