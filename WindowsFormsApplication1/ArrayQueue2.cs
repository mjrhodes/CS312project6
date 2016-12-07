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

        public ArrayQueue2(int numCities)
        {
            currentSize = 0;
            q = new Edge[numCities^numCities];
            for (int i = 0; i < q.Length; i++)
                q[i] = null;
        }

        ~ArrayQueue2() { }

        public void insert(Edge edge)
        {
            q[currentSize] = edge;
            currentSize++;
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
                while(index < q.Length-1 && q[index] != null)
                {
                    q[index] = q[index + 1];
                    index++;
                }
                currentSize--;
                q[currentSize] = null;
            }
            return result;
        }

        public int getSize()
        {
            return currentSize;
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
