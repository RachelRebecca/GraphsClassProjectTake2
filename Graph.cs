using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphsClassProjectTakeTwo
{
    internal class Graph
    {
        // fields
        private String name { get; set; }

        private bool weighted { get; set; }

        private bool directed { get; set; }

        private List<Vertex> vertices { get; set; }

        private List<Edge> edges { get; set; } // TODO: only edge list?

        // constructor
        public Graph(String name, bool weighted, bool directed)
        {
            this.name = name;
            this.weighted = weighted;
            this.directed = directed;
            this.vertices = new List<Vertex>();
            this.edges = new List<Edge>();
        }


        // methods
        public bool loadGraph()
        {
            return false;
        }

        public List<Vertex> Dijkstra()
        {
            return new List<Vertex>();
        }

        public List<Edge> Kruskal()
        {
            return new List<Edge>();

        }

        public List<Edge> Prim()
        {
            return new List<Edge>();

        }

        public List<Vertex> TopologicalSort()
        {
            return new List<Vertex>();
        }
    }
}
