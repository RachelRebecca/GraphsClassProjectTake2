using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphsClassProjectTakeTwo
{
    public class Edge
    {
        // fields
        public Vertex Start { get; set; }

        public Vertex End { get; set; }

        public double Weight { get; set; }

        // constructor
        public Edge(Vertex start, Vertex end, double weight)
        {
            this.Start = start;
            this.End = end;
            this.Weight = weight;
        }

    }
}
