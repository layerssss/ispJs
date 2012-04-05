using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ispJs;
namespace JSoonLibTesting
{
    class WebFerryTest:ParallelTest
    {
        Random r = new Random();
        #region ParallelTest 成员

        public void threadStart(int threadId, ParallelTester tester)
        {
            while (true)
            {
                var wc = new System.Net.WebClient();
                tester.Status = "Opening...";
                try
                {
                    tester.Status = wc.DownloadString("http://localhost:1512/");
                }
                catch (System.Net.WebException ex)
                {
                    var sr = new System.IO.StreamReader(ex.Response.GetResponseStream());
                    tester.Status = sr.ReadToEnd();
                    sr.Close();
                    System.IO.File.WriteAllText("error.txt", tester.Status);
                    tester.Sleep(5000);

                }
                tester.Status = "Resting...";
                tester.Sleep(r.Next(5000));
            }
        }

        #endregion
    }
}
