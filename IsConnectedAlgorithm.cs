using System.Collections.Generic;
using System.Linq;

namespace GraphsClassProjectTakeTwo
{
    public partial class Graph
    {
        public enum Direction { DIRECTION_1, DIRECTION_2 }
        public bool IsConnected()
        {
            bool IsConnected = false;

            if (Vertices.Count > 0)
            {
                IsConnected = IsConnectedGraph();
            }

            return IsConnected;
        }

        public bool IsStronglyConnectedDigraph()
        {
            bool IsConnected = true;
            bool[] InDirection1 = GetIndirection(Edges, Direction.DIRECTION_1);
            bool[] InDirection2 = GetIndirection(Edges, Direction.DIRECTION_2);

            for (int i = 0; i < Vertices.Count; i++)
            {
                if (!InDirection1[i] && !InDirection2[i])
                {
                    IsConnected = false;
                    break;
                }
            }
            return IsConnected;
        }
        public bool IsConnectedGraph()
        {
            List<Edge> edges = new List<Edge>();
            foreach (Edge edge in Edges)
            {
                edges.Add(edge);
                edges.Add(new Edge(edge.End, edge.Start, edge.Weight));
            }

            bool[] InDirection1 = GetIndirection(edges, Direction.DIRECTION_1);
            bool[] InDirection2 = GetIndirection(edges, Direction.DIRECTION_2);

            return !(InDirection1.Contains(false) || InDirection2.Contains(false));
        }

        public bool[] GetIndirection(List<Edge> edges, Direction direction)
        {
            bool[] InDirection = new bool[Vertices.Count];

            Queue<Vertex> Neighbors = new Queue<Vertex>();
            Vertex currentVertex = Vertices[0];
            InDirection[0] = true;

            var startEdges = direction == Direction.DIRECTION_1
                ? edges.Where(e => e.End.Equals(currentVertex))
                : edges.Where(e => e.Start.Equals(currentVertex));
            foreach (Edge edge in startEdges)
            {
                Neighbors.Enqueue(direction == Direction.DIRECTION_1 ? edge.Start : edge.End);
            }

            while (InDirection.Contains(false))
            {
                if (Neighbors.Count == 0)
                {
                    break;
                }
                currentVertex = Neighbors.Dequeue();
                if (InDirection[Vertices.IndexOf(currentVertex)])
                {
                    continue;
                }
                else
                {
                    InDirection[Vertices.IndexOf(currentVertex)] = true;
                    startEdges = direction == Direction.DIRECTION_1
                        ? edges.Where(e => e.End.Equals(currentVertex))
                        : edges.Where(e => e.Start.Equals(currentVertex));
                    foreach (Edge edge in startEdges)
                    {
                        Neighbors.Enqueue(direction == Direction.DIRECTION_1 ? edge.Start : edge.End);
                    }
                }

            }
            return InDirection;
        }
    }
}
