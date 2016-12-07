using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class ArrayQueue2
    {
        private Edge[] q;
        private int currentSize;
        private int free;
        private int max;

        public ArrayQueue2(int numCities)
        {
            currentSize = 0;
            free = 0;
            max = numCities * numCities;
            q = new Edge[max];
            for (int i = 0; i < q.Length; i++)
                q[i] = null;
        }

        ~ArrayQueue2() { }

        public void insert(Edge edge)
        {
            if (free > q.Length - 1)
            {
                throw new InsufficientMemoryException("Exceed Priority Queue memory!");
            }
            q[free] = edge;
            currentSize++;
            free++;
            if (free == max) cleanUp();
        }

        public Edge deletemin()
        {
            double min = -1;
            int index = -1;
            Edge result = null;
            for (int i = 0; i < q.Length; i++)
            {
                if (q[i] == null) continue;
                if (q[i].getCost() < min || min == -1)
                {
                    min = q[i].getCost();
                    index = i;
                    result = q[index];
                }
            }
            if(index != -1)
            {
                currentSize--;
                q[index] = null;
            }
            return result;
        }

        public int getSize()
        {
            return currentSize;
        }

        public void cleanUp()
        {
            int index = 0;
            free = 0;
            while (index < q.Length - 1)
            {
                while (q[index] == null && index < q.Length-1) index++;
                if (free == index)
                {
                    free++;
                    index++;
                }
                else
                {
                    q[free] = q[index];
                    q[index] = null;
                    free++;
                }
            }
            free = currentSize;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("queue: (size {0})\n", currentSize);
            int printMax = Math.Min(q.Length, 10);
            for(int i = 0; i < printMax; i++)
            {
                if(q[i] != null)
                {
                    sb.AppendFormat("[{0}]: {1}\n", i, q[i].getCost());
                }
                else
                {
                    sb.AppendFormat("[{0}]: null\n", i);
                }
            }
            sb.Append("\n");
            return sb.ToString();
        }
    }
}
