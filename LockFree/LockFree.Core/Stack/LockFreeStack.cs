using LockFree.Common;

namespace LockFree.Core.Stack
{
    public class LockFreeStack<T>
    {
        private readonly Node<T> _head;

        public LockFreeStack()
        {
            _head = new Node<T>();
        }

        public LockFreeStack(T item)
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