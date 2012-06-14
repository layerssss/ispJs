using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ispWebTest.guestbook
{
    public class guestbook:ispJs.IISPRenderer
    {
        static List<string> notes;
        public guestbook()
        {
            notes = new List<string>();
            notes.Add("Hello!");
            notes.Add("How are you!");
        }
        /// <summary>
        /// 进行一次“留言操作
        /// </summary>
        /// <param name="note">留言的内容</param>
        [ispJs.Action]
        public void Add(string note)
        {
            notes.Add(note);
            ispJs.WebApplication.SafeDelete("guestbook/guestbook");
        }

        public void Page_Load(Dictionary<string, object> locals)
        {
            locals["notes"] = notes.ToArray();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}