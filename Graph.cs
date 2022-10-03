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
        public Graph(String name, SqlConnection sqlCon)
        {
            this.Name = name;
            this.Vertices = new List<Vertex>();
            this.Edges = new List<Edge>();
            LoadGraph(sqlCon);
        }


        // methods
        public bool LoadGraph(SqlConnection sqlCon) // TODO: convert to GraphType
        {
            bool retVal = true;
            try
            {
                //get graph type
                SqlCommand getGraphType = new SqlCommand("spGetType", sqlCon);
                SqlCommand getEdgesForGraph = new SqlCommand("spGetEdges", sqlCon);

                SqlParameter sqlParameter = new SqlParameter();
                sqlParameter.ParameterName = "@GraphName";
                sqlParameter.Value = this.Name;
                getGraphType.Parameters.Add(sqlParameter);
                getEdgesForGraph.Parameters.Add(sqlParameter);

                getGraphType.CommandType = CommandType.StoredProcedure;
                getGraphType.ExecuteNonQuery();
                SqlDataAdapter da1 = new SqlDataAdapter(getGraphType);
                DataSet dataSet1 = new DataSet();
                da1.Fill(dataSet1, "Flags");

                // if it's "1", then it's true otherwise it's false
                this.IsWeighted = (String)dataSet1.Tables["Flags"].Rows[0].ItemArray[0] == "1";
                this.IsDirected = (String)dataSet1.Tables["Flags"].Rows[0].ItemArray[1] == "1"; 

                getEdgesForGraph.CommandType = CommandType.StoredProcedure;
                getEdgesForGraph.ExecuteNonQuery();
                SqlDataAdapter da2 = new SqlDataAdapter(getEdgesForGraph);
                DataSet dataSet2 = new DataSet();
                da2.Fill(dataSet2, "Edges");
                // edge table: initialNode, terminalNode, weight (should be 1)

                var nrEdges = dataSet2.Tables["Edges"].Rows.Count;

                for (int row = 0; row < nrEdges; ++row)
                {
                    String initialNode = (String)dataSet2.Tables["Edges"].Rows[row].ItemArray[0];
                    String initialNodeX = (String)dataSet2.Tables["Edges"].Rows[row].ItemArray[1];
                    String initialNodeY = (String)dataSet2.Tables["Edges"].Rows[row].ItemArray[2];
                    String terminalNode = (String)dataSet2.Tables["Edges"].Rows[row].ItemArray[3];                
                    String terminalNodeX = (String)dataSet2.Tables["Edges"].Rows[row].ItemArray[4];              
                    String terminalNodeY = (String)dataSet2.Tables["Edges"].Rows[row].ItemArray[5];

                    // TODO: store initialNodeX, initialNodeY, terminalNodeX, terminalNodeY somewhere

                    double weight;
                    try
                    {
                        weight = Double.Parse((String)dataSet2.Tables["Edges"].Rows[row].ItemArray[6]);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Invalid weight in row " + row);
                    }

                    int initialIndex = Vertices.FindIndex(item => initialNode.Equals(item.Name));
                    int terminalIndex = Vertices.FindIndex(item => terminalNode.Equals(item.Name));

                    Vertex initial = initialIndex < 0 ? new Vertex(initialNode)
                                                    : Vertices[initialIndex];
                    Vertex terminal = terminalIndex < 0 ? new Vertex(terminalNode)
                                                    : Vertices[terminalIndex];


                    Edge newEdge = new Edge(initial, terminal, weight); 

                    Edges.Add(newEdge);

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
                    // else, they both already exist and there's no need to create any new vertices
                    
                    
                    // Regardless, adding edge from initial -> terminal 
                    terminal.AddEdge(initial, weight);
                    if (!IsDirected)
                    {
                        // and if not directed, from terminal -> initial
                        initial.AddEdge(terminal, weight);
                    }
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
            List<Edge> shortestPath = new List<Edge>();

            // TODO: Is this an accurate sort??
            /* Possible alternative:
             private List<EdgeStruct> SortEdges()
              {
             List<EdgeStruct> Sorted = new List<EdgeStruct>();
             foreach (EdgeStruct AddingEdge in Edges)
             {
                 foreach (EdgeStruct SortedEdge in Sorted)
                 {
                     if (AddingEdge.weight < SortedEdge.weight)
                     {
                         Sorted.Add(AddingEdge);
                     }
                 }
                 if (!(Sorted[Sorted.Count - 1]).Equals(AddingEdge))
                 {
                     Sorted.Add(AddingEdge);
                 }
             }
             return Sorted;
             }
             */
            List<Edge> orderedEdges = Edges.OrderBy((w) => w.Weight).ToList();

            List<List<Vertex>> visited = new List<List<Vertex>>();

            while (shortestPath.Count < this.Vertices.Count - 1)
            {
                Edge shortest = orderedEdges.ElementAt(0);
                int foundSourceWhere = -1;
                int foundDestinationWhere = -1;

                //check if will create cycle
                foreach (List<Vertex> connectedVertices in visited)
                {
                    foreach (Vertex currVertex in connectedVertices)
                    {
                        if (currVertex == shortest.Start)
                        {
                            foundSourceWhere = visited.IndexOf(connectedVertices);
                        }
                        else if (currVertex == shortest.End)
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
                        unconnected.Add(shortest.Start);
                        unconnected.Add(shortest.End);
                        visited.Add(unconnected);
                        shortestPath.Add(shortest);
                    }
                    else
                    {
                        visited[foundDestinationWhere].Add(shortest.Start);
                        shortestPath.Add(shortest);
                    }
                }
                else
                {
                    if (foundDestinationWhere == -1)
                    {
                        visited[foundSourceWhere].Add(shortest.End);
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
            if (!this.IsDirected) throw new Exception("forbidden algorithm attempt");

            List<Vertex> sorted = new List<Vertex>();

            // make adjacency list (use each vertex's neighbors list) - dictionary
            Dictionary<Vertex, List<Vertex>> adjacencyList = new Dictionary<Vertex, List<Vertex>>();

            // make indegree list (use each vertex's indegree) - dictionary
            Dictionary<Vertex, int> indegreeList = new Dictionary<Vertex, int>();

            foreach (Vertex vertex in this.Vertices)
            {
                foreach (Edge edge in vertex.Edges)
                {
                    if (edge.Start.Equals(vertex))
                    {
                        adjacencyList.Add(vertex, edge.End); // TODO: List<Vertex> not single vertex
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
