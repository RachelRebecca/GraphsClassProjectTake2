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
        public List<Vertex> Dijkstra(Vertex source, Vertex target, out double shortestDistance)
        {

            // Has to be weighted (directed or undirected)
            if (!this.IsWeighted) throw new Exception("forbidden algorithm attempt");

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
                foreach (Edge edge in currNode.Vertex.Edges)
                {
                    // TODO: refactor, this is used in Prim
                    Vertex neighbor = edge.Start.Equals(currNode.Vertex) ? edge.End : edge.Start;
                    currNode = UpdateStructs(vertexAndStruct, currNode, out DijkstraStruct currStruct, out double newDistance, neighbor);

                }

                currNode = GetNewCurrNode(vertexAndStruct, currNode);

                path = UpdatePath(path, source, vertexAndStruct, currNode);

                shortestDistance = currNode.DistanceFromStart;

            }

            return path;
        }

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

            Edge edge = Edges.Find(e => (e.Start.Equals(currNode.Vertex) && e.End.Equals(neighbor)) ||
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
