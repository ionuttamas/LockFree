using System.Collections;
using System.Collections.Generic;
using LockFree.Common;

namespace LockFree.Core.List
{
    public class List<T>:IEnumerable<T>
    {
        private readonly T _sentinel;
        private readonly Node<T> _head;

        public List()
        {
            _head = new Node<T>();
            _sentinel = default(T);
        }

        public List(T item)
        {
            _head = new Node<T>(item);
            _sentinel = item;
        }

        public void Add(T item)
        {
            Node<T> node = new Node<T>(item);
            NodeReference<T> newReference;

            do
            {
                newReference = new NodeReference<T>
                {
                    Next = node
                };

                node.Reference = _head.Reference;
                node.Reference.Value = item;
            }
            while (!Atomic.CAS(ref _head.Reference, node.Reference, newReference));
        }

        public void Remove(T item)
        {
            Node<T> node = _head;

            while (node != null)
            {
                if (node.Reference == null)
                    return;

                if (node.Reference.Value.Equals(item))
                {
                    Remove(node);
                    break;
                }
                 
                node = node.Reference.Next;
            }
        }

        private void Remove(Node<T> item)
        {
            Node<T> next;
            NodeReference<T> oldReference;
            NodeReference<T> newReference;

            do
            {
                oldReference = item.Reference;
                next = item.Reference.Next;

                if (next==null || next.Reference == null)
                {
                    newReference = null;
                }
                else
                {
                    newReference = new NodeReference<T>
                    {
                        Next = next.Reference.Next,
                        Value = next.Reference.Value
                    };
                }
                
            } while (!Atomic.CAS(ref item.Reference, oldReference, newReference));
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node<T> node = _head.Reference.Next;

            while (node != null)
            {
                if (node.Reference == null)
                    yield break;

                yield return node.Reference.Value;

                node = node.Reference.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}