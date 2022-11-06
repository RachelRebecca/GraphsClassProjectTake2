using System.Collections.Generic;

namespace GraphsClassProjectTakeTwo
{
    /// <summary>
    /// Represents a Vertex
    ///     - Contains basic information about Vertex 
    ///     - Contains an Edges List of all edges pointing to this Vertex
    /// </summary>
    public class Vertex
    {
        // Name of Vertex
        public string Name { get; set; }

        // List of Edges that are pointing to the Vertex 
        public List<Edge> Edges { get; set; }

        // Indegree of Vertex
        public int Indegree { get; set; }

        // Outdegree of Vertex
        public int Outdegree { get; set; }

        // X coordinate of the Vertex (relative position)
        public double XCoord { get; set; }

        // Y coordinate of the Vertex (relative position)
        public double YCoord { get; set; }

        public Vertex(string name, double xCoord, double yCoord)
        {
            Name = name;
            Indegree = 0;
            Outdegree = 0;
            Edges = new List<Edge>();
            XCoord = xCoord;
            YCoord = yCoord;
        }

        /// <summary>
        /// Adds an edge to the Edges list 
        /// Each Vertex knows the edges that are pointing to it
        /// </summary>
        /// <param name="source">The starting Vertex that is pointing to this Vertex in the Edge</param>
        /// <param name="weight">Weight of edge</param>
        public void AddEdge(Vertex source, double weight)
        {
            Edges.Add(new Edge(source, this, weight));
            source.Outdegree++;
            this.Indegree++;
        }
    }
}
