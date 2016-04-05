package List;

import java.util.concurrent.atomic.AtomicStampedReference;

public class LockFreeList<T> implements IList<T> {
    private final StampedNode _head;
    private static final int NO_STAMP = 0;
    private static final int DELETED_STAMP = 0;

    public LockFreeList() {
        _head = new StampedNode(null);
    }

    @Override
    public void Add(T item) {
        StampedNode node = new StampedNode(item);

        do
        {
            _head.setNext();

            newReference = new NodeReference<T>
            {
                Next = node
            };

            node.Reference = _head.Reference;
            node.Reference.Value = item;
        }
        while (!Atomic.CAS(ref _head.Reference, node.Reference, newReference));
    }

    @Override
    public void Remove(T item) {

    }

    private class StampedNode {
        private final T _value;
        private AtomicStampedReference<StampedNode> _next;

        private StampedNode (T value) {
            _value = value;
        }

        public T getValue() {
            return _value;
        }

        public AtomicStampedReference<StampedNode> getNextReference() {
            return _next;
        }

        public StampedNode getNext() {
            return _next.getReference();
        }

        public boolean setNext(StampedNode newNext) {
            return _next.compareAndSet(_next.getReference(), newNext, NO_STAMP, NO_STAMP);
        }
    }
}
