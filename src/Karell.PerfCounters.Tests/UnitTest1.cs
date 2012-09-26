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
        private const string CounterName1 = "Test Counter 1";
        private const string CounterHelp1 = "Test Counter 1 Help String";
        private const string CounterName2 = "Test Counter 2";
        private const string CounterHelp2 = "Test Counter 2 Help String";

        [TestMethod]
        public void TestMethod1()
        {
            var pm = new PerformanceCounterManager(CategoryName, CategoryHelp);
            pm.RegisterCounter(CounterName1, CounterHelp1, PerformanceCounterType.NumberOfItems64);
            pm.RegisterCounter(CounterName2, CounterHelp2, PerformanceCounterType.NumberOfItems64);
            pm.RegisterCounters();
            DoFull(CounterName1, pm);
            DoFull(CounterName2, pm);
            pm.DeregisterCounters();
        }

        private static void DoFull(string counterName, PerformanceCounterManager manager)
        {
            using (var c = manager.GetCounter(counterName))
            {
                Parallel.For(1, 10, a => DoItem(c));

                c.Close();
            }
        }

        private static void DoItem(PerformanceCounter c)
        {
            c.Increment();
            Thread.Sleep(5);
            c.Decrement();
        }
    }
}
