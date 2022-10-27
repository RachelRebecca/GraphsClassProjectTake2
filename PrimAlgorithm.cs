using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace GraphsClassProjectTakeTwo
{
    public partial class Graph
    {
        public List<Edge> Prim(Vertex start)
        {
            // Has to be a graph (weighted or unweighted)
            if (this.IsDirected) throw new Exception("forbidden algorithm attempt");

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
                edges[numEdgesFound] = Edges.Find(e => (e.Start.Equals(currentPrim.Parent) && e.End.Equals(currentPrim.vertex)) ||
                                                       (e.Start.Equals(currentPrim.vertex) && e.End.Equals(currentPrim.Parent)));

                foundVertices.Add(currentPrim.vertex);
                numEdgesFound++;


                var currentEdges = Edges.Where(e => (e.Start.Equals(currentPrim.vertex) || e.End.Equals(currentPrim.vertex)));


                foreach (Edge edge in currentEdges)
                {
                    Vertex neighbor = edge.Start.Equals(currentPrim.vertex) ? edge.End : edge.Start;
                    if (!foundVertices.Contains(neighbor))
                    {
                        PrimStruct neighborPrim = prims.Find(p => p.vertex.Equals(neighbor));
                        if (neighborPrim.vertex != null)
                        {
                            if (edge.Weight < neighborPrim.Cost)
                            {
                                neighborPrim.Cost = edge.Weight;
                                neighborPrim.Parent = currentPrim.vertex;
                            }
                        }
                        else
                        {
                            prims.Add(new PrimStruct(neighbor, edge.Weight, currentPrim.vertex));
                        }
                    }

                }
            }

            return edges.ToList();

        }

        struct PrimStruct
        {
            public PrimStruct(Vertex vertex, double cost, Vertex parent)
            {
                this.vertex = vertex;
                this.Cost = cost;
                this.Parent = parent;
            }

            internal Vertex vertex;
            internal double Cost { get; set; }
            internal Vertex Parent { get; set; }
        }

    }
}
