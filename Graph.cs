using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace GraphsClassProjectTakeTwo
{
    /// <summary>
    /// Represents a Graph 
    ///     - Contains basic information about the graph - takes this information from the database
    ///     - Contains graph algorithms:
    ///         - Dijkstra's Algorithm
    ///         - Kruskal's MST Algorithm
    ///         - Topological Sort
    ///         - Prim's MST Algorithm
    ///         - Connected algorithm
    ///    
    /// Graph is split into partial classes - this file contains the basic Graph information
    /// Other files are: DijkstraAlgorithm, KruskalAlgorithm, TopologicalSort, PrimAlgorithm, IsConnectedAlgorithm
    /// </summary>

    public partial class Graph
    {
        // Name of the Graph
        public string Name { get; set; }

        // Is weighted flag
        public bool IsWeighted { get; set; }

        //Is directed flag
        public bool IsDirected { get; set; }

        // List of Vertices within the Graph
        public List<Vertex> Vertices { get; set; }

        // List of Edges within the Graph
        public List<Edge> Edges { get; set; }

        // TODO: Refactor the Edges.Where and Edges.Find methods 
        public Graph(string name, SqlConnection sqlCon)
        {
            this.Name = name;
            this.Vertices = new List<Vertex>();
            this.Edges = new List<Edge>();
            LoadGraph(sqlCon);
        }

        /// <summary>
        /// Loads the graph's information when the graph is created
        /// Sets the graph's IsDirected, IsWeighted, Vertices, Edges
        /// </summary>
        /// <param name="sqlCon">The open SQL connection getting the graph information from the database</param>
        /// <returns>if loading the graph was successful or not</returns>
        public bool LoadGraph(SqlConnection sqlCon)
        {
            bool retVal = true;
            try
            {
                SetGraphFlagsUsingStoredProcedure(sqlCon);

                DataSet dataSet2 = GetEdgesFromStoredProcedure(sqlCon);

                var nrEdges = dataSet2.Tables["Edges"].Rows.Count;

                for (int row = 0; row < nrEdges; ++row)
                {
                    try
                    {
                        GetSpecificEdgeValues(dataSet2, row, out string initialNode, out string terminalNode, out double weight,
                            out double ix, out double iy, out double tx, out double ty); // initial and terminal X and Y coordinates

                        int initialIndex = Vertices.FindIndex(item => initialNode.Equals(item.Name));
                        int terminalIndex = Vertices.FindIndex(item => terminalNode.Equals(item.Name));

                        // if vertex is not already in Vertices, make a new vertex, otherwise, use the found vertex
                        Vertex initial = initialIndex < 0 ? new Vertex(initialNode, ix, iy)
                                                        : Vertices[initialIndex];
                        Vertex terminal = terminalIndex < 0 ? new Vertex(terminalNode, tx, ty)
                                                        : Vertices[terminalIndex];

                        // add new edge to Edges list
                        Edge newEdge = new Edge(initial, terminal, weight);
                        Edges.Add(newEdge);

                        UpdateVertices(weight, initialIndex, terminalIndex, initial, terminal); 
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Something went wrong with this row - it is being ignored.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Uses stored procedure spGetGraphFlags to set if the graph is weighted and/or directed
        /// </summary>
        /// <param name="sqlCon">The open SQL Connection getting the flags from the database</param>
        private void SetGraphFlagsUsingStoredProcedure(SqlConnection sqlCon)
        {
            SqlCommand getGraphType = new SqlCommand("spGetGraphFlags", sqlCon);

            SqlParameter sqlParameter1 = new SqlParameter
            {
                ParameterName = "@GraphName",
                Value = Name // set @GraphName to this graph's Name
            };
            getGraphType.Parameters.Add(sqlParameter1);

            getGraphType.CommandType = CommandType.StoredProcedure;
            getGraphType.ExecuteNonQuery();
            SqlDataAdapter da1 = new SqlDataAdapter(getGraphType);
            DataSet dataSet1 = new DataSet();
            da1.Fill(dataSet1, "Flags");

            IsWeighted = (bool)dataSet1.Tables["Flags"].Rows[0].ItemArray[0];
            IsDirected = (bool)dataSet1.Tables["Flags"].Rows[0].ItemArray[1];
        }

        /// <summary>
        /// Get this graph's Edges using the stored procedure
        /// </summary>
        /// <param name="sqlCon">The open SQL connection getting the edges from the database</param>
        /// <returns>DataSet containing this graph's Edges</returns>
        private DataSet GetEdgesFromStoredProcedure(SqlConnection sqlCon)
        {
            SqlCommand getEdgesForGraph = new SqlCommand("spGetEdges", sqlCon);
            SqlParameter sqlParameter2 = new SqlParameter
            {
                ParameterName = "@GraphName",
                Value = Name // set @GraphName to this graph's Name
            };

            getEdgesForGraph.Parameters.Add(sqlParameter2);

            getEdgesForGraph.CommandType = CommandType.StoredProcedure;
            getEdgesForGraph.ExecuteNonQuery();
            SqlDataAdapter da2 = new SqlDataAdapter(getEdgesForGraph);
            DataSet dataSet2 = new DataSet();
            da2.Fill(dataSet2, "Edges");
            return dataSet2;
        }

        /// <summary>
        /// Uses DataSet to get all the specific edge's values
        /// </summary>
        /// <param name="dataSet2">DataSet containing edge values</param>
        /// <param name="row">The specific index for this edge from the DataSet's table</param>
        /// <param name="initialNode">out parameter for the name of the initial vertex</param>
        /// <param name="terminalNode">out parameter for the name of the terminal vertex</param>
        /// <param name="weight">the weight of the edge (1 if !IsWeighted)</param>
        /// <param name="ix">out parameter for the x coordinate of the initial node</param>
        /// <param name="iy">out parameter for the y coordinate of the initial node</param>
        /// <param name="tx">out parameter for the x coordinate of the terminal node</param>
        /// <param name="ty">out parameter for the y coordinate of the terminal node</param>
        /// <exception cref="Exception">invalid row value</exception>
        private static void GetSpecificEdgeValues(DataSet dataSet2, int row, out string initialNode, out string terminalNode, out double weight, out double ix, out double iy, out double tx, out double ty)
        {
            initialNode = (string)dataSet2.Tables["Edges"].Rows[row].ItemArray[0];
            terminalNode = (string)dataSet2.Tables["Edges"].Rows[row].ItemArray[3];
            try
            {
                weight = (double)dataSet2.Tables["Edges"].Rows[row].ItemArray[6];
            }
            catch (Exception)
            {
                throw new Exception("Invalid weight in row " + row + ". Weight = " + dataSet2.Tables["Edges"].Rows[row].ItemArray[6]);
            }

            try
            {
                ix = (double)dataSet2.Tables["Edges"].Rows[row].ItemArray[1];
                iy = (double)dataSet2.Tables["Edges"].Rows[row].ItemArray[2];
                tx = (double)dataSet2.Tables["Edges"].Rows[row].ItemArray[4];
                ty = (double)dataSet2.Tables["Edges"].Rows[row].ItemArray[5];

            }
            catch (Exception)
            {
                throw new Exception("Invalid location given in row " + row);
            }
        }

        /// <summary>
        /// Update the Vertices list and add edge between the initial and terminal vertex
        /// </summary>
        /// <param name="weight">the weight of the edge</param>
        /// <param name="initialIndex">index of initial vertex in Vertices list or -1 if not in Vertices list</param>
        /// <param name="terminalIndex">index of terminal vertex in Vertices list or -1 if not in Vertices list</param>
        /// <param name="initial">initial Vertex</param>
        /// <param name="terminal">terminal Vertex</param>
        private void UpdateVertices(double weight, int initialIndex, int terminalIndex, Vertex initial, Vertex terminal)
        {
            if (initialIndex < 0 && terminalIndex < 0)
            {
                // neither exist - create both
                Vertices.Add(initial);
                Vertices.Add(terminal);
            }
            else if (initialIndex < 0 && terminalIndex > -1)
            {
                // initial doesn't exist - create
                Vertices.Add(initial);
            }
            else if (initialIndex > -1 && terminalIndex < 0)
            {
                // terminal doesn't exist - create
                Vertices.Add(terminal);
            }
            // else, they both already exist and there's no need to create any new vertices


            // Add new edge from initial -> terminal -- NOTE: there is only one edge for any given pair of vertices
            terminal.AddEdge(initial, weight);

            // if !IsDirected, officially the edge is initial -> terminal, but it won't make any real difference
        }


    }
}
