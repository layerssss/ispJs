using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ispJs;
namespace ispWebTest
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            WebApplication.ActionCommentsXmlPath = Server.MapPath("/Bin/ispWebTest.XML");
            WebApplication.RegisterRenderer("guestbook/guestbook.isp.js", new guestbook.guestbook());
            WebApplication.RegisterActions("ispWebTest", typeof(guestbook.guestbook));
            WebApplication.RegisterSubPage("x2.isp.js");
            WebApplication.HandleStart(Server);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            var path = Request.Path;
            var exts = new[] { "js", "css", "jpeg", "jpg", "gif", "png", "swf", "pdf", "mp3", "mp4" };
            var ext = "";
            try
            {
                ext = path.Substring(path.LastIndexOf('.') + 1);
            }
            catch { }
            if (!(path.Contains('.') && exts.Contains(ext)))
            {
                Response.ContentType = "text/html";
            }
        }
        
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            WebApplication.HandleBeginRequest();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            WebApplication.HandleEnd();
        }
    }
}