using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphsClassProjectTakeTwo
{
    /// <summary>
    /// Partial class of the Graph - this file contains the Dijsktra's Algorithm methods
    /// Other files are: KruskalAlgorithm, ToplogicalSort, Graph, PrimAlgorithm, IsConnectedAlgorithm
    /// </summary>
    public partial class Graph
    {
        /// <summary>
        /// Dijkstra's Algorithm finds the shortest path between any two given nodes in a Graph
        /// </summary>
        /// <param name="source">Starting Vertex</param>
        /// <param name="target">Ending Vertex</param>
        /// <param name="shortestDistance">out param, the shortest distance between them</param>
        /// <returns>The shortest path (a List of Vertices)</returns>
        /// <exception cref="Exception">Forbidden algorithm attempt or source and target are the same</exception>
        public List<Vertex> Dijkstra(Vertex source, Vertex target, out double shortestDistance)
        { 

            // Has to be weighted (directed or undirected)
            if (!IsWeighted) throw new Exception("forbidden algorithm attempt");

            List<Vertex> path = new List<Vertex>();

            if (source.Equals(target))
            {
                throw new Exception("Source and target are the same. Shortest distance: 0.0");
            }

            shortestDistance = 0.0;

            Dictionary<Vertex, DijkstraStruct> vertexAndStruct = new Dictionary<Vertex, DijkstraStruct>();
            DijkstraStruct currNode = new DijkstraStruct(true, 0, source, source);

            vertexAndStruct.Add(source, currNode);

            while (currNode.Vertex != target)
            {
                var currentEdgesForUndirected = Edges.Where(e => e.Start.Equals(currNode.Vertex) || e.End.Equals(currNode.Vertex));

                var currentEdgesForDirected = Edges.Where(e => e.Start.Equals(currNode.Vertex));

                foreach (Edge edge in IsDirected ? currentEdgesForDirected : currentEdgesForUndirected)
                {
                    Vertex neighbor = edge.Start.Equals(currNode.Vertex) ? edge.End : edge.Start;
                    currNode = UpdateStructs(vertexAndStruct, currNode, out DijkstraStruct currStruct, out double newDistance, neighbor);

                }

                currNode = GetNewCurrNode(vertexAndStruct, currNode);
            }
            
            path = UpdatePath(path, source, vertexAndStruct, currNode);

            shortestDistance = currNode.DistanceFromStart;

            return path;
        }

        /// <summary>
        /// struct to keep track of shortest paths to each vertex
        /// </summary>
        struct DijkstraStruct
        {
            internal bool SdFound { get; set; }
            internal double DistanceFromStart { get; set; }
            internal Vertex Parent { get; set; }
            internal Vertex Vertex { get; set; }

            public DijkstraStruct(bool sdFound, double distanceFromStart, Vertex parent, Vertex vertex)
            {
                this.SdFound = sdFound;
                this.DistanceFromStart = distanceFromStart;
                this.Parent = parent;
                this.Vertex = vertex;
            }
        }

        /// <summary>
        /// Updates the DijkstraStructs with newly found paths
        /// </summary>
        /// <param name="vertexStructs">DijkstraStruct for each of the vertices found</param>
        /// <param name="currNode">The node currently being examined</param>
        /// <param name="currStruct">out param, DijkstraStruct made with current node</param>
        /// <param name="newDistance">out param, newest shortest distance</param>
        /// <param name="neighbor">The vertex next to current node whose path with current node is being explored</param>
        /// <returns>A DijkstraStruct</returns>
        private DijkstraStruct UpdateStructs(Dictionary<Vertex, DijkstraStruct> vertexStructs, DijkstraStruct currNode,
                                             out DijkstraStruct currStruct, out double newDistance, Vertex neighbor)
        {
            int maxVal = int.MaxValue;
            if (!vertexStructs.ContainsKey(neighbor))
            {
                DijkstraStruct newNode = new DijkstraStruct(false, maxVal, null, neighbor);
                vertexStructs.Add(neighbor, newNode);
            }

            currStruct = vertexStructs[neighbor];

            Edge edge = IsDirected ?
                Edges.Find(e => e.Start.Equals(currNode.Vertex) && e.End.Equals(neighbor)) :
                Edges.Find(e => (e.Start.Equals(currNode.Vertex) && e.End.Equals(neighbor)) ||
                                                       (e.Start.Equals(neighbor) && e.End.Equals(currNode.Vertex)));

            newDistance = vertexStructs[currNode.Vertex].DistanceFromStart + edge.Weight;

            if (newDistance < currStruct.DistanceFromStart)
            {
                //update parent and shortest dist of v
                currStruct.Parent = currNode.Vertex;
                currStruct.DistanceFromStart = newDistance;
                vertexStructs.Remove(neighbor);
                vertexStructs.Add(neighbor, currStruct);
            }

            return currNode;
        }

        /// <summary>
        /// Choose the next node to begin examining
        /// </summary>
        /// <param name="vertexStructs">DijkstraStruct for each of the vertices found</param>
        /// <param name="currNode">The previous node being examined</param>
        /// <returns>DijkstraStruct for the node now being examined</returns>
        /// <exception cref="Exception">No path exists</exception>
        private DijkstraStruct GetNewCurrNode(Dictionary<Vertex, DijkstraStruct> vertexStructs, DijkstraStruct currNode)
        {
            int maxVal = int.MaxValue;

            //find shortest false node and set to currNode and true
            double shortestFalse = maxVal;
            foreach (KeyValuePair<Vertex, DijkstraStruct> d in vertexStructs)
            {

                if (!d.Value.SdFound && d.Value.DistanceFromStart < shortestFalse)
                {
                    currNode = d.Value;
                    shortestFalse = d.Value.DistanceFromStart;
                }
            }

            if (shortestFalse == maxVal)
            {
                //all shortest paths have been found
                throw new Exception("No path exists");
            }

            currNode.SdFound = true;
            vertexStructs.Remove(currNode.Vertex);
            vertexStructs.Add(currNode.Vertex, currNode);
            return currNode;
        }

        /// <summary>
        /// Update the shortest path
        /// </summary>
        /// <param name="path">The unfinished shortest path</param>
        /// <param name="source">The node that we're backtracking towards</param>
        /// <param name="vertexStructs">DijkstraStruct for each of the vertices found</param>
        /// <param name="currNode">The vertex being added to the path</param>
        /// <returns></returns>
        private List<Vertex> UpdatePath(List<Vertex> path, Vertex source, Dictionary<Vertex, DijkstraStruct> vertexStructs, DijkstraStruct currNode)
        {
            Vertex parent = currNode.Parent;
            path.Add(parent);

            while (parent != source)
            {
                parent = vertexStructs[parent].Parent;
                path.Insert(0, parent);
            }

            path.Add(currNode.Vertex);

            return path;
        }
    }
}
