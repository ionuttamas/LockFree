namespace LockFree.Core.List
{
    public class Node<T>
    {
        public volatile NodeReference<T> Reference;
        
        public Node()
        {
            Reference = new NodeReference<T>();
        }

        public Node(T item) 
        {
            Reference = new NodeReference<T>(item);
        }
    }
}
