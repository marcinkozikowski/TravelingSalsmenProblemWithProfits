using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        double[] dist;
        int[] prev;
        int[] visited;
        private Queue<int> distaneQueue;
        ArrayList[] list;

        public Dijkstry(ArrayList[] _list)
        {
            list = _list;
            int size = list.Length;
            dist = new double[size];
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
                foreach (NodeD n in list[smallestNode])
                {
                    if (dist[n.Index] > (dist[smallestNode] + getPathCost(smallestNode, n.Index)))
                    {
                        dist[n.Index] = dist[smallestNode] + getPathCost(smallestNode, n.Index);
                        prev[n.Index] = smallestNode;
                    }
                }
            }
            int[] tempPath = ReconstructPath(prev, SRC, DEST);
            if (tempPath != null)
            {
                return Path = ReconstructPath(prev, SRC, DEST);
            }
            else
            {
                return null;
            }
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
            double distance = INF;
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

        public double getPathDistance()
        {
            double distance = 0;
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

        private double getPathCost(int src, int dst)
        {
            if (src == dst)
            {
                return 0;
            }
            ArrayList nodes = list[src];
            foreach (NodeD n in nodes)
            {
                if (n.Index == dst)
                {
                    return n.Cost;
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
                if (prev[ret[currentNode]] < 0 || prev[ret[currentNode]]>prev.Length)
                {
                    return null;
                }
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

        public static ArrayList[] setIncidenceList(int cities,double[,] distances,List<Node> nodesList)
        {
            //TODO poprawic tworzenie listy incydencji 
            ArrayList[] list = new ArrayList[cities];
            for(int i=0;i<cities;i++)
            {
                list[i] = new ArrayList();
            }

            for(int i=0;i< cities;i++)
            {
                for(int j=0;j< cities;j++)
                {
                    if(distances[i,j]!=0)
                    {
                        list[i].Add(new NodeD(j,distances[i,j]));
                    }
                }
            }
            return list;
        }
    }
}
