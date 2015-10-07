using LockFree.Common;

namespace LockFree.Core.Stack
{
    public class Stack<T>
    {
        private readonly Node<T> _top;

        public Stack()
        {
            _top = new Node<T>();
        }

        public Stack(T item)
        {
            _top = new Node<T>(item);
        }

        public void Push(T item)
        {
            Node<T> node = new Node<T>(item);
              
            do
            {
                node.Next = _top.Next;
            } while (!Atomic.CAS(ref _top.Next, node.Next, node));
        }

        public T Pop()
        {
            Node<T> next; 

            do
            {
                next = _top.Next; 

                if (next == null)
                    return _top.Value;

            } while (!Atomic.CAS(ref _top.Next, next, next.Next));

            return next.Value;
        }
    }
}