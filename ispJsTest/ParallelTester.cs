using System;
using System.Threading;
using System.Collections.Generic;
namespace JSoonLibTesting
{
	public class ParallelTester
	{
		public ParallelTester (ParallelTest test,int num)
		{
			this.test=test;
			this.threadStatus=new Dictionary<Thread, string>();
			while(num!=0){
				var t=new Thread(this.threadStart);
				lock(this.threadStatus){
				this.threadStatus.Add(t,"Started");
				}
				t.Start();
				num--;
			}
			while(true){
				Console.Clear();
				lock(this.threadStatus){
				foreach(var t in this.threadStatus.Keys){
					Console.WriteLine("Thread{0} {1} {2}",
					                  t.ManagedThreadId,
					                  t.ThreadState,
					                  this.threadStatus[t]
					                  );
					}
				}
				Thread.Sleep(150);
			}
		}
		ParallelTest test;
		public void threadStart(){
			test.threadStart(Thread.CurrentThread.ManagedThreadId,this);
		}
		public void Sleep(int ms){
			Thread.Sleep(ms);
		}
		Dictionary<Thread,string> threadStatus;
		public string Status{
			set{
				lock(this.threadStatus){
					this.threadStatus[Thread.CurrentThread]=value;
				}
			}
			get{
				lock(this.threadStatus){
					return this.threadStatus[Thread.CurrentThread];
				}
			}
		}
	}
}

