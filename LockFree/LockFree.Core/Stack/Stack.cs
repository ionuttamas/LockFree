using LockFree.Common;

namespace LockFree.Core.Stack
{
    public class Stack<T>
    {
        private readonly Node<T> _head;

        public Stack()
        {
            _head = new Node<T>();
        }

        public Stack(T item)
        {
            _head = new Node<T>(item);
        }

        public void Push(T item)
        {
            Node<T> node = new Node<T>(item);
              
            do
            {
                node.Next = _head.Next;
            } while (!Atomic.CAS(ref _head.Next, node.Next, node));
        }

        public T Pop()
        {
            Node<T> next; 

            do
            {
                next = _head.Next; 

                if (next == null)
                    return _head.Value;

            } while (!Atomic.CAS(ref _head.Next, next, next.Next));

            return next.Value;
        }
    }
}