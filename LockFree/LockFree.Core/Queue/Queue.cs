using LockFree.Common;

namespace LockFree.Core.Queue
{
    public class Queue<T>
    {
        //Insert at tail and remove from head
        private Node<T> _head;
        private Node<T> _tail;
        private readonly T _sentinel;
        
        public Queue()
        {
            _head = null;
            _tail = _head;
        }

        public Queue(T sentinel) 
        {
            _head = new Node<T>(sentinel);
            _tail = _head; 
            _sentinel = sentinel;
        }

        public void Enqueue(T item)
        {
            Node<T> node = new Node<T>(item);
            Node<T> oldTail;
            Node<T> next;

            while (true)
            {
                oldTail = _tail;

                if (_tail == null && Atomic.CAS(ref _tail, null, node))
                    break;
                
                next = oldTail.Next;

                if (_tail == oldTail)
                {
                    if (next == null)
                    {
                        if (Atomic.CAS(ref _tail.Next, null, node))
                            break;
                    }
                    else
                    {
                        Atomic.CAS(ref _tail, oldTail, next);
                    }
                }
            }

            Atomic.CAS(ref _tail, oldTail, node);
            Atomic.CAS(ref _head.Next, null, node);
        }

        public T Dequeue()
        {
            Node<T> head;
            Node<T> oldHead;
            Node<T> oldTail;
            Node<T> next;

            while (true)
            {
                oldTail = _tail;
                head = _head.Next;

                if (head == null)
                    return _sentinel;

                oldHead = head;
                next = oldHead.Next;

                if (head == oldHead)
                {
                    if (oldHead == oldTail)
                    {
                        Atomic.CAS(ref _tail, oldTail, next);

                        if (next == null && Atomic.CAS(ref _head.Next, oldHead, null))
                            return _sentinel; 
                    }
                    else
                    {
                        if (Atomic.CAS(ref _head.Next, oldHead, next))
                            return oldHead.Value;
                    }
                }
            }
        }
    }
}