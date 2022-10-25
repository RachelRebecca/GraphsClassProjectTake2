using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphsClassProjectTakeTwo
{
    public partial class Graph
    {
        public List<Vertex> TopologicalSort()
        {
            // Has to be a digraph (weighted or unweighted)
            if (!this.IsDirected) throw new Exception("forbidden algorithm attempt");

            List<Vertex> sorted = new List<Vertex>();

            // make adjacency list (use each vertex's neighbors list) - dictionary
            Dictionary<Vertex, List<Vertex>> adjacencyList = new Dictionary<Vertex, List<Vertex>>();

            // make indegree list (use each vertex's indegree) - dictionary
            Dictionary<Vertex, int> indegreeList = new Dictionary<Vertex, int>();

            foreach (Vertex vertex in Vertices)
            {
                var edgesForVertex = Edges.Where(e => e.Start.Equals(vertex));
                List<Vertex> neighbors = new List<Vertex>();
                foreach (Edge edge in edgesForVertex)
                {
                    if (!neighbors.Contains(edge.End))
                    {
                        neighbors.Add(edge.End);
                    }
                }
                adjacencyList.Add(vertex, neighbors);
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

                if (numVerticesAdded > Vertices.Count)
                {
                    throw new Exception("Graph contains cycle");
                }
            }

            if (numVerticesAdded != Vertices.Count)
            {
                throw new Exception("Graph contains cycle");
            }
            return sorted;
        }

    }
}
