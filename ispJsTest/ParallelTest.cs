using System;
using System.Collections.Generic;
using System.Threading;
namespace JSoonLibTesting
{
	public interface ParallelTest
	{
		void threadStart(int threadId,ParallelTester tester);
	}
}

