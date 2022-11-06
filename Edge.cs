namespace GraphsClassProjectTakeTwo
{
    public class Edge
    {
        public Vertex Start { get; set; }

        public Vertex End { get; set; }

        public double Weight { get; set; }

        public Edge(Vertex start, Vertex end, double weight)
        {
            this.Start = start;
            this.End = end;
            this.Weight = weight;
        }

    }
}
