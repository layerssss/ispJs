using System;
using ispJs;
namespace JSoonLibTesting
{
	public class FerryTest:ParallelTest
	{
		public FerryTest ()
		{

			this.ferry = new Ferry ();
			this.r = new Random ();
		}

		Random r;
		Ferry ferry;

		#region ParallelTest implementation
		public void threadStart (int threadId, ParallelTester tester)
		{
			while (true) {
				tester.Status = "Taking the ferry!";
				if (this.ferry.Call ("haha")) {
					tester.Status = "Driving the ferry!";
					tester.Sleep (r.Next (5000));
					this.ferry.Finish ("haha");
				}
				tester.Status = "";
				tester.Sleep (r.Next (5000));
			}
		}
		#endregion
	}
}

