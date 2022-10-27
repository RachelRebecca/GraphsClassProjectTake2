using System;
using System.Collections.Generic;

namespace GraphsClassProjectTakeTwo
{
    public class Vertex
    {
        public String Name { get; set; }
        public List<Edge> Edges { get; set; }
        public int Indegree { get; set; }

        public int Outdegree { get; set; }
        public double XCoord { get; set; }
        public double YCoord { get; set; }

        public Vertex(String nm, double xCoord, double yCoord)
        {
            Name = nm;
            Indegree = 0;
            Outdegree = 0;
            Edges = new List<Edge>();
            XCoord = xCoord;
            YCoord = yCoord;
        }

        public void AddEdge(Vertex source, double weight)
        {
            Edges.Add(new Edge(source, this, weight)); // Each vertex knows the edges that are pointing to it
            source.Outdegree++;
            this.Indegree++;
        }
    }
}
