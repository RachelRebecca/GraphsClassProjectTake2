using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GraphsClassProjectTakeTwo
{
    internal class Graph
    {
        // fields
        private String Name { get; set; }

        private bool IsWeighted { get; set; }

        private bool IsDirected { get; set; }

        private List<Vertex> Vertices { get; set; }

        private List<Edge> Edges { get; set; } // TODO: only edge list?

        // constructor
        public Graph(String name, bool weighted, bool directed)
        {
            this.Name = name;
            this.IsWeighted = weighted;
            this.IsDirected = directed;
            this.Vertices = new List<Vertex>();
            this.Edges = new List<Edge>();
        }


        // methods
        public bool LoadGraph(SqlConnection sqlCon, String name) // TODO: convert to GraphType
        {
            bool retVal = true;
            try
            {
                SqlCommand getEdgesForGraph = new SqlCommand("spGetEdges", sqlCon);

                SqlParameter sqlParameter = new SqlParameter();
                sqlParameter.ParameterName = "@GraphName";
                sqlParameter.Value = name;
                getEdgesForGraph.Parameters.Add(sqlParameter);

                getEdgesForGraph.CommandType = CommandType.StoredProcedure;
                getEdgesForGraph.ExecuteNonQuery();
                SqlDataAdapter da2 = new SqlDataAdapter(getEdgesForGraph);
                DataSet dataSet = new DataSet();
                da2.Fill(dataSet, "Edges");
                // edge table: initialNode, terminalNode, weight (should be 1)

                var nrEdges = dataSet.Tables["Edges"].Rows.Count;
                for (int row = 0; row < nrEdges; ++row)
                {
                    // check initial node
                    String initialNode = (String)dataSet.Tables["Edges"].Rows[row].ItemArray[0];
                    String terminalNode = (String)dataSet.Tables["Edges"].Rows[row].ItemArray[1];

                    int initialIndex = Vertices.FindIndex(item => initialNode.Equals(item.Name));
                    int terminalIndex = Vertices.FindIndex(item => terminalNode.Equals(item.Name));

                    Vertex initial = initialIndex < 0 ? new Vertex(initialNode)
                                                    : Vertices[initialIndex];
                    Vertex terminal = terminalIndex < 0 ? new Vertex(terminalNode)
                                                    : Vertices[terminalIndex];

                    if (initialIndex < 0 && terminalIndex < 0)
                    {
                        // neither exist - create both, add edge between them with weight = 1
                        Vertices.Add(initial);
                        Vertices.Add(terminal);
                    }
                    else if (initialIndex < 0 && terminalIndex > -1)
                    {
                        // initial doesn't exist, create and add edge between it and terminal with weight = 1
                        Vertices.Add(initial);
                    }
                    else if (initialIndex > -1 && terminalIndex < 0)
                    {
                        // terminal doesn't exist, create and add edge between initial and it with weight = 1
                        Vertices.Add(terminal);
                    }
                    // if they both already exist, no need to add anything
                    initial.AddEdge(terminal, 1);
                    terminal.AddEdge(initial, 1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException());
                Console.WriteLine(e.StackTrace);
                retVal = false;
            }

            return retVal;
        }

        // todo: add an out parameter to return the sum of the weights of the total path
        public List<Vertex> Dijkstra()
        {
            return new List<Vertex>();
        }

        public List<Edge> Kruskal()
        {
            List<Edge> shortestPath = new List<Edge>;
            List<Edge> orderedEdges = this.Edges.Sort((x, y) => x.Weight - y.Weight);

            List<List<Vertex>> visited = new List<List<Vertex>>();

            while (shortestPath.Count < this.Vertices.Count - 1)
            {
                Edge shortest = orderedEdges.ElementAt(0);
                int foundSoureWhere = -1;
                int foundDestinationWhere = -1;

                //check if will create cycle
                foreach (List<Vertex> connectedVertices in visited)
                {
                    foreach (Vertex currVertex in connectedVertices)
                    {
                        if (currVertex == shortest.source)
                        {
                            foundSourceWhere = visited.IndexOf(connectedVertices);
                        }
                        else if (currVertex == shortest.Destination)
                        {
                            foundDestinationWhere = visited.IndexOf(connectedVertices);
                        }
                    }
                }

                if (foundSourceWhere == -1)
                {
                    if (foundDestinationWhere == -1)
                    {
                        List<Vertex> unconnected = new List<Vertex>();
                        unconnected.Add(shortest.source);
                        unconnected.Add(shortest.Destination);
                        visited.Add(unconnected);
                        shortestPath.Add(shortest);
                    }
                    else
                    {
                        visited[foundDestinationWhere].Add(shortest.source);
                        shortestPath.Add(shortest);
                    }
                }
                else
                {
                    if (foundDestinationWhere == -1)
                    {
                        visited[foundSourceWhere].Add(shortest.Destination);
                        shortestPath.Add(shortest);
                    }
                    else if (foundDestinationWhere != foundSourceWhere)
                    {
                        foreach (Vertex movingVertex in visited[foundDestinationWhere])
                        {
                            visited[foundSourceWhere].Add(movingVertex);
                        }

                        visited.RemoveAt(foundDestinationWhere);
                        shortestPath.Add(shortest);
                    }
                }

                orderedEdges.RemoveAt(0);
            }

            return shortestPath;

        }

        public List<Edge> Prim()
        {
            return new List<Edge>();

        }

        public List<Vertex> TopologicalSort()
        {
            // backup in case the button isn't properly disabled
            if (!this.IsDirected) throw new Exception('forbidden algorithm attempt');

            List<Vertex> sorted = new List<Vertex>();

            // make adjacency list (use each vertex's neighbors list) - dictionary
            Dictionary<Vertex, List<Vertex>> adjacencyList = new Dictionary<Vertex, List<Vertex>>();

            // make indegree list (use each vertex's indegree) - dictionary
            Dictionary<Vertex, int> indegreeList = new Dictionary<Vertex, int>();

            foreach (Vertex vertex in this.Vertices)
            {
                foreach (Edge edge in vertex.edges)
                {
                    if (edge.Start.Equals(vertex)
                    {
                        adjacencyList.Add(vertex, edge.End)
                    }
                }
                indegreeList.Add(vertex, vertex.Indegree);
            }

            // make 0s queue
            Queue<Vertex> zeroes = new Queue<Vertex>();

            // enqueue all 0 indegrees
            foreach (KeyValuePair<Vertex, int> entry in indegreeList)
            {
                if (entry.Value == 0)
                {
                    zeroes.Enqueue(entry.Key);
                }
            }

            if (zeroes.Count == 0)
            {
                throw new Exception("Graph contains cycle");
            }

            int numVerticesAdded = 0;

            while (zeroes.Count > 0)
            {
                // dequeue Vertex v, add to sorted
                Vertex v = zeroes.Dequeue();
                sorted.Add(v);
                numVerticesAdded++;

                // for all vertices in v's adjacency list, indegree--. if new indegree == 0, enqueue
                List<Vertex> verticesToSubtract = adjacencyList[v];
                foreach (Vertex neighbor in verticesToSubtract)
                {
                    indegreeList[neighbor]--;
                    if (indegreeList[neighbor] == 0)
                    {
                        zeroes.Enqueue(neighbor);
                    }
                }
                
                if (numVerticesAdded > graph.Vertices.Count)
                {
                    throw new Exception("Graph contains cycle");
                }
            }

            return sorted;
        }
    }
}
