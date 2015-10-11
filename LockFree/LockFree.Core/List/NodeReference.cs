namespace LockFree.Core.List
{
    public class NodeReference<T>
    {
        public T Value { get; set; }
        public Node<T> Next;

        public NodeReference()
        {
        }

        public NodeReference(T item)
        {
            Value = item;
        }
    }
}