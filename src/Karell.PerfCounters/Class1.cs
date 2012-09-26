using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Karell.PerfCounters
{
    public class PerformanceCounterManager
    {
        private readonly CounterCreationDataCollection _collection =
            new CounterCreationDataCollection();
        private readonly Dictionary<string, CounterCreationData> _items =
            new Dictionary<string, CounterCreationData>();
        private readonly string _categoryName;
        private readonly string _categoryHelp;

        public PerformanceCounterManager(string categoryName, string categoryHelp)
        {
            _categoryName = categoryName;
            _categoryHelp = categoryHelp;
        }

        public void RegisterCounter(string counterName, string helpString, PerformanceCounterType performanceCounterType)
        {
            CounterCreationData counter =
                new CounterCreationData
                    {
                        CounterName = counterName,
                        CounterHelp = helpString,
                        CounterType = performanceCounterType
                    };

            _collection.Add(counter);
            _items.Add(counterName, counter);
        }

        public void DeregisterCounter(string counterName)
        {
            _collection.Remove(_items[counterName]);
            _items.Remove(counterName);
        }

        public IList<string> CounterNames
        {
            get { return _items.Keys.ToList(); }
        }

        public bool RegisterCounters()
        {
            if (!PerformanceCounterCategory.Exists(_categoryName))
            {
                PerformanceCounterCategory.Create(
                    _categoryName, _categoryHelp,
                    PerformanceCounterCategoryType.SingleInstance, _collection);
                return true;
            }
            return false;
        }

        public void DeregisterCounters()
        {
            PerformanceCounterCategory.Delete(_categoryName);
        }

        public void UpdateCounters()
        {
            DeregisterCounters();
            RegisterCounters();
        }

        public PerformanceCounter GetCounter(string counterName)
        {
            if (_items.ContainsKey(counterName))
                return new PerformanceCounter(_categoryName, counterName, false);
            throw new ArgumentException("Counter name does not exist");
        }
    }
}
