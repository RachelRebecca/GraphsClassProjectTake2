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
        public Vertex Start { get; set; }

        public Vertex End { get; set; }

        public int Weight { get; set; }

        // constructor
        public Edge(Vertex start, Vertex end, int weight)
        {
            this.start = start;
            this.end = end;
            this.weight = weight;
        }

    }
}
