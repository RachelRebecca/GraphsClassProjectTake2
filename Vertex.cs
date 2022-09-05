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
        public List<Edge> edges { get; set; } // THE CHANGE 
        public int Indegree { get; set; }

        public Vertex(String nm)
        {
            Name = nm;
            Indegree = 0;
            edges = new List<Edge>();
        }

        public void AddEdge(Vertex target, int weight)
        {
            // TODO: Target, this? or this, target?
            edges.Add(new Edge(target, this, weight));
            target.Indegree++;
        }
    }
}
