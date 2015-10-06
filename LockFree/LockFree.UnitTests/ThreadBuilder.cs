using System;
using System.Collections.Generic;
using System.Threading;

namespace LockFree.UnitTests
{
    public class ThreadBuilder
    {
        private static readonly ThreadBuilder Instance = new ThreadBuilder();
        private static readonly List<Thread> Threads = new List<Thread>(); 

        public static ThreadBuilder Empty()
        {
            return Instance;
        }

        public ThreadBuilder AddThreads(Action action, int count = 1)
        {
            if (count <= 0)
                return this;

            for (int i = 0; i < count; i++)
            {
                Threads.Add(new Thread(new ThreadStart(action)));
            }     

            return this;
        }

        public void Start()
        {
            foreach (Thread thread in Threads)
            {
                thread.Start();
            }

            foreach (Thread thread in Threads)
            {
                thread.Join();
            }

            Threads.Clear();
        }
    }
}