using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PWH.Utils
{
    public class PriorityQueue<T> where T : System.IComparable<T>
    {
        List<T> data;
        public int Count { get { return data.Count; } }

        public PriorityQueue()
        {
            this.data = new List<T>();
        }

        public void Enqueue(T item)
        {
            data.Add(item);

            int childIndex = data.Count - 1;

            while (childIndex > 0)
            {
                int parentindex = (childIndex - 1) / 2;

                if (data[childIndex].CompareTo(data[parentindex]) >= 0)
                {
                    break;
                }

                T tmp = data[childIndex];
                data[childIndex] = data[parentindex];
                data[parentindex] = tmp;
                childIndex = parentindex;
            }
        }

        public T Dequeue()
        {
            int lastindex = data.Count - 1;

            T frontItem = data[0];

            data[0] = data[lastindex];
            data.RemoveAt(lastindex);

            lastindex--;

            int parentindex = 0;

            while (true)
            {
                int childindex = parentindex * 2 + 1;

                if (childindex > lastindex)
                {
                    break;
                }

                int rightchild = childindex + 1;
                if (rightchild <= lastindex && data[rightchild].CompareTo(data[childindex]) < 0)
                {
                    childindex = rightchild;
                }

                if (data[parentindex].CompareTo(data[childindex]) <= 0)
                {
                    break;
                }

                T tmp = data[parentindex];
                data[parentindex] = data[childindex];
                data[childindex] = tmp;

                parentindex = childindex;
            }


            return frontItem;
        }

        public T Peek()
        {
            T frontItem = data[0];

            return frontItem;
        }

        public bool Contains(T item)
        {
            return data.Contains(item);
        }

        public List<T> ToList()
        {
            return data;
        }
    }
}