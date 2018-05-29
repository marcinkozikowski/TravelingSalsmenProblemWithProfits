using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;

namespace Algorytmika
{
    class Algorithm
    {
        private int startedCity { get; set; } // miasto z ktorego zaczynami i konczymy

        private int quantity { get; set; } // ilosc tras ile mamy w

        private double Dmax { get; set; } // maksymalna trasa w km

        private int randomlyFlag = 0;

        public List<Node> NodesList { get; set; } // lista wszystkich punktow
        public List<Node> UnvisitedNodesList { get; set; } // lista wszystkich punktow
        public List<Tuple<Node, Node>> Connections = new List<Tuple<Node, Node>>();

        public double[,] NodeDistances { get; set; } // tablica dystansow.

        public int[] currentSequence;

        private int numberOfNodes;

        private int numberOfPaths;

        public void LoadData(string path)
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
            }
            else
            {
                LoadPolishRoadData(path);
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
            for(int i=0;i<numberOfNodes;i++)
            {
                string[] line = streamCheck.ReadLine().Split(' ');
                NodesList.Add(new Node
                {
                    Position = Convert.ToInt32(line[0]),
                    Profit = Convert.ToInt32(line[1]),
                    X = Convert.ToDouble(line[2])*10,
                    Y = Convert.ToDouble(line[3])*10
                });
            }
            //wczytywanie dystansow miedzy miastami

            
            UnvisitedNodesList = new List<Node>(NodesList);
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
            for (int i = 0; i < NodesList.Count; i++)
            {
                for (int j = 0; j < NodesList.Count; j++)
                {
                    var x = NodesList[i].X - NodesList[j].X;
                    var y = NodesList[i].Y - NodesList[j].Y;
                    NodeDistances[i, j] = Convert.ToDouble(Math.Floor(Math.Sqrt(x * x + y * y)));
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

        public Route GreedyRouteConstruction(double maxDistance)
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

            while (distance < maxDistance)
            {
                Node best = GetTheBestNode(currentNode,unvisited,distance,profit); //get whivh has the best overal profil to distance
                if (CheckDistance(distance,maxDistance,currentNode,best,startNode)) //check wether route back to start is possible
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

            return r;

        }

        public Route GreedyRandomlyRouteConstruction(double maxDistance)
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

            while (distance < maxDistance)
            {
                Node best = GetTheBestNodeWithRandomly(currentNode, unvisited, distance, profit); //get whivh has the best overal profil to distance
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
                    if (randomlyFlag == 17)
                    {
                        best = GetShortestWayNode(current, unvistedNodes);
                        break;
                    }
                    else
                    {
                        if ((n.Profit + profit) / (NodeDistances[current.Position, n.Position] + distance) > bestProfit)
                        {
                            best = n;
                            bestProfit = (n.Profit + profit) / (NodeDistances[current.Position, n.Position] + distance);
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

        private Node GetTheBestNode(Node current,List<Node> unvistedNodes,double distance,double profit)
        {
            Node best=new Node();
            double bestProfit = 0;
            foreach (var n in unvistedNodes)
            {
                if (current != n)
                {
                    if ((n.Profit+profit)/ (NodeDistances[current.Position, n.Position]+distance) > bestProfit)
                    {
                        best = n;
                        bestProfit = (n.Profit + profit) / (NodeDistances[current.Position, n.Position] + distance);
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

        #endregion

       public Route TwoOpt(Route currentRoute)
        {
            List<Node> newRoute = new List<Node>();
            List<Node> bestRoute = new List<Node>(currentRoute.CalculatedRoute);
            int n = currentRoute.CalculatedRoute.Count;
            double newDist = 0;
            double bestDist = currentRoute.Distance;
            bool improve = true;
                while (improve)
                {
                    improve = false;
                    for (int i = 1; i < n - 2; i++)
                    {
                        for (int k = i + 1; k < n - 1; k++)
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
            return r;

        }

        public Route Insert(Route currentRoute, int max)
        {
            Route optiPath = new Route();
            int n = currentRoute.CalculatedRoute.Count;
            List<Node> tempPath = new List<Node>();
            List<Node> currentPath = new List<Node>(currentRoute.CalculatedRoute);
            double profit = currentRoute.RouteProfit;
            double tempProfit = 0;
            double tempDistance = 0;
            bool improve = true;
            int count = 0;

            while (improve)
            {
                improve = false;
                for (int v = 0; v < UnvisitedNodesList.Count-1; v++)
                {
                    for (int i = 1; i < n - 1; i++)
                    {
                        tempPath = ConstructNewPath(i, UnvisitedNodesList.ElementAt(v), currentPath);
                        tempProfit = CalcProfit(tempPath);
                        tempDistance = CalcDistance(tempPath);
                        if ((tempProfit > profit) && (tempDistance <= max))
                        {
                            currentPath = tempPath;
                            profit = tempProfit;
                            improve = true;
                            UnvisitedNodesList.RemoveAt(v);

                        }
                    }
                }
                count++;
            }
            optiPath.RouteProfit = profit;
            optiPath.Distance = CalcDistance(currentPath);
            optiPath.CalculatedRoute = currentPath;
            return optiPath;
        }

        public Route ConstructAnotherRoute(int maxDistance)
        {
            double distance = 0;
            double profit = 0;
            List<Node> route = new List<Node>();    //construct route
            List<Node> unvisited = new List<Node>(UnvisitedNodesList);
            route.Add(NodesList.ElementAt(0));      //add first point to route
            Node currentNode;
            Node startNode = new Node();
            startNode = NodesList.ElementAt(0);
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

            r = TwoOpt(r);
            r = Insert(r, 7600);

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
            for (int i = 0; i < route.Count - 2; i++)
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
    }

    class Route
    {
        public List<Node> CalculatedRoute { get; set; }
        public double RouteProfit { get; set; }
        public double Distance { get; set; }
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
