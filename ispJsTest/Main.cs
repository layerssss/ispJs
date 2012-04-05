using System;
namespace JSoonLibTesting
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            
            var t = new FerryTest();
            var tester = new ParallelTester(t, 20);
            
            
		}
		
	}
}
