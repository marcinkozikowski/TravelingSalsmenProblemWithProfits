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

        public List<Node> NodesList { get; set; } // lista wszystkich punktow
        public List<Tuple<Node, Node>> Connections = new List<Tuple<Node, Node>>();

        public double[,] NodeDistances { get; set; } // tablica dystansow.

        public int[] currentSequence;

        private int numberOfNodes;

        public void LoadData(string path)
        { 
            NodesList = new List<Node>();
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
                         //   X = Convert.ToDouble(data[0].Replace('.',',')),
                         //   Y = Convert.ToDouble(data[1].Replace('.', ',')),
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

        public List<Node> GreedyRouteConstruction(double maxDistance)
        {
            double distance = 0;
            List<Node> route = new List<Node>();    //construct route
            List<Node> unvisited = NodesList;
            route.Add(NodesList.ElementAt(0));      //add first point to route
            Node currentNode;
            Node startNode = route.ElementAt(0);
            unvisited.Remove(startNode);
            currentNode = startNode;
            while (distance < maxDistance)
            {
                Node best = GetTheBestNode(currentNode,unvisited);
                if (CheckDistance(distance,maxDistance,currentNode,best,startNode))
                {
                    distance = distance + NodeDistances[currentNode.Position, best.Position];
                    route.Add(best);
                    unvisited.Remove(best);
                    currentNode = best;
                }
                else
                {
                    break;
                }
            }

            return route;

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

        private Node GetTheBestNode(Node current,List<Node> unvistedNodes)
        {
            Node best=new Node();
            double bestProfit = 0;
            foreach (var n in unvistedNodes)
            {
                if (current != n)
                {
                    if ((NodeDistances[current.Position, n.Position])/n.Profit > bestProfit)
                    {
                        best = n;
                    }
                }
            }

            return best;
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

        private void TwoOpt(int size, double currentDistance)
        {
            
            var iteration = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = i; j < size; j++)
                {
                    
                }
            }
            
        }

        public static int[] TwoOptSwap(int[] sequence, int i, int k)
        {
            int[] nextSequence = new int[sequence.Length];
            Array.Copy(sequence, nextSequence, sequence.Length);
            Array.Reverse(nextSequence, i, k - i + 1);

            return nextSequence;
        }
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
