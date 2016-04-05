package List;

public class FullBlockingList<T> implements IList<T> {
    private final Object _locker;
    private final BlockingNode _head;

    public FullBlockingList() {
        _locker = new Object();
        _head = new BlockingNode(null);
    }

    @Override
    public void Add(T item) {
        BlockingNode node = new BlockingNode(item);

        synchronized (_locker) {
            node.setNext(_head.getNext());
            _head.setNext(node);
        }
    }

    @Override
    public void Remove(T item) {
        BlockingNode node = _head;

        synchronized (_locker) {
            while (node != null) {
                if (node.getNext() == null)
                    return;

                if (node.getNext().getValue().equals(item)) {
                    RemoveNext(node);
                    break;
                }

                node = node.getNext();
            }
        }
    }

    private void RemoveNext(BlockingNode node) {
        node.setNext(node.getNext().getNext());
    }

    private class BlockingNode {
        private final T _value;
        private BlockingNode _next;

        private BlockingNode(T value) {
            _value = value;
        }

        public T getValue() {
            return _value;
        }

        public BlockingNode getNext() {
            return _next;
        }

        public void setNext(BlockingNode _next) {
            this._next = _next;
        }
    }
}
