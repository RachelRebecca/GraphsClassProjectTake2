using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

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
                SqlCommand getGraphType = new SqlCommand("spGetGraphFlags", sqlCon);
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


                    Edge newEdge = new Edge(initial, terminal, weight); // weight = 1 for unweighted and database weights for weighted


                    Edges.Add(newEdge);

                    if (initialIndex < 0 && terminalIndex < 0)
                    {
                        // neither exist - create both, add edge between them
                        Vertices.Add(initial);
                        Vertices.Add(terminal);
                    }
                    else if (initialIndex < 0 && terminalIndex > -1)
                    {
                        // initial doesn't exist, create and add edge between it and terminal
                        Vertices.Add(initial);
                    }
                    else if (initialIndex > -1 && terminalIndex < 0)
                    {
                        // terminal doesn't exist, create and add edge between initial and it
                        Vertices.Add(terminal);
                    }
                    // else, they both already exist and there's no need to create any new vertices


                    // Regardless, adding edge from initial -> terminal 
                    terminal.AddEdge(initial, weight);
                    // if it's undirected, it's going to be officially initial -> terminal, but it won't make any real difference,
                    // this way there's only one edge for any given pair of vertices
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
        public List<Vertex> Dijkstra(Vertex source, Vertex target, out double shortestDistance)
        {
            if (source.Equals(target))
            {
                throw new Exception("Source and target are the same. Shortest distance: 0.0");
            }

            shortestDistance = 0.0;
            return new List<Vertex>();


            Dictionary<Vertex, DijkstraStruct> vertexAndStruct = new Dictionary<Vertex, DijkstraStruct>();
            DijkstraStruct currNode = new DijkstraStruct(true, 0, source, source);

            vertexAndStruct.Add(source, currNode);

            while (currNode.Vertex != target)
            {
                foreach (Edge edge in currNode.Vertex.Edges)
                {
                    // TODO: refactor, this is used in Prim
                    Vertex neighbor = edge.Start.Equals(currNode.Vertex) ? edge.End : edge.Start;
                    currNode = UpdateStructs(vertexAndStruct, currNode, out DijkstraStruct currStruct, out int newDistance, neighbor);

                }

                currNode = GetNewCurrNode(vertexAndStruct, currNode);

                CreatePath(source, vertexAndStruct, currNode);

                shortestDistance = currNode.DistanceFromStart;

            }
        }

        struct DijkstraStruct
        {
            internal bool SdFound { get; set; }
            internal int DistanceFromStart { get; set; }
            internal Vertex Parent { get; set; }
            internal Vertex Vertex { get; set; }

            public DijkstraStruct(bool sdFound, int distanceFromStart, Vertex parent, Vertex vertex)
            {
                this.SdFound = sdFound;
                this.DistanceFromStart = distanceFromStart;
                this.Parent = parent;
                this.Vertex = vertex;
            }
        }

        /*
        private static Dijkstra GetNewCurrNode(Dictionary<Vertex, Dijkstra> vertexStructs, Dijkstra currNode)
        {
            //find shortest false node and set to currNode and true
            int shortestFalse = MaxVal;
            foreach (KeyValuePair<Vertex, Dijkstra> d in vertexStructs)
            {

                if (!d.Value.SdFound && d.Value.DistanceFromStart < shortestFalse)
                {
                    currNode = d.Value;
                    shortestFalse = d.Value.DistanceFromStart;
                }
            }

            if (shortestFalse == MaxVal)
            {
                //all shortest paths have been found
                throw new Exception("No path exists");
            }


            currNode.SdFound = true;
            vertexStructs.Remove(currNode.Vertex);
            vertexStructs.Add(currNode.Vertex, currNode);
            return currNode;
        }

        private Dijkstra UpdateStructs(Dictionary<Vertex, Dijkstra> vertexStructs, Dijkstra currNode, out Dijkstra currStruct, out int newDistance, Vertex neighbor)
        {
            if (!vertexStructs.ContainsKey(neighbor))
            {
                Dijkstra newNode = new Dijkstra(false, MaxVal, null, neighbor);
                vertexStructs.Add(neighbor, newNode);
            }

            currStruct = vertexStructs[neighbor];
            newDistance = vertexStructs[currNode.Vertex].DistanceFromStart + graph.GetWeight(currNode.Vertex, neighbor);

            if (newDistance < currStruct.DistanceFromStart)
            {
                //update parent and shortest dist of v
                currStruct.Parent = currNode.Vertex;
                currStruct.DistanceFromStart = newDistance;
                vertexStructs.Remove(neighbor);
                vertexStructs.Add(neighbor, currStruct);


            }

            return currNode;
        }

        private void CreatePath(Vertex source, Dictionary<Vertex, Dijkstra> vertexStructs, Dijkstra currNode)
        {
            Vertex parent = currNode.Parent;
            Path.Add(parent);

            while (parent != source)
            {
                parent = vertexStructs[parent].Parent;
                Path.Insert(0, parent);
            }

            Path.Add(currNode.Vertex);
            
        }
         */

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

        public List<Edge> Prim(Vertex start)
        {
            // Has to a weighted graph 
            if (!this.IsWeighted || this.IsDirected) throw new Exception("forbidden algorithm attempt");

            Edge[] edges = new Edge[Vertices.Count - 1];
            List<PrimStruct> prims = new List<PrimStruct>();
            List<Vertex> foundVertices = new List<Vertex>();
            int numEdgesFound = 0;

            foundVertices.Add(start);

            // add prims for all neighbors of start
            foreach (Edge edge in start.Edges)
            {
                Vertex neighbor = edge.Start.Equals(start) ? edge.End : edge.Start;
                if (!foundVertices.Contains(neighbor))
                {
                    prims.Add(new PrimStruct(neighbor, edge.Weight, start));
                }
            }

            while (numEdgesFound < Vertices.Count - 1)
            {

                // get the vertex with the shortest cost
                prims.Sort((x, y) => x.Cost.CompareTo(y.Cost));
                PrimStruct currentPrim = prims[0];
                prims.RemoveAt(0);

                // add an edge to that prim
                edges[numEdgesFound] = Edges.Find(e => (e.Start.Equals(currentPrim.Parent) && e.End.Equals(currentPrim.vertex)) ||
                                                       (e.Start.Equals(currentPrim.vertex) && e.End.Equals(currentPrim.Parent))); 

                foundVertices.Add(currentPrim.vertex);
                numEdgesFound++;

                foreach (Edge edge in currentPrim.vertex.Edges)
                {
                    Vertex neighbor = edge.Start.Equals(start) ? edge.End : edge.Start;

                    if (!foundVertices.Contains(neighbor))
                    {
                        PrimStruct neighborPrim = prims.Find(p => p.vertex.Equals(neighbor));
                        if (neighborPrim.vertex != null)
                        {
                            if (edge.Weight < neighborPrim.Cost)
                            {
                                neighborPrim.Cost = edge.Weight;
                                neighborPrim.Parent = currentPrim.vertex;
                            }
                        }
                        else
                        {
                            prims.Add(new PrimStruct(neighbor, edge.Weight, currentPrim.vertex));
                        }
                    }

                }
            }

            return edges.ToList();

        }

        struct PrimStruct
        {
            public PrimStruct(Vertex vertex, double cost, Vertex parent)
            {
                this.vertex = vertex;
                this.Cost = cost;
                this.Parent = parent;
            }

            internal Vertex vertex;
            internal double Cost { get; set; }
            internal Vertex Parent { get; set; }
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

            List<Vertex> terminalVertices = new List<Vertex>();
            foreach (Vertex vertex in this.Vertices)
            {
                foreach (Edge edge in vertex.Edges)
                {
                    if (edge.Start.Equals(vertex))
                    {
                        terminalVertices.Add(edge.End);
                    }
                }
                adjacencyList.Add(vertex, terminalVertices);
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

                if (numVerticesAdded > Vertices.Count)
                {
                    throw new Exception("Graph contains cycle");
                }
            }

            return sorted;
        }
    }
}
