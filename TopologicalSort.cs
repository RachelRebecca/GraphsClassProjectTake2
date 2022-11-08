using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphsClassProjectTakeTwo
{
    /// <summary>
    /// Partial class of the Graph - this file contains the Topological Sort methods
    /// Other files are: DijkstraAlgorithm, KruskalAlgorithm, Graph, PrimAlgorithm, IsConnectedAlgorithm
    /// </summary>
    public partial class Graph
    {
        /// <summary>
        /// Topological Sort sorts the nodes in a directed graph 
        /// such that every node is behind every node that it is pointing to
        /// </summary>
        /// <returns>A List of the sorted Vertices in the Graph</returns>
        /// <exception cref="Exception">Forbidden algorithm attempt, or Graph contains cycle</exception>
        public List<Vertex> TopologicalSort()
        {
            // Has to be a digraph (weighted or unweighted)
            if (!IsDirected) throw new Exception("forbidden algorithm attempt");

            List<Vertex> sorted = new List<Vertex>();
            Dictionary<Vertex, List<Vertex>> adjacencyList;
            Dictionary<Vertex, int> indegreeList;
            SetUpLists(out adjacencyList, out indegreeList);

            // make 0s queue
            Queue<Vertex> zeroes = new Queue<Vertex>();

            FillZeroesQueue(indegreeList, zeroes);

            if (zeroes.Count == 0)
            {
                throw new Exception("Graph contains cycle");
            }

            int numVerticesAdded = 0;

            while (zeroes.Count > 0)
            {
                // dequeue Vertex v, add to sorted
                Vertex nextVertex = zeroes.Dequeue();

                sorted.Add(nextVertex);
                numVerticesAdded++;

                CheckAllNeighbors(adjacencyList, indegreeList, zeroes, nextVertex); 

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

        /// <summary>
        /// Sets up lists 
        /// </summary>
        /// <param name="adjacencyList">The Vertices pointing at any given Vertex</param>
        /// <param name="indegreeList">The number of unfound (unsorted) Vertices pointing at any given Vertex</param>
        private void SetUpLists(out Dictionary<Vertex, List<Vertex>> adjacencyList, out Dictionary<Vertex, int> indegreeList)
        {
            // make adjacency list (use each vertex's neighbors list) - dictionary
            adjacencyList = new Dictionary<Vertex, List<Vertex>>();

            // make indegree list (use each vertex's indegree) - dictionary
            indegreeList = new Dictionary<Vertex, int>();
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
        }

        /// <summary>
        /// Enqueue all Vertices with an indegree of 0
        /// </summary>
        /// <param name="indegreeList">The number of unfound Vertices pointing at any given Vertex</param>
        /// <param name="zeroes">The Queue of all Vertices with no unfound Vertices pointing at them</param>
        private static void FillZeroesQueue(Dictionary<Vertex, int> indegreeList, Queue<Vertex> zeroes)
        {
            foreach (KeyValuePair<Vertex, int> entry in indegreeList)
            {
                if (entry.Value == 0)
                {
                    zeroes.Enqueue(entry.Key);
                }
            }
        }

        /// <summary>
        /// For all vertices in the specified Vertex's adjacency list, indegree--. If new indegree == 0, enqueue
        /// </summary>
        /// <param name="adjacencyList">The Vertices pointing at any given Vertex</param>
        /// <param name="indegreeList">The number of unfound Vertices pointing at any given Vertex</param>
        /// <param name="zeroes">The Queue of all Vertices with no unfound Vertices pointing at them</param>
        /// <param name="vertex">The specified Vertex</param>
        private static void CheckAllNeighbors(Dictionary<Vertex, List<Vertex>> adjacencyList, Dictionary<Vertex, int> indegreeList,
                                              Queue<Vertex> zeroes, Vertex vertex)
        {
            List<Vertex> verticesToSubtract = adjacencyList[vertex];
            foreach (Vertex neighbor in verticesToSubtract)
            {
                indegreeList[neighbor]--;
                if (indegreeList[neighbor] == 0)
                {
                    zeroes.Enqueue(neighbor);
                }
            }
        }
    }
}
