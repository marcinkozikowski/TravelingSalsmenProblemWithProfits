using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorytmika
{
    class Dijkstry
    {
        public const int NONODESBEFORE = -1;
        public const int NOVISITED = -2;
        public const int VISITED = 1;
        public const int INF = 1000000;
        public long[,] Graph { get; set; }
        private int[] Path;
        int[] dist;
        int[] prev;
        int[] visited;
        private Queue<int> distaneQueue;
        ArrayList[] list;

        public Dijkstry(ArrayList[] _list)
        {
            list = _list;
            int size = list.Length;
            dist = new int[size];
            prev = new int[size];
            visited = new int[size];
        }

        public Dijkstry(long[,] graph)
        {
            Graph = graph;
            long size = graph.Length;
            dist = new int[size];
            prev = new int[size];
            visited = new int[size];
        }

        public int[] GetPath(int SRC, int DEST)
        {
            int graphSize = list.Count();
            int smallestNode = 0;

            for (int i = 0; i < graphSize; i++)     //Inicjalizacja pustych tabele odleglosci, poprzednikow oraz nazw wierzcholkow
            {
                dist[i] = INF;
                prev[i] = NONODESBEFORE;
                visited[i] = NOVISITED;
            }

            dist[SRC] = 0;                          //odleglosc od punktu poczatkowego = 0
            while (isAnyUnvisited())
            {
                smallestNode = getSmallestNodeNoVisited();
                if (smallestNode == INF || smallestNode == DEST)
                {
                    break;
                }
                visited[smallestNode] = VISITED;
                foreach (Node n in list[smallestNode])
                {
                    if (dist[n.Position] > (dist[smallestNode] + getPathCost(smallestNode, n.Position)))
                    {
                        dist[n.Position] = dist[smallestNode] + getPathCost(smallestNode, n.Position);
                        prev[n.Position] = smallestNode;
                    }
                }
            }
            Path = ReconstructPath(prev, SRC, DEST);
            return Path;
        }

        public int[] GetPathMatrix(int SRC, int DEST)
        {
            int graphSize = Graph.GetLength(0);
            int[] nodes = new int[dist.Length];

            for (int i = 0; i < dist.Length; i++)
            {
                dist[i] = prev[i] = INF;
                nodes[i] = i;
            }

            dist[SRC] = 0;
            do
            {
                int smallest = nodes[0];
                int smallestIndex = 0;
                for (int i = 1; i < graphSize; i++)
                {
                    if (dist[nodes[i]] < dist[smallest])
                    {
                        smallest = nodes[i];
                        smallestIndex = i;
                    }
                }
                graphSize--;
                nodes[smallestIndex] = nodes[graphSize];

                if (dist[smallest] == INF || smallest == DEST)
                    break;

                for (int i = 0; i < graphSize; i++)
                {
                    int v = nodes[i];
                    int newDist = dist[smallest] + Convert.ToInt32(Graph[smallest, v]);
                    if (newDist < dist[v])
                    {
                        dist[v] = newDist;
                        prev[v] = smallest;
                    }
                }
            } while (graphSize > 0);
            return ReconstructPath(prev, SRC, DEST);
        }

        private bool isAnyUnvisited()
        {
            for (int i = 0; i < visited.Count(); i++)
            {
                if (visited[i] == NOVISITED)
                {
                    return true;
                }
            }
            return false;
        }

        private int getSmallestNodeNoVisited()
        {
            int node = INF;
            int distance = INF;
            for (int i = 0; i < dist.Length; i++)
            {
                if (visited[i] == NOVISITED && dist[i] <= distance)
                {
                    distance = dist[i];
                    node = i;
                }
            }
            return node;
        }

        public int getPathDistance()
        {
            int distance = 0;
            int currentNode = 0;
            int nextNode = 0;
            distaneQueue = new Queue<int>();
            for (int i = 0; i < Path.Length; i++)      //add all path nodes to queue
            {
                distaneQueue.Enqueue(Path[i]);
            }
            currentNode = distaneQueue.Dequeue();
            while (distaneQueue.Count() > 0)
            {
                nextNode = distaneQueue.Dequeue();
                distance = distance + getPathCost(currentNode, nextNode);
                currentNode = nextNode;
            }
            return distance;
        }

        private int getPathCost(int src, int dst)
        {
            if (src == dst)
            {
                return 0;
            }
            ArrayList nodes = list[src];
            foreach (Node n in nodes)
            {
                if (n.Position == dst)
                {
                    return n.Position;
                }
            }
            return INF;
        }

        public int[] ReconstructPath(int[] prev, int SRC, int DEST)
        {
            int[] ret = new int[prev.Length];
            int currentNode = 0;
            ret[currentNode] = DEST;
            while (ret[currentNode] != INF && ret[currentNode] != SRC)
            {
                ret[currentNode + 1] = prev[ret[currentNode]];
                currentNode++;
            }
            if (ret[currentNode] != SRC)
                return null;
            int[] reversed = new int[currentNode + 1];
            for (int i = currentNode; i >= 0; i--)
                reversed[currentNode - i] = ret[i];
            return reversed;
        }

        public ArrayList[] setIncidenceList(int citiesNumber, int pathsCount, ArrayList[] list,double[,] distances)
        {
            //TODO poprawic tworzenie listy incydencji 
            string[] lineSplit;
            int source = 0;
            int dest = 0;
            int count = 0;
            double pathLength = 0;

            //for (int i = 0; i < pathsCount; i++)
            //{
            //    lineSplit = fileStream.ReadLine().Split(' ');
            //    source = int.Parse(lineSplit[0]);
            //    dest = int.Parse(lineSplit[1]);
            //    pathLength = int.Parse(lineSplit[2]);
            //    Node n = new Node((dest), pathLength);
            //    Node n1 = new Graph.Node((source), pathLength);
            //    if (pathLength < 0)
            //    {
            //        throw new Exception("Distance can`t be less than 0");
            //    }
            //    else
            //    {
            //        list[source].Add(n);
            //        list[dest].Add(n1);
            //        count++;
            //    }
            //}
            return list;
        }
    }
}
