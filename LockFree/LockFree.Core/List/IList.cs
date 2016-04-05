namespace LockFree.Core.List {
    public interface IList<in T> {
        void Add(T item);
        void Remove(T item);
    }
}