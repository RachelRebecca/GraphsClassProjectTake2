namespace GraphsClassProjectTakeTwo
{
    public class Edge
    {
        // The initial Vertex in the pair of vertices
        public Vertex Start { get; set; }

        // The terminal Vertex in the pair of vertices
        public Vertex End { get; set; }

        // The weight of the Edge
        public double Weight { get; set; }

        public Edge(Vertex start, Vertex end, double weight)
        {
            this.Start = start;
            this.End = end;
            this.Weight = weight;
        }

    }
}
