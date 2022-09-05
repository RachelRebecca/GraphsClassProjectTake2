using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphsClassProjectTakeTwo
{
    internal class Edge
    {
        // fields
        private Vertex start { get; set; }

        private Vertex end { get; set; }

        private int weight { get; set; }

        // constructor
        public Edge(Vertex start, Vertex end, int weight)
        {
            this.start = start;
            this.end = end;
            this.weight = weight;
        }

    }
}
