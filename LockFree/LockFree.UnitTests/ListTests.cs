using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace LockFree.UnitTests
{
    [TestFixture]
    public class ListTests
    {
        [Test]
        public void List_WithAdditions_And_Removals_WorksCorrectly()
        {
            int initialValue = -1;
            Core.List.List<int> list = new Core.List.List<int>(initialValue);

            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Remove(3);
            list.Remove(2);
            list.Remove(1);
            list.Remove(2);
            list.Add(4);
            list.Add(6);
            list.Add(8);

            foreach (var item in list)
            {
                Console.WriteLine(item);
            }

            Assert.AreEqual(3, list.ToList().Count);
        }

        [TestCase(2, 100, 50)]
        [TestCase(1, 100, 80)]
        [TestCase(1, 1000, 700)]
        [TestCase(2, 10000, 6000)]
        [TestCase(2, 100000, 25000)]
        [TestCase(4, 150000, 85000)]
        public void List_WithLowThreadCount_WorksCorrectly(int threads, int adds, int removes)
        {
            //The elements we insert are unique and sentinel value is -1
            int initialValue = -1;
            int currentValue = initialValue;
            ConcurrentDictionary<int, int> table = new ConcurrentDictionary<int, int>();
            Core.List.List<int> list = new Core.List.List<int>(initialValue);
            Random random = new Random();
             
            ThreadBuilder
                .Empty()
                .AddThreads(() =>
                {
                    for (int i = 0; i < adds; i++)
                    {
                        int value = Interlocked.Increment(ref currentValue);
                        list.Add(value);
                    }
                }, threads)
                .AddThreads(() =>
                {
                    for (int i = 0; i < removes; i++)
                    {
                        int element = random.Next(currentValue);

                        if (table.ContainsKey(element))
                        {
                            list.Remove(element); 
                            table.AddOrUpdate(element, x => 1, (k, v) => v + 1);
                        }
                    }
                }, threads)
                .Start();

            //The sentinel value can be returned more than once if queue is empty at the time of a dequeue
            int expectedAdds = table.Keys.Count + (table.ContainsKey(initialValue) ? -1 : 0);
            int actualRemoves = table.Count(x => x.Key != initialValue && x.Value == 1);

            Assert.AreEqual(expectedAdds, actualRemoves);
        }

        [TestCase(10, 100000, 50000)]
        [TestCase(20, 100000, 50000)]
        [TestCase(20, 1000000, 250000)]
        [TestCase(40, 1500000, 850000)]
        public void List_WithHighThreadCount_WorksCorrectly(int threads, int adds, int removes)
        {
            //The elements we insert are unique and sentinel value is -1
            int initialValue = -1;
            int currentValue = initialValue;
            ConcurrentDictionary<int, int> table = new ConcurrentDictionary<int, int>();
            Core.List.List<int> list = new Core.List.List<int>(initialValue);
            Random random = new Random();

            ThreadBuilder
                .Empty()
                .AddThreads(() =>
                {
                    for (int i = 0; i < adds; i++)
                    {
                        int value = Interlocked.Increment(ref currentValue);
                        list.Add(value);
                    }
                }, threads)
                .AddThreads(() =>
                {
                    for (int i = 0; i < removes; i++)
                    {
                        int element = random.Next(currentValue);

                        if (table.ContainsKey(element))
                        {
                            list.Remove(element);
                            table.AddOrUpdate(element, x => 1, (k, v) => v + 1);
                        }
                    }
                }, threads)
                .Start();

            //The sentinel value can be returned more than once if queue is empty at the time of a dequeue
            int expectedAdds = table.Keys.Count + (table.ContainsKey(initialValue) ? -1 : 0);
            int actualRemoves = table.Count(x => x.Key != initialValue && x.Value == 1);

            Assert.AreEqual(expectedAdds, actualRemoves);
        }
    }
}