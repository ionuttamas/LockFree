namespace LockFree.Core.List {
    public class FullBlockingList<T> {
        private readonly BlockingNode _head;
        private readonly object _locker;

        public FullBlockingList() {
            _head = new BlockingNode(default(T));
            _locker = new object();
        }

        public FullBlockingList(T item) {
            _head = new BlockingNode(item);
        }

        public void Add(T item) {
            BlockingNode node = new BlockingNode(item);

            lock (_locker) {
                node.Next = _head.Next;
                _head.Next = node;
            } 
        }

        public void Remove(T item) {
            BlockingNode node = _head;

            lock (_locker) {
                while (node != null) {
                    if (node.Next == null)
                        return;

                    if (node.Next.Value.Equals(item)) {
                        RemoveNext(node);
                        break;
                    }

                    node = node.Next;
                }
            } 
        }
        
        private void RemoveNext(BlockingNode node) {
            node.Next = node.Next.Next;    
        }

        private class BlockingNode {
            public T Value { get; private set; }
            public BlockingNode Next { get; set; }

            public BlockingNode(T value) {
                Value = value;
            }
        }
    }
}