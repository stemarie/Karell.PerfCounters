using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Karell.PerfCounters.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var pm = new PerformanceCounterManager("Multi Counter Category", "Category help");
            pm.RegisterCounter("Counter1", "help string1", PerformanceCounterType.NumberOfItems64);
            pm.RegisterCounters();
            Do2("Counter1", pm);
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
