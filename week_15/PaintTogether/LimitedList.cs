using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintTogether
{
    internal class LimitedList<T>
    {
        private LinkedList<T> list;
        private int maxCount;

        public LimitedList(int maxCount)
        {
            this.maxCount = maxCount;
            list = new LinkedList<T>();
        }

        public void Add(T element)
        {
            if (list.Count == maxCount)
            {
                list.RemoveFirst();
            }

            list.AddLast(element);
        }

        public T[] ToArray()
        {
            return list.ToArray();
        }
    }
}
