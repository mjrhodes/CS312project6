using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using Wintellect.PowerCollections;

namespace TSP
{

    class ProblemAndSolver
    {

        private class TSPSolution
        {
            /// <summary>
            /// we use the representation [cityB,cityA,cityC] 
            /// to mean that cityB is the first city in the solution, cityA is the second, cityC is the third 
            /// and the edge from cityC to cityB is the final edge in the path.  
            /// You are, of course, free to use a different representation if it would be more convenient or efficient 
            /// for your data structure(s) and search algorithm. 
            /// </summary>
            public ArrayList
                Route;

            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="iroute">a (hopefully) valid tour</param>
            public TSPSolution(ArrayList iroute)
            {
                Route = new ArrayList(iroute);
            }

            /// <summary>
            /// Compute the cost of the current route.  
            /// Note: This does not check that the route is complete.
            /// It assumes that the route passes from the last city back to the first city. 
            /// </summary>
            /// <returns></returns>
            public double costOfRoute()
            {
                // go through each edge in the route and add up the cost. 
                int x;
                City here;
                double cost = 0D;

                for (x = 0; x < Route.Count - 1; x++)
                {
                    here = Route[x] as City;
                    cost += here.costToGetTo(Route[x + 1] as City);
                }

                // go from the last city to the first. 
                here = Route[Route.Count - 1] as City;
                cost += here.costToGetTo(Route[0] as City);
                return cost;
            }
        }

        #region Private members 

        /// <summary>
        /// Default number of cities (unused -- to set defaults, change the values in the GUI form)
        /// </summary>
        // (This is no longer used -- to set default values, edit the form directly.  Open Form1.cs,
        // click on the Problem Size text box, go to the Properties window (lower right corner), 
        // and change the "Text" value.)
        private const int DEFAULT_SIZE = 25;

        /// <summary>
        /// Default time limit (unused -- to set defaults, change the values in the GUI form)
        /// </summary>
        // (This is no longer used -- to set default values, edit the form directly.  Open Form1.cs,
        // click on the Time text box, go to the Properties window (lower right corner), 
        // and change the "Text" value.)
        private const int TIME_LIMIT = 60;        //in seconds

        private const int CITY_ICON_SIZE = 5;


        // For normal and hard modes:
        // hard mode only
        private const double FRACTION_OF_PATHS_TO_REMOVE = 0.20;

        /// <summary>
        /// the cities in the current problem.
        /// </summary>
        private City[] Cities;
        /// <summary>
        /// a route through the current problem, useful as a temporary variable. 
        /// </summary>
        private ArrayList Route;
        /// <summary>
        /// best solution so far. 
        /// </summary>
        private TSPSolution bssf; 

        /// <summary>
        /// how to color various things. 
        /// </summary>
        private Brush cityBrushStartStyle;
        private Brush cityBrushStyle;
        private Pen routePenStyle;

        private int count;
        private int timesImproved;
        private int statesMade;
        private int statesChopped;
        private int maxStored;


        /// <summary>
        /// keep track of the seed value so that the same sequence of problems can be 
        /// regenerated next time the generator is run. 
        /// </summary>
        private int _seed;
        /// <summary>
        /// number of cities to include in a problem. 
        /// </summary>
        private int _size;

        /// <summary>
        /// Difficulty level
        /// </summary>
        private HardMode.Modes _mode;

        /// <summary>
        /// random number generator. 
        /// </summary>
        private Random rnd;

        /// <summary>
        /// time limit in milliseconds for state space search
        /// can be used by any solver method to truncate the search and return the BSSF
        /// </summary>
        private int time_limit;
        #endregion

        #region Public members

        /// <summary>
        /// These three constants are used for convenience/clarity in populating and accessing the results array that is passed back to the calling Form
        /// </summary>
        public const int COST = 0;           
        public const int TIME = 1;
        public const int COUNT = 2;
        
        public int Size
        {
            get { return _size; }
        }

        public int Seed
        {
            get { return _seed; }
        }
        #endregion

        #region Constructors
        public ProblemAndSolver()
        {
            this._seed = 1; 
            rnd = new Random(1);
            this._size = DEFAULT_SIZE;
            this.time_limit = TIME_LIMIT * 1000;                  // TIME_LIMIT is in seconds, but timer wants it in milliseconds

            this.resetData();
        }

        public ProblemAndSolver(int seed)
        {
            this._seed = seed;
            rnd = new Random(seed);
            this._size = DEFAULT_SIZE;
            this.time_limit = TIME_LIMIT * 1000;                  // TIME_LIMIT is in seconds, but timer wants it in milliseconds

            this.resetData();
        }

        public ProblemAndSolver(int seed, int size)
        {
            this._seed = seed;
            this._size = size;
            rnd = new Random(seed);
            this.time_limit = TIME_LIMIT * 1000;                        // TIME_LIMIT is in seconds, but timer wants it in milliseconds

            this.resetData();
        }
        public ProblemAndSolver(int seed, int size, int time)
        {
            this._seed = seed;
            this._size = size;
            rnd = new Random(seed);
            this.time_limit = time*1000;                        // time is entered in the GUI in seconds, but timer wants it in milliseconds

            this.resetData();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Reset the problem instance.
        /// </summary>
        private void resetData()
        {

            Cities = new City[_size];
            Route = new ArrayList(_size);
            bssf = null;

            if (_mode == HardMode.Modes.Easy)
            {
                for (int i = 0; i < _size; i++)
                    Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble());
            }
            else // Medium and hard
            {
                for (int i = 0; i < _size; i++)
                    Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble() * City.MAX_ELEVATION);
            }

            HardMode mm = new HardMode(this._mode, this.rnd, Cities);
            if (_mode == HardMode.Modes.Hard)
            {
                int edgesToRemove = (int)(_size * FRACTION_OF_PATHS_TO_REMOVE);
                mm.removePaths(edgesToRemove);
            }
            City.setModeManager(mm);

            cityBrushStyle = new SolidBrush(Color.Black);
            cityBrushStartStyle = new SolidBrush(Color.Red);
            routePenStyle = new Pen(Color.Blue,1);
            routePenStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// make a new problem with the given size.
        /// </summary>
        /// <param name="size">number of cities</param>
        public void GenerateProblem(int size, HardMode.Modes mode)
        {
            this._size = size;
            this._mode = mode;
            resetData();
        }

        /// <summary>
        /// make a new problem with the given size, now including timelimit paremeter that was added to form.
        /// </summary>
        /// <param name="size">number of cities</param>
        public void GenerateProblem(int size, HardMode.Modes mode, int timelimit)
        {
            this._size = size;
            this._mode = mode;
            this.time_limit = timelimit*1000;                                   //convert seconds to milliseconds
            resetData();
        }

        /// <summary>
        /// return a copy of the cities in this problem. 
        /// </summary>
        /// <returns>array of cities</returns>
        public City[] GetCities()
        {
            City[] retCities = new City[Cities.Length];
            Array.Copy(Cities, retCities, Cities.Length);
            return retCities;
        }

        /// <summary>
        /// draw the cities in the problem.  if the bssf member is defined, then
        /// draw that too. 
        /// </summary>
        /// <param name="g">where to draw the stuff</param>
        public void Draw(Graphics g)
        {
            float width  = g.VisibleClipBounds.Width-45F;
            float height = g.VisibleClipBounds.Height-45F;
            Font labelFont = new Font("Arial", 10);

            // Draw lines
            if (bssf != null)
            {
                // make a list of points. 
                Point[] ps = new Point[bssf.Route.Count];
                int index = 0;
                foreach (City c in bssf.Route)
                {
                    if (index < bssf.Route.Count -1)
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[index+1]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    else 
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[0]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    ps[index++] = new Point((int)(c.X * width) + CITY_ICON_SIZE / 2, (int)(c.Y * height) + CITY_ICON_SIZE / 2);
                }

                if (ps.Length > 0)
                {
                    g.DrawLines(routePenStyle, ps);
                    g.FillEllipse(cityBrushStartStyle, (float)Cities[0].X * width - 1, (float)Cities[0].Y * height - 1, CITY_ICON_SIZE + 2, CITY_ICON_SIZE + 2);
                }

                // draw the last line. 
                g.DrawLine(routePenStyle, ps[0], ps[ps.Length - 1]);
            }

            // Draw city dots
            foreach (City c in Cities)
            {
                g.FillEllipse(cityBrushStyle, (float)c.X * width, (float)c.Y * height, CITY_ICON_SIZE, CITY_ICON_SIZE);
            }

        }

        /// <summary>
        ///  return the cost of the best solution so far. 
        /// </summary>
        /// <returns></returns>
        public double costOfBssf ()
        {
            if (bssf != null)
                return (bssf.costOfRoute());
            else
                return -1D; 
        }

        /// <summary>
        /// This is the entry point for the default solver
        /// which just finds a valid random tour 
        /// </summary>
        /// <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution, number of solutions found during search (not counting initial BSSF estimate)</returns>
        public string[] defaultSolveProblem()
        {
            int i, swap, temp, count=0;
            string[] results = new string[3];
            int[] perm = new int[Cities.Length];
            Route = new ArrayList();
            Random rnd = new Random();
            Stopwatch timer = new Stopwatch();

            timer.Start();

            do
            {
                for (i = 0; i < perm.Length; i++)                                 // create a random permutation template
                    perm[i] = i;
                for (i = 0; i < perm.Length; i++)
                {
                    swap = i;
                    while (swap == i)
                        swap = rnd.Next(0, Cities.Length);
                    temp = perm[i];
                    perm[i] = perm[swap];
                    perm[swap] = temp;
                }
                Route.Clear();
                for (i = 0; i < Cities.Length; i++)                            // Now build the route using the random permutation 
                {
                    Route.Add(Cities[perm[i]]);
                }
                bssf = new TSPSolution(Route);
                count++;
            } while (costOfBssf() == double.PositiveInfinity);                // until a valid route is found
            timer.Stop();

            results[COST] = costOfBssf().ToString();                          // load results array
            results[TIME] = timer.Elapsed.ToString();
            results[COUNT] = count.ToString();

            return results;
        }

        /// <summary>
        /// performs a Branch and Bound search of the state space of partial tours
        /// stops when time limit expires and uses BSSF as solution
        /// </summary>
        /// <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution, number of solutions found during search (not counting initial BSSF estimate)</returns>
        public string[] bBSolveProblem()
        {
            count = 0;
            timesImproved = 0;
            statesMade = 0;
            statesChopped = 0;
            maxStored = 0;
            string[] results = new string[3];
            Stopwatch timer = new Stopwatch();

            // TODO: Add your implementation for a branch and bound solver here.
            getInitialBSSF();
            double[,] matrix = getInitialMatrix();
            double b = 0;


            Node initialNode = new Node(Cities.Length);
            initialNode.setMatrix(reduceMatrix(matrix, ref b));
            initialNode.setB(b);
            initialNode.addToRoute(0);

            OrderedBag<Node> q = new OrderedBag<Node>();
            q.Add(initialNode);
            statesMade++;
            maxStored++;

            timer.Start();
            while(q.Count > 0 && timer.Elapsed.TotalSeconds < TIME_LIMIT )
            {
                Node current = q.RemoveFirst();
                if(current.getRoute().Count == Cities.Length)
                {
                    ArrayList thisRoute = current.getRoute();
                    if (!double.IsPositiveInfinity(current.getMatrix()[current.getLastCity(), 0]))
                    {
                        ArrayList routeAsCities = getRouteAsCities(current.getRoute());
                        TSPSolution thisSol = new TSPSolution(routeAsCities);
                        if(thisSol.costOfRoute() < bssf.costOfRoute())
                        {
                            bssf = thisSol;
                            timesImproved++;
                            Console.WriteLine(bssf.costOfRoute());
                        }
                    }
                    count++;
                }
                else
                {
                    if (current.getB() > bssf.costOfRoute())
                    {
                        statesChopped++;
                        continue;
                    }
                    addChildrenToQueue(q, current);
                }
            }
            timer.Stop();

            Console.WriteLine("times improved: {0}", timesImproved);
            Console.WriteLine("states made: {0}", statesMade);
            Console.WriteLine("states chopped: {0}", statesChopped);
            Console.WriteLine("max stored: {0}", maxStored);

            results[COST] = bssf.costOfRoute().ToString();    // load results into array here, replacing these dummy values
            results[TIME] = timer.Elapsed.ToString();
            results[COUNT] = count.ToString();

            return results;
        }

        private void getInitialBSSF()
        {
            defaultSolveProblem();
            ArrayList bestRoute = Route;
            TSPSolution best = new TSPSolution(Route);
            for (int i = 0; i < 5; i++)
            {
                defaultSolveProblem();
                TSPSolution newRoute = new TSPSolution(Route);
                if (newRoute.costOfRoute() < best.costOfRoute())
                {
                    bestRoute = Route;
                    best = newRoute;
                }
            }
            bssf = best;
        }

        private double[,] getInitialMatrix()
        {
            double[,] matrix = new double[Cities.Length, Cities.Length];
            for(int i = 0; i < Cities.Length; i++)
            {
                for (int j = 0; j < Cities.Length; j++)
                {
                    matrix[i, j] = Cities[i].costToGetTo(Cities[j]);
                    if (i == j) matrix[i, j] = double.PositiveInfinity;
                }
            }
            return matrix;
        }

        private double[,] reduceMatrix(double[,] matrix, ref double b)
        {
            //Reduce Rows
            for (int i = 0; i < Cities.Length; i++)
            {
                double min = double.PositiveInfinity;
                for(int j = 0; j < Cities.Length; j++)
                {
                    if (matrix[i, j] < min) min = matrix[i, j];
                }
                if (double.IsPositiveInfinity(min)) continue;
                b += min;
                for(int j = 0; j < Cities.Length; j++)
                {
                    if (double.IsPositiveInfinity(matrix[i, j])) continue;
                    matrix[i, j] -= min;
                }
            }

            //Reduce Columns
            for (int j = 0; j < Cities.Length; j++)
            {
                double min = double.PositiveInfinity;
                for(int i = 0; i < Cities.Length; i++)
                {
                    if(double.IsPositiveInfinity(matrix[i,j])) continue;
                    if (matrix[i, j] < min) min = matrix[i, j];
                }
                if (double.IsPositiveInfinity(min)) continue;
                b += min;
                for(int i = 0; i < Cities.Length; i++)
                {
                    if (double.IsPositiveInfinity(matrix[i, j])) continue;
                    matrix[i, j] -= min;
                }
            }

            return matrix;
        }

        private void addChildrenToQueue(OrderedBag<Node> q, Node current)
        {
            for(int i = 1; i < Cities.Length; i++)
            {
                if(!current.getRoute().Contains(i))
                {
                    double[,] newMatrix = copyMatrix(current.getMatrix());
                    newMatrix = visitFromTo(newMatrix, current.getLastCity(), i);
                    double newB = current.getB();
                    newMatrix = reduceMatrix(newMatrix, ref newB);

                    //Don't add child if it won't lead to better solution
                    if (newB > bssf.costOfRoute())
                    {
                        statesChopped++;
                        continue;
                    }

                    ArrayList newRoute = new ArrayList();
                    foreach(int element in current.getRoute())
                    {
                        newRoute.Add(element);
                    }
                    Node childNode = new Node(Cities.Length);
                    childNode.setRoute(newRoute);
                    childNode.addToRoute(i);
                    childNode.setB(newB);
                    childNode.setMatrix(newMatrix);
                    childNode.setPValue(calcPValue(newB, newRoute.Count));
                    q.Add(childNode);
                    statesMade++;
                    if (q.Count > maxStored) maxStored = q.Count;
                }
            }
        }

        private double calcPValue(double b, int depth)
        {
            return b/(depth^2);
        }

        private double[,] visitFromTo(double[,] matrix, int from, int to)
        {
            for(int i = 0; i < Cities.Length; i++)
            {
                matrix[from, i] = double.PositiveInfinity;
                matrix[i, to] = double.PositiveInfinity;
            }
            matrix[to, from] = double.PositiveInfinity;
            return matrix;
        }

        private ArrayList getRouteAsCities(ArrayList route)
        {
            ArrayList routeAsCities = new ArrayList();
            for(int i = 0; i < route.Count; i++)
            {
                int index = Int32.Parse(route[i].ToString());
                routeAsCities.Add(Cities[index]);
            }
            return routeAsCities;
        }

        private double[,] copyMatrix(double[,] matrix)
        {
            double[,] copy = new double[Cities.Length, Cities.Length];
            for (int i = 0; i < Cities.Length; i++)
            {
                for (int j = 0; j < Cities.Length; j++)
                {
                    copy[i, j] = matrix[i, j];
                }
            }
            return copy;
        }

        private void printMatrix(double[,] matrix)
        {
            for (int i = 0; i < Cities.Length; i++)
            {
                for (int j = 0; j < Cities.Length; j++)
                {
                    Console.Write("{0,8}", matrix[i, j]);
                }
                Console.Write("\n");
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // These additional solver methods will be implemented as part of the group project.
        ////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// finds the greedy tour starting from each city and keeps the best (valid) one
        /// </summary>
        /// <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution, number of solutions found during search (not counting initial BSSF estimate)</returns>
        public string[] greedySolveProblem()
        {

            // TODO: Add your implementation for a greedy solver here.

            //1.V = { 1, ..., n - 1}          // Vertices except for 0.
            ArrayList cities = new ArrayList();
            for(int i = 1; i < this.Cities.Length; i++)
            {
                cities.Add(this.Cities[i]);
            }

            //2.U = { 0}                    // Vertex 0.
            ArrayList updatedCities = new ArrayList() { this.Cities[0] };

            Stopwatch timer = new Stopwatch();
            timer.Start();
            //3.   while V not empty
            while (cities.Count > 0)
            {
                //4.u = most recently added vertex to U
                City currentCity = (City)updatedCities[updatedCities.Count - 1];

                //5.Find vertex v in V closest to u
                City closestCity = (City)cities[0];
                double distanceToClosestCity = currentCity.costToGetTo(closestCity);
                int closestCityIndex = 0;
                for (int i = 1; i < cities.Count; i++)
                {
                    if(distanceToClosestCity > currentCity.costToGetTo((City)cities[i]))
                    {
                        closestCityIndex = i;
                        closestCity = (City)cities[closestCityIndex];
                        distanceToClosestCity = currentCity.costToGetTo(closestCity);
                    }
                }

                //6.Add v to U and remove v from V.
                updatedCities.Add(closestCity);
                cities.RemoveAt(closestCityIndex);
            } //7.endwhile
            timer.Stop();

            //8.Output vertices in the order they were added to U
            this.Route = new ArrayList();
            for (int i = 0; i < Cities.Length; i++)                            // Now build the route
            {
                this.Route.Add(updatedCities[i]);
            }

            this.bssf = new TSPSolution(Route);
            count++;

            string[] results = new string[3];
            results[COST] = costOfBssf().ToString();                          // load results array
            results[TIME] = timer.Elapsed.ToString();
            results[COUNT] = count.ToString();

            return results;
        }

        public string[] fancySolveProblem()
        {
            string[] results = new string[3];
            Random rand = new Random();

            // TODO: Add your implementation for your advanced solver here.

            Edge[] MST = getMST();
            int[] oddVertices = findOddDegrees(MST);
            Edge[] newMST = matchOddDegreeVertices(MST, oddVertices);

            results[COST] = "not implemented";    // load results into array here, replacing these dummy values
            results[TIME] = "-1";
            results[COUNT] = "-1";

            return results;
        }

        /* 
         * MST : Minimum Spanning Tree
         * Iterating through 'Cities' to evaluate travel costs,
         * adds costs to PQ,
         * and adds smallest cost edges to array representing a MST
         */ 
        private Edge[] getMST()
        {
            ArrayQueue2 q = new ArrayQueue2(Cities.Length);
            ArrayList edges = new ArrayList();
            HashSet<int> s = new HashSet<int>();

            s.Add(0);
            for (int i = 1; i < Cities.Length; i++)
            {
                double cost = Cities[0].costToGetTo(Cities[i]);
                if (!double.IsPositiveInfinity(cost)) q.insert(new Edge(0, i, cost));
            }

            while(q.getSize() > 0)
            {
                Edge edge = q.deletemin();
                int city = edge.getDestination();
                if (s.Contains(city)) continue;
                s.Add(city);
                edges.Add(edge);
                for(int i = 0; i < Cities.Length; i++)
                {
                    if (s.Contains(i)) continue;
                    double cost = Cities[city].costToGetTo(Cities[i]);
                    if (!double.IsPositiveInfinity(cost)) q.insert(new Edge(city, i, cost));
                }
                if (edges.Count == Cities.Length - 1) break;
            }

            Edge[] newArray = new Edge[edges.Count];
            edges.CopyTo(newArray);

            return newArray;
        }

        /*
         * Takes a MST and returns a list of indices representing vertices with odd degrees
         * (degrees = number of edges going in/out of vertex)
         */
        private int[] findOddDegrees(Edge[] MST)
        {
            ArrayList vertices = new ArrayList();
            int[] counts = new int[Cities.Length];
            foreach(Edge e in MST)
            {
                counts[e.getOrigin()] = counts[e.getOrigin()] + 1;
                counts[e.getDestination()] = counts[e.getDestination()] + 1;
            }

            Console.WriteLine(counts.ToString());

            for(int i = 0; i < Cities.Length; i++)
            {
                if(counts[i] % 2 != 0) vertices.Add(i);
            }
            int[] oddVerts = new int[vertices.Count];
            vertices.CopyTo(oddVerts);

            Console.WriteLine(oddVerts.ToString());

            return oddVerts;
        }

        /*
         * Takes a MST and a list of vertices with odd degrees
         * matches edges between pairs of vertices (which have odd degrees) in MST (if destination can be reached)
         * vertices located at indices given in int[] vertices
         */
        private Edge[] matchOddDegreeVertices(Edge[] MST, int[] vertices)
        {
            ArrayList newALMST = new ArrayList();
            foreach(Edge e in MST) newALMST.Add(e);

            ArrayList remaining = new ArrayList();
            foreach(int v in vertices) remaining.Add(v);
            
            while(remaining.Count > 0)
            {
                IEnumerator e = remaining.GetEnumerator();
                e.MoveNext();
                int city = (int)e.Current;
                int dest = -1;
                double min = double.PositiveInfinity;
                while(e.MoveNext())
                {
                    bool flag = false;
                    foreach(Edge edge in MST)
                    {
                        if (edge.getOrigin() == city && edge.getDestination() == (int)e.Current)
                        {
                            flag = true;
                            break;
                        }
                        if (edge.getOrigin() == (int)e.Current && edge.getDestination() == city)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag) continue;
                    double cost = Cities[city].costToGetTo(Cities[(int)e.Current]);
                    if (cost < min)
                    {
                        min = cost;
                        dest = (int)e.Current;
                    }
                }
                if (dest == -1) throw new IndexOutOfRangeException("Couldn't find matching!");
                remaining.Remove(city);
                remaining.Remove(dest);
                newALMST.Add(new Edge(city, dest, Cities[city].costToGetTo(Cities[dest])));
            }

            Edge[] newMST = new Edge[newALMST.Count];
            newALMST.CopyTo(newMST);
            return newMST;
        }

        /*
         * Takes a MST and finds a Eulerian tour by traversing edges in MST
         * adds cities visited in order to Route
         */
        private ArrayList findEulerianTour(Edge[] MST)
        {
            //TODO
            //add cities visited by tour to Route
            return Route;
        }

        /*
         * Checks cities in Route
         * Where a city would be revisited,
         * skip that city and visit next city in Route, if next city is reachable
         */
        private ArrayList skipDuplicates()
        {
            //TODO
            //Find revisited vertices and skip
            return Route; // Completed TSP tour
        }

        #endregion
    }

}
