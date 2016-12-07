using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{

    class Edge
    {
        private int origin;
        private int destination;
        private double cost;

        public Edge(int origin, int destination, double cost)
        {
            this.origin = origin;
            this.destination = destination;
            this.cost = cost;
        }

        public int getOrigin()
        {
            return origin;
        }
        
        public int getDestination()
        {
            return destination;
        }
        
        public double getCost()
        {
            return cost;
        }
    }
}
