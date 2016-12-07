using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class Node : IComparable
    {
        private double pValue;
        private double[,] matrix;
        private ArrayList route;
        private double b;
        private int lastCity;

        public Node(int numCities)
        {
            matrix = null;
            route = new ArrayList();
            b = 0;
            lastCity = -1;
        }

        public double getPValue()
        { 
            return pValue;
        }

        public void setPValue(double newValue)
        {
            pValue = newValue;
        }

        public double[,] getMatrix()
        {
            return matrix;
        }

        public void setMatrix(double[,] newMatrix)
        {
            matrix = newMatrix;
        }

        public ArrayList getRoute()
        {
            return route;
        }

        public void setRoute(ArrayList newRoute)
        {
            route = newRoute;
        }

        public void addToRoute(int city)
        {
            route.Add(city);
            lastCity = city;
        }

        public double getB()
        {
            return b;
        }

        public void setB(double newB)
        {
            b = newB;
        }

        public int getLastCity()
        {
            return lastCity;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Node otherNode = obj as Node;
            if (otherNode != null)
                return this.pValue.CompareTo(otherNode.pValue);
            else
                throw new ArgumentException("Object is not a Node");
        }
    }
}
