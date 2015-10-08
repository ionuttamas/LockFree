using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LockFree.Core.Stack;
using NUnit.Framework;

namespace LockFree.UnitTests
{
    [TestFixture]
    public class QueueTests
    {
        [TestCase(2, 100, 50)]
        [TestCase(1, 100, 60)]
        [TestCase(1, 1000, 500)]
        [TestCase(2, 10000, 5000)]
        [TestCase(2, 100000, 25000)]
        [TestCase(4, 150000, 85000)]
        public void Queue_WithLowThreadCount_WorksCorrectly(int threads, int enqueues, int dequeues)
        {
            //The elements we insert are unique and sentinel value is -1
            int initialValue = -1;
            int currentValue = initialValue;
            ConcurrentDictionary<int, int> table = new ConcurrentDictionary<int, int>();
            Core.Queue.Queue<int> queue = new Core.Queue.Queue<int>(initialValue);
              
            ThreadBuilder
                .Empty()
                .AddThreads(() =>
                {
                    for (int i = 0; i < enqueues; i++)
                    {
                        int value = Interlocked.Increment(ref currentValue);
                        queue.Enqueue(value); 
                    }
                }, threads)
                .AddThreads(() =>
                {
                    for (int i = 0; i < dequeues; i++)
                    {
                        int value = queue.Dequeue();
                        table.AddOrUpdate(value, x => 1, (k, v) => v + 1);
                    }
                }, threads)
                .Start();

            //The sentinel value can be returned more than once if queue is empty at the time of a dequeue
            int expectedDequeues = table.Keys.Count + (table.ContainsKey(initialValue) ? -1 : 0);
            int actualDequeues = table.Count(x => x.Key != initialValue && x.Value == 1);
          
            Assert.AreEqual(expectedDequeues, actualDequeues);
        }

        [TestCase(10, 100000, 50000)]
        [TestCase(20, 100000, 50000)]
        [TestCase(20, 1000000, 250000)]
        [TestCase(40, 1500000, 850000)]
        public void Queue_WithHighThreadCount_WorksCorrectly(int threads, int enqueues, int dequeues)
        {
            //The elements we insert are unique and sentinel value is -1
            int initialValue = -1;
            int currentValue = initialValue;
            ConcurrentDictionary<int, int> table = new ConcurrentDictionary<int, int>();
            Core.Queue.Queue<int> queue = new Core.Queue.Queue<int>(initialValue);

            ThreadBuilder
                .Empty()
                .AddThreads(() =>
                {
                    for (int i = 0; i < enqueues; i++)
                    {
                        int value = Interlocked.Increment(ref currentValue);
                        queue.Enqueue(value);
                    }
                }, threads)
                .AddThreads(() =>
                {
                    for (int i = 0; i < dequeues; i++)
                    {
                        int value = queue.Dequeue();
                        table.AddOrUpdate(value, x => 1, (k, v) => v + 1);
                    }
                }, threads)
                .Start();

            //The sentinel value can be returned more than once if queue is empty at the time of a pop
            int expectedPops = table.Keys.Count + (table.ContainsKey(initialValue) ? -1 : 0);
            int actualPops = table.Count(x => x.Key != initialValue && x.Value == 1);

            Assert.AreEqual(expectedPops, actualPops);
        }
    }
}
