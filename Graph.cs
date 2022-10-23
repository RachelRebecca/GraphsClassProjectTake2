﻿using System;
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
        public String Name { get; set; }

        public bool IsWeighted { get; set; }

        public bool IsDirected { get; set; }

        public List<Vertex> Vertices { get; set; }

        public List<Edge> Edges { get; set; } // TODO: only edge list?

        // constructor
        public Graph(String name, SqlConnection sqlCon)
        {
            this.Name = name;
            this.Vertices = new List<Vertex>();
            this.Edges = new List<Edge>();
            LoadGraph(sqlCon);
        }


        // methods
        public bool LoadGraph(SqlConnection sqlCon)
        {
            bool retVal = true;
            try
            {
                //get graph type
                SqlCommand getGraphType = new SqlCommand("spGetGraphFlags", sqlCon);
                SqlCommand getEdgesForGraph = new SqlCommand("spGetEdges", sqlCon);

                SqlParameter sqlParameter1 = new SqlParameter();
                sqlParameter1.ParameterName = "@GraphName";
                sqlParameter1.Value = this.Name;

                SqlParameter sqlParameter2 = new SqlParameter();
                sqlParameter2.ParameterName = "@GraphName";
                sqlParameter2.Value = this.Name;

                getGraphType.Parameters.Add(sqlParameter1);
                getEdgesForGraph.Parameters.Add(sqlParameter2);

                getGraphType.CommandType = CommandType.StoredProcedure;
                getGraphType.ExecuteNonQuery();
                SqlDataAdapter da1 = new SqlDataAdapter(getGraphType);
                DataSet dataSet1 = new DataSet();
                da1.Fill(dataSet1, "Flags");

                // if it's "1", then it's true otherwise it's false
                this.IsWeighted = (bool)dataSet1.Tables["Flags"].Rows[0].ItemArray[0];
                this.IsDirected = (bool)dataSet1.Tables["Flags"].Rows[0].ItemArray[1];

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
                    String terminalNode = (String)dataSet2.Tables["Edges"].Rows[row].ItemArray[3];

                    // TODO: store initialNodeX, initialNodeY, terminalNodeX, terminalNodeY somewhere

                    double weight;
                    try
                    {
                        weight = (double)dataSet2.Tables["Edges"].Rows[row].ItemArray[6];
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Invalid weight in row " + row + ". Weight = " + dataSet2.Tables["Edges"].Rows[row].ItemArray[6]);
                    }

                    double ix, iy, tx, ty;

                    try
                    {
                        ix = (double)dataSet2.Tables["Edges"].Rows[row].ItemArray[1];
                        iy = (double)dataSet2.Tables["Edges"].Rows[row].ItemArray[2];
                        tx = (double)dataSet2.Tables["Edges"].Rows[row].ItemArray[4];
                        ty = (double)dataSet2.Tables["Edges"].Rows[row].ItemArray[5];

                    }
                    catch (Exception e)
                    {
                        throw new Exception("Invalid location given in row " + row);
                    }


                    int initialIndex = Vertices.FindIndex(item => initialNode.Equals(item.Name));
                    int terminalIndex = Vertices.FindIndex(item => terminalNode.Equals(item.Name));

                    Vertex initial = initialIndex < 0 ? new Vertex(initialNode, ix, iy)
                                                    : Vertices[initialIndex];
                    Vertex terminal = terminalIndex < 0 ? new Vertex(terminalNode, tx, ty)
                                                    : Vertices[terminalIndex];


                    Edge newEdge = new Edge(initial, terminal, weight); // weight = 1 for unweighted and database weights for weighted
                    Edges.Add(newEdge);

                    /*if (!IsDirected)
                    {
                        Edge newEdge2 = new Edge(terminal, initial, weight); // weight = 1 for unweighted and database weights for weighted
                        Edges.Add(newEdge2);
                    }*/

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
                Console.WriteLine(e.StackTrace);
                retVal = false;
            }

            return retVal;
        }

        public List<Vertex> Dijkstra(Vertex source, Vertex target, out double shortestDistance)
        {

            // Has to be weighted (directed or undirected)
            if (!this.IsWeighted) throw new Exception("forbidden algorithm attempt");

            List<Vertex> path = new List<Vertex>();

            if (source.Equals(target))
            {
                throw new Exception("Source and target are the same. Shortest distance: 0.0");
            }

            shortestDistance = 0.0;

            Dictionary<Vertex, DijkstraStruct> vertexAndStruct = new Dictionary<Vertex, DijkstraStruct>();
            DijkstraStruct currNode = new DijkstraStruct(true, 0, source, source);

            vertexAndStruct.Add(source, currNode);

            while (currNode.Vertex != target)
            {
                foreach (Edge edge in currNode.Vertex.Edges)
                {
                    // TODO: refactor, this is used in Prim
                    Vertex neighbor = edge.Start.Equals(currNode.Vertex) ? edge.End : edge.Start;
                    currNode = UpdateStructs(vertexAndStruct, currNode, out DijkstraStruct currStruct, out double newDistance, neighbor);

                }

                currNode = GetNewCurrNode(vertexAndStruct, currNode);

                path = UpdatePath(path, source, vertexAndStruct, currNode);

                shortestDistance = currNode.DistanceFromStart;

            }

            return path;
        }

        struct DijkstraStruct
        {
            internal bool SdFound { get; set; }
            internal double DistanceFromStart { get; set; }
            internal Vertex Parent { get; set; }
            internal Vertex Vertex { get; set; }

            public DijkstraStruct(bool sdFound, double distanceFromStart, Vertex parent, Vertex vertex)
            {
                this.SdFound = sdFound;
                this.DistanceFromStart = distanceFromStart;
                this.Parent = parent;
                this.Vertex = vertex;
            }
        }

        private DijkstraStruct UpdateStructs(Dictionary<Vertex, DijkstraStruct> vertexStructs, DijkstraStruct currNode,
                                             out DijkstraStruct currStruct, out double newDistance, Vertex neighbor)
        {
            int maxVal = int.MaxValue;
            if (!vertexStructs.ContainsKey(neighbor))
            {
                DijkstraStruct newNode = new DijkstraStruct(false, maxVal, null, neighbor);
                vertexStructs.Add(neighbor, newNode);
            }

            currStruct = vertexStructs[neighbor];

            Edge edge = Edges.Find(e => (e.Start.Equals(currNode.Vertex) && e.End.Equals(neighbor)) ||
                                                       (e.Start.Equals(neighbor) && e.End.Equals(currNode.Vertex)));

            newDistance = vertexStructs[currNode.Vertex].DistanceFromStart + edge.Weight;

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


        private DijkstraStruct GetNewCurrNode(Dictionary<Vertex, DijkstraStruct> vertexStructs, DijkstraStruct currNode)
        {
            int maxVal = int.MaxValue;

            //find shortest false node and set to currNode and true
            double shortestFalse = maxVal;
            foreach (KeyValuePair<Vertex, DijkstraStruct> d in vertexStructs)
            {

                if (!d.Value.SdFound && d.Value.DistanceFromStart < shortestFalse)
                {
                    currNode = d.Value;
                    shortestFalse = d.Value.DistanceFromStart;
                }
            }

            if (shortestFalse == maxVal)
            {
                //all shortest paths have been found
                throw new Exception("No path exists");
            }


            currNode.SdFound = true;
            vertexStructs.Remove(currNode.Vertex);
            vertexStructs.Add(currNode.Vertex, currNode);
            return currNode;
        }

        private List<Vertex> UpdatePath(List<Vertex> path, Vertex source, Dictionary<Vertex, DijkstraStruct> vertexStructs, DijkstraStruct currNode)
        {
            Vertex parent = currNode.Parent;
            path.Add(parent);

            while (parent != source)
            {
                parent = vertexStructs[parent].Parent;
                path.Insert(0, parent);
            }

            path.Add(currNode.Vertex);

            return path;

        }


        public List<Edge> Kruskal()
        {
            // Has to be a graph (weighted or unweighted)
            if (this.IsDirected) throw new Exception("forbidden algorithm attempt");

            List<Edge> shortestPath = new List<Edge>();

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
            // Has to be a graph (weighted or unweighted)
            if (this.IsDirected) throw new Exception("forbidden algorithm attempt");

            Edge[] edges = new Edge[Vertices.Count - 1];
            List<PrimStruct> prims = new List<PrimStruct>();
            List<Vertex> foundVertices = new List<Vertex>();
            int numEdgesFound = 0;

            foundVertices.Add(start);

            var startEdges = Edges.Where(e => (e.Start.Equals(start) || e.End.Equals(start)));

            // add prims for all neighbors of start
            foreach (Edge edge in startEdges)
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


                var currentEdges = Edges.Where(e => (e.Start.Equals(currentPrim.vertex) || e.End.Equals(currentPrim.vertex)));
           

                foreach (Edge edge in currentEdges)
                {
                    Vertex neighbor = edge.Start.Equals(currentPrim.vertex) ? edge.End : edge.Start;
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
            // Has to be a digraph (weighted or unweighted)
            if (!this.IsDirected) throw new Exception("forbidden algorithm attempt");

            List<Vertex> sorted = new List<Vertex>();

            // make adjacency list (use each vertex's neighbors list) - dictionary
            Dictionary<Vertex, List<Vertex>> adjacencyList = new Dictionary<Vertex, List<Vertex>>();

            // make indegree list (use each vertex's indegree) - dictionary
            Dictionary<Vertex, int> indegreeList = new Dictionary<Vertex, int>();

            List<Vertex> terminalVertices = new List<Vertex>();
            foreach (Vertex vertex in this.Vertices)
            {
                var currentEdges = Edges.Where(e => (e.Start.Equals(vertex) || e.End.Equals(vertex)));

                foreach (Edge edge in currentEdges)
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
