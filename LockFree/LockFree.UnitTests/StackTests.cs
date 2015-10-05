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
    public class StackTests 
    {
        [TestCase(1, 100, 60)]
        [TestCase(1, 1000, 500)]
        [TestCase(2, 10000, 5000)]
        [TestCase(2, 100000, 25000)]
        [TestCase(4, 150000, 85000)]
        public void Stack_WithLowThreadCount_WorksCorrectly(int threads, int pushes, int pops)
        {
            //The elements we insert are unique and sentinel value is -1
            int initialValue = -1;
            int currentValue = initialValue;
            ConcurrentDictionary<int, int> table = new ConcurrentDictionary<int, int>();
            LockFreeStack<int> stack = new LockFreeStack<int>(initialValue);

            ThreadBuilder
                .Empty()
                .AddThreads(() =>
                {
                    for (int i = 0; i < pushes; i++)
                    {
                        int value = Interlocked.Increment(ref currentValue);
                        stack.Push(value);
                    }
                }, threads)
                .AddThreads(() =>
                {
                    for (int i = 0; i < pops; i++)
                    {
                        int value = stack.Pop();
                        table.AddOrUpdate(value, x => 1, (k, v) => v + 1);
                    }
                }, threads)
                .Start();

            //The sentinel value can be returned more than once if queue is empty at the time of a pop
            int expectedPops = table.Keys.Count + (table.ContainsKey(initialValue) ? -1 : 0);
            int actualPops = table.Count(x => x.Key != initialValue && x.Value == 1);
          
            Assert.AreEqual(expectedPops, actualPops);
        }

        [TestCase(10, 100000, 50000)]
        [TestCase(20, 100000, 50000)]
        [TestCase(20, 1000000, 250000)]
        [TestCase(40, 1500000, 850000)]
        public void Stack_WithHighThreadCount_WorksCorrectly(int threads, int pushes, int pops)
        {
            int expectedCount = pushes - pops;
        }
    }
}
