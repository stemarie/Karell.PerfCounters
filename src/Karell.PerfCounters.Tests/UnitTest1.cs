using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Karell.PerfCounters.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private const string CategoryName = "Test Multi Counter Category";
        private const string CategoryHelp = "Test Category help";
        private const string CounterName = "Test Counter";
        private const string CounterHelp = "Test Counter Help String";

        [TestMethod]
        public void TestMethod1()
        {
            var pm = new PerformanceCounterManager(CategoryName, CategoryHelp);
            pm.RegisterCounter(CounterName, CounterHelp, PerformanceCounterType.NumberOfItems64);
            pm.RegisterCounters();
            Do2(CounterName, pm);
            pm.DeregisterCounters();
        }

        private static void Do2(string counterName, PerformanceCounterManager manager)
        {
            using (var c = manager.GetCounter(counterName))
            {
                Parallel.For(1, 10, a => DoYourThing(c));

                c.Close();
            }
        }

        private static void DoYourThing(PerformanceCounter c)
        {
            c.Increment();
            Thread.Sleep(5);
            c.Decrement();
        }
    }
}
