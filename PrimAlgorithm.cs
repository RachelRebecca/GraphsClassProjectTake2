using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphsClassProjectTakeTwo
{
    /// <summary>
    /// Partial class of the Graph - this file contains the Prim Algorithm methods
    /// Other files are: DijkstraAlgorithm, ToplogicalSort, Graph, KruskalAlgorithm, IsConnectedAlgorithm
    /// </summary>
    public partial class Graph
    {
        /// <summary>
        /// Does Prim's Minimum Spanning Tree algorithm from a starting Vertex
        /// </summary>
        /// <param name="start">The starting Vertex</param>
        /// <returns>List of Edges in a MST</returns>
        /// <exception cref="Exception">Forbidden algorithm attempt</exception>
        public List<Edge> Prim(Vertex start)
        {
            // Has to be a graph (weighted or unweighted)
            if (IsDirected) throw new Exception("forbidden algorithm attempt");

            Edge[] edges = new Edge[Vertices.Count - 1];
            List<PrimStruct> prims = new List<PrimStruct>();
            List<Vertex> foundVertices = new List<Vertex>();
            int numEdgesFound = 0;

            foundVertices.Add(start);

            var startEdges = Edges.Where(e => e.Start.Equals(start) || e.End.Equals(start));

            // add prims for all neighbors of start
            foreach (Edge edge in startEdges)
            {
                Vertex neighbor = edge.Start.Equals(start) ? edge.End : edge.Start;
                if (!foundVertices.Contains(neighbor))
                {
                    prims.Add(new PrimStruct(neighbor, edge.Weight, start));
                }
            }

            while (numEdgesFound < Vertices.Count - 1)
            {

                // get the vertex with the shortest cost
                prims.Sort((x, y) => x.Cost.CompareTo(y.Cost));
                PrimStruct currentPrim = prims[0];
                prims.RemoveAt(0);

                // add an edge to that prim
                edges[numEdgesFound] = Edges.Find(e => (e.Start.Equals(currentPrim.Parent) && e.End.Equals(currentPrim.Node)) ||
                                                       (e.Start.Equals(currentPrim.Node) && e.End.Equals(currentPrim.Parent)));

                foundVertices.Add(currentPrim.Node);
                numEdgesFound++;


                var currentEdges = Edges.Where(e => e.Start.Equals(currentPrim.Node) || e.End.Equals(currentPrim.Node));


                foreach (Edge edge in currentEdges)
                {
                    Vertex neighbor = edge.Start.Equals(currentPrim.Node) ? edge.End : edge.Start;
                    if (!foundVertices.Contains(neighbor))
                    {
                        PrimStruct neighborPrim = prims.Find(p => p.Node.Equals(neighbor));
                        if (neighborPrim.Node != null)
                        {
                            if (edge.Weight < neighborPrim.Cost)
                            {
                                neighborPrim.Cost = edge.Weight;
                                neighborPrim.Parent = currentPrim.Node;
                            }
                        }
                        else
                        {
                            prims.Add(new PrimStruct(neighbor, edge.Weight, currentPrim.Node));
                        }
                    }

                }
            }

            return edges.ToList();

        }

        /// <summary>
        /// struct for a Prim - for each Node, gives the cost to get to it from a given parent
        /// </summary>
        struct PrimStruct
        {
            public PrimStruct(Vertex node, double cost, Vertex parent)
            {
                this.Node = node;
                this.Cost = cost;
                this.Parent = parent;
            }

            internal Vertex Node;
            internal double Cost { get; set; }
            internal Vertex Parent { get; set; }
        }

    }
}
