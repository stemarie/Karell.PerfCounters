﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Karell.PerfCounters.Properties;

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
            if (_items.ContainsKey(counterName))
                throw new ArgumentException(Resources.CounterNameAlreadyExists);

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

        public void RegisterCounterIfNotExist(string counterName, string helpString,
                                              PerformanceCounterType performanceCounter)
        {
            if (!_items.ContainsKey(counterName))
                RegisterCounter(counterName, helpString, performanceCounter);
        }

        public void DeregisterCounter(string counterName)
        {
            if (_items.ContainsKey(counterName))
            {
                _collection.Remove(_items[counterName]);
                _items.Remove(counterName);
            }
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

        public bool DeregisterCounters()
        {
            try
            {
                PerformanceCounterCategory.Delete(_categoryName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
            throw new ArgumentException(Resources.CounterNameDoesNotExist);
        }
    }
}
