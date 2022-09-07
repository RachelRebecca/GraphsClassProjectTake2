using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphsClassProjectTakeTwo
{
    internal class Graph
    {
        // fields
        private String name { get; set; }

        private bool IsWeighted { get; set; }

        private bool IsDirected { get; set; }

        private List<Vertex> vertices { get; set; }

        private List<Edge> edges { get; set; } // TODO: only edge list?

        // constructor
        public Graph(String name, bool weighted, bool directed)
        {
            this.name = name;
            this.weighted = weighted;
            this.directed = directed;
            this.vertices = new List<Vertex>();
            this.edges = new List<Edge>();
        }


        // methods
        public bool loadGraph()
        {
            return false;
        }

        // todo: add an out parameter to return the sum of the weights of the total path
        public List<Vertex> Dijkstra()
        {
            return new List<Vertex>();
        }

        public List<Edge> Kruskal()
        {
            return new List<Edge>();

        }

        public List<Edge> Prim()
        {
            return new List<Edge>();

        }

        public List<Vertex> TopologicalSort()
        {
            // backup in case the button isn't properly disabled
            if (!this.IsDirected) throw new Exception('forbidden algorithm attempt');

            List<Vertex> sorted = new List<Vertex>();

            // make adjacency list (use each vertex's neighbors list) - dictionary
            Dictionary<Vertex, List<Vertex>> adjacencyList = new Dictionary<Vertex, List<Vertex>>();

            // make indegree list (use each vertex's indegree) - dictionary
            Dictionary<Vertex, int> indegreeList = new Dictionary<Vertex, int>();

            foreach (Vertex vertex in this.Vertices)
            {
                foreach (Edge edge in vertex.edges)
                {
                    if (edge.Start.Equals(vertex)
                    {
                        adjacencyList.Add(vertex, edge.End)
                    }
                }
                indegreeList.Add(vertex, vertex.Indegree);
            }

            // make 0s queue
            Queue<Vertex> zeroes = new Queue<Vertex>();

            // enqueue all 0 indegrees
            foreach (KeyValuePair<Vertex, int> entry in indegreeList)
            {
                if (entry.Value == 0)
                {
                    zeroes.Enqueue(entry.Key);
                }
            }

            if (zeroes.Count == 0)
            {
                throw new Exception("Graph contains cycle");
            }

            int numVerticesAdded = 0;

            while (zeroes.Count > 0)
            {
                // dequeue Vertex v, add to sorted
                Vertex v = zeroes.Dequeue();
                sorted.Add(v);
                numVerticesAdded++;

                // for all vertices in v's adjacency list, indegree--. if new indegree == 0, enqueue
                List<Vertex> verticesToSubtract = adjacencyList[v];
                foreach (Vertex neighbor in verticesToSubtract)
                {
                    indegreeList[neighbor]--;
                    if (indegreeList[neighbor] == 0)
                    {
                        zeroes.Enqueue(neighbor);
                    }
                }
                
                if (numVerticesAdded > graph.Vertices.Count)
                {
                    throw new Exception("Graph contains cycle");
                }
            }

            return sorted;
        }
    }
}
