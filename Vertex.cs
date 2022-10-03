using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphsClassProjectTakeTwo
{
    internal class Vertex
    {
        public String Name { get; set; }
        public List<Edge> Edges { get; set; }
        public int Indegree { get; set; }

        public Vertex(String nm)
        {
            Name = nm;
            Indegree = 0;
            Edges = new List<Edge>();
        }

        public void AddEdge(Vertex source, double weight)
        {
            Edges.Add(new Edge(source, this, weight)); // Each vertex knows the edges that are pointing to it
            this.Indegree++;
        }
    }
}
