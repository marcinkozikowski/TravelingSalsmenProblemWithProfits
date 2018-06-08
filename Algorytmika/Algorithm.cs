using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xaml;

namespace Algorytmika
{
    internal class Algorithm
    {
        private int startedCity { get; set; } // miasto z ktorego zaczynami i konczymy
        private int quantity { get; set; } // ilosc tras ile mamy w
        public double Dmax { get; set; } // maksymalna trasa w km
        private int randomlyFlag = 0;

        public List<Node> NodesList { get; set; } // lista wszystkich punktow
        public List<Node> UnvisitedNodesList { get; set; } // lista wszystkich punktow
        public List<Tuple<Node, Node>> Connections = new List<Tuple<Node, Node>>();
        public double[,] NodeDistances { get; set; } // tablica dystansow.
        public double[,] NodeDistancesDisjkstry { get; set; } // tablica dystansow.
        public int[] currentSequence;
        public int numberOfNodes;
        public int numberOfPaths;
        Random rand = new Random();
        public ArrayList[] incidenceList { get; set; }

        public bool LoadData(string path)
        {
            randomlyFlag = 0;
            if(NodesList!=null)
            {
                NodesList.Clear();
            }
            NodesList = new List<Node>();
            StreamReader streamCheck = new StreamReader(path);
            string[] lineCheck = streamCheck.ReadLine().Split(' ');
            streamCheck.Close();
            if (lineCheck.Length < 2)
            {
                LoadTestData(path);
                calcDistances();
                return true;
            }
            else
            {
                LoadPolishRoadData(path);
                return false;
            }
            UnvisitedNodesList = new List<Node>(NodesList);
        }

        public void LoadPolishRoadData(string path)
        {
            StreamReader streamCheck = new StreamReader(path);
            string[] lineCheck = streamCheck.ReadLine().Split(' ');

            numberOfNodes = Convert.ToInt32(lineCheck[0]);
            numberOfPaths = Convert.ToInt32(lineCheck[1]);

            //wczytywanie miast profitow i pozycji geograficznej
            for(int j=0;j<numberOfNodes;j++)
            {
                string[] linee = streamCheck.ReadLine().Split(' ');
                NodesList.Add(new Node
                {
                    Position = Convert.ToInt32(linee[0])-1,
                    Profit = Convert.ToInt32(linee[1]),
                    X = Convert.ToDouble(linee[2]),
                    Y = Convert.ToDouble(linee[3])
                });
            }
            UnvisitedNodesList = new List<Node>(NodesList);
            //wczytywanie dystansow miedzy miastami
            NodeDistances = new double[NodesList.Count, NodesList.Count];
            NodeDistancesDisjkstry = new double[NodesList.Count, NodesList.Count];
            int i = 0;
            string[] line=null;
            try
            {
                for (i = 0; i < numberOfPaths-1; i++)
                {
                    if(i>852)
                    {

                    }
                    line = streamCheck.ReadLine().Split(' ');
                    int from = Convert.ToInt32(line[0]);
                    int to = Convert.ToInt32(line[1]);
                    int distance = Convert.ToInt32(line[2]);
                    NodeDistances[from - 1, to - 1] = distance;
                    NodeDistances[to - 1, from - 1] = distance;
                    NodeDistancesDisjkstry[from - 1, to - 1] = distance;
                    NodeDistancesDisjkstry[to - 1, from - 1] = distance;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString() + "\n i:"+i+"\n"+line.ToString());
            }
            streamCheck.Close();
        }

        public void LoadTestData(string path)
        {
            using (StreamReader stream = new StreamReader(path))
            {
                numberOfNodes = Convert.ToInt32(stream.ReadLine());
                string line = null;
                int inc = 0;
                do
                {
                    line = stream.ReadLine();
                    if (line != null)
                    {
                        var data = line.Split(' ');
                        NodesList.Add(new Node
                        {
                            Position = inc++,
                            X = Double.Parse(data[0], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture),
                            Y = Double.Parse(data[1], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture),
                            Profit = Convert.ToDouble(data[2]),
                            Visited = false
                        });
                    }
                } while (line != null);
            }
            calcDistances();
        }

        private void calcDistances()
        {
            NodeDistances = new double[NodesList.Count, NodesList.Count];
            NodeDistancesDisjkstry = new double[NodesList.Count, NodesList.Count];
            for (int i = 0; i < NodesList.Count; i++)
            {
                for (int j = 0; j < NodesList.Count; j++)
                {
                    var x = NodesList[i].X - NodesList[j].X;
                    var y = NodesList[i].Y - NodesList[j].Y;
                    NodeDistances[i, j] = Convert.ToDouble(Math.Floor(Math.Sqrt(x * x + y * y)));
                    NodeDistancesDisjkstry[i,j] = Convert.ToDouble(Math.Floor(Math.Sqrt(x * x + y * y)));
                }
            }
        }

        public void getRoute(double maxDistance, int startPoint)
        {
            var currentNode = NodesList.First(p => p.Position == startPoint - 1);
            var list = new List<int>();
            currentNode.Visited = true;
            double currentProfit = currentNode.Profit;
            var notVisitedNodes = new List<Node>(NodesList);
            list.Add(currentNode.Position);
            notVisitedNodes.Remove(currentNode);

            double distance = 0;
            while (true)
            {
                Node nextNode = null;
                double bestResult = 0;
                //wez srednio najlepszy punkt.
                foreach (var notVisitedNode in NodesList)
                {
                    if (notVisitedNode.Visited == false)
                    {
                        var result = notVisitedNode.Profit /
                                     NodeDistances[currentNode.Position, notVisitedNode.Position];
                        if (result > bestResult &&
                            distance + NodeDistances[currentNode.Position, notVisitedNode.Position] <= maxDistance)
                        {
                            bestResult = result;
                            nextNode = notVisitedNode;
                        }
                    }
                }

                //isc randomowo po bliskich dosc dobrych miastach, i miec ok. pol odleglosci powrotnej w zapasie jak zblizymy sie do tej wartosci wracamy.
                if (nextNode == null)
                {
                    break;
                }

                nextNode.Visited = true;
                currentProfit += nextNode.Profit;
                distance += NodeDistances[currentNode.Position, nextNode.Position];
                //var pair = new Tuple<Node,Node>(currentNode,nextNode);
                //Connections.Add(pair);
                //list.Add(nextNode.Position);
                currentNode = nextNode;
            }

            currentSequence = list.ToArray();

        }

        #region Greedy Route Construction + help methods

        public Route GreedyRouteConstruction()
        {
            double distance = 0;
            double profit = 0;
            List<Node> route = new List<Node>();    //construct route
            List<Node> unvisited = new List<Node>(NodesList);
            Node currentNode;
            Node startNode = new Node();
            //int random = rand.Next(0, NodesList.Count - 1);
            //random = rand.Next(0, NodesList.Count - 1);
            startNode = NodesList.ElementAt(0);
            route.Add(startNode);   //add start point to route
            unvisited.Remove(startNode);
            currentNode = startNode;

            while (distance < Dmax)
            {
                Node best = GetTheBestNode(currentNode,unvisited,distance,profit); //get whivh has the best overal profil to distance
                if (CheckDistance(distance, Dmax, currentNode,best,startNode)) //check wether route back to start is possible
                {
                        distance = distance + NodeDistances[currentNode.Position, best.Position];
                        profit = profit + best.Profit;
                        route.Add(best);
                        unvisited.Remove(best);
                        currentNode = best;
                }
                else
                {
                    break;
                }
            }
            route.Add(route.ElementAt(0));
            distance = distance + NodeDistances[route.ElementAt(route.Count() - 2).Position, route.ElementAt(route.Count() - 1).Position];
            //UnvisitedNodesList = unvisited;
            Route r = new Route();
            r.CalculatedRoute = route;
            r.Distance = distance;
            r.RouteProfit = profit;
            r.Unvisited = unvisited;
            UnvisitedNodesList = unvisited;
            return r;

        }

        public Route GreedyRandomlyRouteConstruction()
        {
            double distance = 0;
            double profit = 0;
            List<Node> route = new List<Node>();    //construct route
            List<Node> unvisited = new List<Node>(NodesList);
            route.Add(NodesList.ElementAt(0));      //add first point to route
            Node currentNode;
            Node startNode = new Node();
            startNode = NodesList.ElementAt(0);
            unvisited.Remove(startNode);
            currentNode = startNode;

            while (distance < Dmax)
            {
                Node best = GetTheBestNodeWithRandomly(currentNode, unvisited, distance, profit); //get whivh has the best overal profil to distance
                if (CheckDistance(distance, Dmax, currentNode, best, startNode)) //check wether route back to start is possible
                {
                    distance = distance + NodeDistances[currentNode.Position, best.Position];
                    profit = profit + best.Profit;
                    route.Add(best);
                    unvisited.Remove(best);
                    currentNode = best;
                }
                else
                {
                    break;
                }
            }
            route.Add(route.ElementAt(0));
            Route r = new Route();
            r.Unvisited = unvisited;
            r.CalculatedRoute = route;
            r.Distance = distance;
            r.RouteProfit = profit;
            UnvisitedNodesList = unvisited;
            return r;
        }

        private Node GetTheBestNodeWithRandomly(Node current, List<Node> unvistedNodes, double distance, double profit)
        {
            Node best = new Node();
            double bestProfit = 0;

            foreach (var n in unvistedNodes)
            {
                if (current != n)
                {
                    if (randomlyFlag == 18)
                    {
                        best = GetShortestWayNode(current, unvistedNodes);
                        randomlyFlag = 0;
                        break;
                    }
                    else
                    {
                        if ((n.Profit) / (NodeDistances[current.Position, n.Position]) > bestProfit)
                        {
                            best = n;
                            bestProfit = (n.Profit) / (NodeDistances[current.Position, n.Position]);
                            randomlyFlag++;
                        }
                    }
                }
            }

            return best;
        }

        private Node GetShortestWayNode(Node current, List<Node> unvisitedNodes)
        {
            double bestDist = double.MaxValue;
            Node best = new Node();
            foreach(var n in unvisitedNodes)
            {
                if(NodeDistances[current.Position, n.Position] < bestDist)
                {
                    best = n;
                    bestDist = NodeDistances[current.Position, n.Position];
                }
            }
            return best;     
        }

        private bool CheckDistance(double currentDistance, double maxDistance, Node current, Node next,Node start)
        {
            if (NodeDistances[current.Position, next.Position] + currentDistance < maxDistance)
            {
                double tempDistance = NodeDistances[current.Position, next.Position] + currentDistance;
                if (tempDistance + NodeDistances[next.Position, start.Position] <= maxDistance)
                {
                    return true;
                }
            }
            return false;
        }

        //private Node GetTheBestNode(Node current,List<Node> unvistedNodes,double distance,double profit)
        //{
        //    Node best=new Node();
        //    double bestProfit = 0;
        //    foreach (var n in unvistedNodes)
        //    {
        //        if (current != n)
        //        {
        //            if ((n.Profit+profit)/ (NodeDistances[current.Position, n.Position]+distance) > bestProfit)
        //            {
        //                best = n;
        //                bestProfit = (n.Profit + profit) / (NodeDistances[current.Position, n.Position] + distance);
        //            }
        //        }
        //    }

        //    return best;
        //}

        private Node GetTheBestNode(Node current, List<Node> unvistedNodes, double distance, double profit)
        {
            Node best = new Node();
            double bestProfit = 0;
            foreach (var n in unvistedNodes)
            {
                if (current != n)
                {
                    if (NodeDistances[current.Position, n.Position] != double.MaxValue)
                    {
                        if ((n.Profit) / (NodeDistances[current.Position, n.Position]) > bestProfit)
                        {
                            best = n;
                            bestProfit = (n.Profit) / (NodeDistances[current.Position, n.Position]);
                        }
                    }
                }
            }

            return best;
        }

        private Node GetNodeWithHighestProfit()
        {
            double profit = 0;
            Node best = new Node();
            foreach (var n in NodesList)
            {
                if (n.Profit > profit)
                {
                    best = n;
                    profit = best.Profit;
                }
            }

            return best;

        }

        public Route GreedyLocalSearch(int numberOfIterations)
        {
            List<Route> routs = new List<Route>();
            for (int i=0; i < numberOfIterations;i++)
            {
                routs.Add(GreedyRouteConstruction());
            }
            Route best = routs.ElementAt(0);
            Route temp;
            foreach(var r in routs)
            {
                temp = LocalSearch(r);
                if(temp.RouteProfit>best.RouteProfit)
                {
                    best = temp;
                }
                
            }
            return best;
        }

        public Route GreedyLocalSearch2(int numberOfRouts)
        {
            // conctruct N routes then find best one and take localsearch on it
            List<Route> routs = new List<Route>();
            for (int i = 0; i < numberOfRouts; i++)
            {
                routs.Add(GreedyRouteConstruction());
            }
            Route best = routs.ElementAt(0);

            foreach(Route r in routs)
            {
                if(r.RouteProfit>best.RouteProfit)
                {
                    best = r;
                }
            }
            //best.CalculatedRoute.Add(best.CalculatedRoute.ElementAt(0));
            best = LocalSearch(best);
            return best;
        }

        #endregion

        public Route TwoOpt(Route currentRoute)
        {
            List<Node> newRoute = new List<Node>();
            List<Node> unvisited = new List<Node>(currentRoute.Unvisited);
            List<Node> bestRoute = new List<Node>(currentRoute.CalculatedRoute);
            int n = currentRoute.CalculatedRoute.Count;
            double newDist = 0;
            double newProfit = 0;
            double bestDist = currentRoute.Distance;
            double bestProfit = currentRoute.RouteProfit;
            bool improve = true;
                while (improve)
                {
                    improve = false;
                    for (int i = 1; i < n - 1; i++)
                    {
                        for (int k = i + 1; k < n-1; k++)
                        {
                            newRoute = optSwap(bestRoute, i, k);
                            newDist = CalcDistance(newRoute);
                            if (newDist < bestDist)
                            {
                                bestRoute = newRoute;
                                bestDist = newDist;
                                improve = true;
                            }
                        }
                    }
                }
            Route r = new Route();
            r.CalculatedRoute = bestRoute;
            r.Distance = bestDist;
            r.RouteProfit = CalcProfit(bestRoute);
            r.Unvisited = unvisited;
            return r;

        }

        public Route Insert(Route currentRoute,double max)
        {
            // przekazac liste nieodwiedzonych do trasy
            Route optiPath = new Route();
            int n = currentRoute.CalculatedRoute.Count;
            List<Node> tempPath = new List<Node>();
            List<Node> currentPath = new List<Node>(currentRoute.CalculatedRoute);
            List<Node> unvisited = new List<Node>(currentRoute.Unvisited);
            double profit = currentRoute.RouteProfit;
            double tempProfit = 0;
            double tempDistance = 0;
            bool improve = true;
            int count = 0;

            while (improve)
            {
                improve = false;
                for (int v = 0; v < unvisited.Count-1; v++)
                {
                    for (int i = 1; i < n - 1; i++)
                    {
                        tempPath = ConstructNewPath(i, unvisited.ElementAt(v), currentPath);
                        tempProfit = CalcProfit(tempPath);
                        tempDistance = CalcDistance(tempPath);
                        if ((tempProfit > profit) && (tempDistance <= max))
                        {
                            currentPath = tempPath;
                            profit = tempProfit;
                            improve = true;
                            unvisited.RemoveAt(v);

                        }
                    }
                }
                count++;
            }
            optiPath.RouteProfit = profit;
            optiPath.Distance = CalcDistance(currentPath);
            optiPath.CalculatedRoute = currentPath;
            optiPath.Unvisited = unvisited;
            return optiPath;
        }

        public Route LocalSearch(Route route)
        {
            double bestProfit = route.RouteProfit;
            Route bestRout = route;
            Route temp;
            bool improve = true;
            while(improve)
            {
                improve = false;
                temp = TwoOpt(bestRout);
                if (temp.Distance <= bestRout.Distance)
                {
                    temp = Insert(temp, Dmax);
                    if (temp.RouteProfit>bestRout.RouteProfit)
                    {
                        improve = true;
                        bestRout = temp;
                    }
                }
            }
            return bestRout;
        }

        public Route ConstructAnotherRoute(double maxDistance, Route first)
        {
            double distance = 0;
            double profit = 0;
            List<Node> route = new List<Node>();    //construct route
            List<Node> unvisited = new List<Node>(first.Unvisited);
            route.Add(first.CalculatedRoute.ElementAt(0));      //add first point to route
            Node currentNode;
            Node startNode = new Node();
            startNode = first.CalculatedRoute.ElementAt(0);
            unvisited.Remove(startNode);
            currentNode = startNode;

            while (distance < maxDistance)
            {
                Node best = GetTheBestNode(currentNode, unvisited, distance, profit); //get whivh has the best overal profil to distance
                if (CheckDistance(distance, maxDistance, currentNode, best, startNode)) //check wether route back to start is possible
                {
                    distance = distance + NodeDistances[currentNode.Position, best.Position];
                    profit = profit + best.Profit;
                    route.Add(best);
                    unvisited.Remove(best);
                    currentNode = best;
                }
                else
                {
                    break;
                }
            }
            UnvisitedNodesList = unvisited;
            Route r = new Route();
            r.CalculatedRoute = route;
            r.Distance = distance;
            r.RouteProfit = profit;
            r.Unvisited = unvisited;

            r = LocalSearch(r);

            return r;
        }

        private List<Node> ConstructNewPath(int insertIdx,Node insertNode, List<Node> currentPath)
        {
            List<Node> newPath = new List<Node>();

            //add first half of current path
            for(int i=0;i<insertIdx;i++)
            {
                newPath.Add(currentPath.ElementAt(i));
            }
            //add node to list on insert index
            newPath.Add(insertNode);
            for(int i=insertIdx;i<currentPath.Count;i++)
            {
                newPath.Add(currentPath.ElementAt(i));
            }
            return newPath;
        }

        private double CalcDistance(List<Node> route)
        {
            double dist = 0;
            for (int i = 0; i <= route.Count - 2; i++)
            {
                dist = dist + NodeDistances[route.ElementAt(i).Position, route.ElementAt(i + 1).Position];
            }
            return dist;
        }

        private double CalcProfit(List<Node> route)
        {
            double dist = 0;
            foreach (var n in route)
            {
                dist = dist + n.Profit;
            }
            return dist;
        }

        private List<Node> optSwap(List<Node >route,int i, int k)
        {
            Node current;
            List<Node> newRoute=new List<Node>();
            List<Node> temp = new List<Node>();
            List<Node> order = new List<Node>();


            //1.take route[0] to route[i - 1] and add them in order to new_route
            for (int a = 0; a < i; a++)
            {
                newRoute.Add(route.ElementAt(a));
            }
            //2.take route[i] to route[k] and add them in reverse order to new_route
            for (int b = i; b < k + 1; b++)
            {
                temp.Add(route.ElementAt(b));
            }
            temp.Reverse();
            newRoute.AddRange(temp);
            //3.take route[k + 1] to end and add them in order to new_route
            for (int c = k +1; c < route.Count; c++)
            {
                newRoute.Add(route.ElementAt(c));
            }
            
            return newRoute;
        }

        public static int[] TwoOptSwap(int[] sequence, int i, int k)
        {
            List<Node> next = new List<Node>();
            
            int[] nextSequence = new int[sequence.Length];
            Array.Copy(sequence, nextSequence, sequence.Length);
            Array.Reverse(nextSequence, i, k - i + 1);
            return nextSequence;
        }

        public List<Node> getDijkstryNodes(int src, int dst)
        {
            Dijkstry di = new Dijkstry(incidenceList);
            List<Node> dijkstryPath = new List<Node>();
            int[] path = di.GetPath(src, dst);
            if (path != null)
            {
                //convert from int[] to List<Node>
                foreach (var p in path)
                {
                    foreach (var n in NodesList)
                    {
                        if (n.Position == p)
                        {
                            dijkstryPath.Add(n);
                            break;
                        }
                    }
                }
            }
            return dijkstryPath;
        }
    }

    class Route
    {
        public List<Node> CalculatedRoute { get; set; }
        public double RouteProfit { get; set; }
        public double Distance { get; set; }
        public List<Node> Unvisited { get; set; }
    }

    class Node
    {
        public int Position { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Profit { get; set; }
        public bool Visited { get; set; }
    }
}
