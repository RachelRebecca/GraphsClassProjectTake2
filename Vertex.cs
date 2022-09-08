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
        public List<Edge> Edges { get; set; } // THE CHANGE 
        public int Indegree { get; set; }

        public Vertex(String nm)
        {
            Name = nm;
            Indegree = 0;
            edges = new List<Edge>();
        }

        public void AddEdge(Vertex source, int weight)
        {
            edges.Add(new Edge(source, this, weight));
            this.Indegree++;
        }
    }
}
