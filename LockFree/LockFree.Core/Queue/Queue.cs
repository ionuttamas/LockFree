using LockFree.Common;

namespace LockFree.Core.Queue
{
    public class Queue<T>
    {
        //Insert at tail and remove from head
        private Node<T> _head;
        private Node<T> _tail;

        public Queue()
        {
            _head = new Node<T>();
            _tail = _head;
        }

        public Queue(T item)
        {
            _head = new Node<T>(item);
            _tail = _head;
        }

        public void Enqueue(T item)
        {
            Node<T> node = new Node<T>(item);
            Node<T> oldTail;
            Node<T> oldNext;

            while (true)
            {
                oldTail = _tail;
                oldNext = oldTail.Next;

                if (_tail== oldTail)
                {
                    if (oldNext == null)
                    {
                        if (Atomic.CAS(ref _tail.Next, null, node))
                            break;
                    }
                    else
                    {
                        Atomic.CAS(ref _tail, oldTail, oldNext);
                    }
                }
            }
             
            Atomic.CAS(ref _tail, oldTail, node);
        }

        public T Dequeue()
        {
            Node<T> next;
            Node<T> oldHead;  

            do
            {
                oldHead = _head; 
                next = _head.Next; 

                if (next == null)
                    return _head.Value;

            } while (!Atomic.CAS(ref _head, oldHead, next));

            return oldHead.Value;
        }
    }
}