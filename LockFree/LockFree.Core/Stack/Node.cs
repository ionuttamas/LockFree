namespace LockFree.Core.Stack
{
    public class Node<T>  
    {
        public T Value { get; set; }
        public Node<T> Next;

        public Node()
        {
        }

        public Node(T item)
        {
            Value = item;
        }
    }
}
