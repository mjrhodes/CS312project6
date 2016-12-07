using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class ArrayQueue
    {
        private Node[] q;
        private int currentSize;

        public ArrayQueue(int numCities)
        {
            currentSize = 0;
            q = new Node[1000000];
            for (int i = 0; i < q.Length; i++)
                q[i] = null;
        }

        ~ArrayQueue() { }

        public void insert(Node node)
        {
            q[currentSize] = node;
            currentSize++;
        }

        public Node deletemin()
        {
            double min = -1;
            int index = -1;
            Node result = null;
            for (int i = 0; i < q.Length; i++)
            {
                if (q[i] == null) continue;
                if (q[i].getPValue() < min || min == -1)
                {
                    min = q[i].getPValue();
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
                    sb.AppendFormat("[{0}]: {1}\n", i, q[i].getPValue());
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
