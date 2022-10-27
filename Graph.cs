using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace GraphsClassProjectTakeTwo
{
    public partial class Graph
    {
        public String Name { get; set; }

        public bool IsWeighted { get; set; }

        public bool IsDirected { get; set; }

        public List<Vertex> Vertices { get; set; }

        public List<Edge> Edges { get; set; }

        // TODO: Refactor the Edges.Where and Edges.Find methods 
        public Graph(String name, SqlConnection sqlCon)
        {
            this.Name = name;
            this.Vertices = new List<Vertex>();
            this.Edges = new List<Edge>();
            LoadGraph(sqlCon);
        }

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

        public bool IsConnected()
        {
            bool IsConnected = false;

            if (Vertices.Count > 0)
            {
                // initialized to false
                bool[] InDirection1 = new bool[Vertices.Count];
                bool[] InDirection2 = new bool[Vertices.Count];

                // InDirection1: 
                Queue<Vertex> Neighbors = new Queue<Vertex>();
                Vertex currentVertex = Vertices[0];

                InDirection1[0] = true;

                var startEdges = Edges.Where(e => e.Start.Equals(currentVertex));
                foreach (Edge edge in startEdges)
                {
                    Neighbors.Enqueue(edge.End);
                }

                while (InDirection1.Contains(false))
                {
                    if (Neighbors.Count == 0)
                    {
                        break;
                    }
                    currentVertex = Neighbors.Dequeue();
                    if (InDirection1[Vertices.IndexOf(currentVertex)])
                    {
                        continue;
                    }
                    else
                    {
                        InDirection1[Vertices.IndexOf(currentVertex)] = true;
                        startEdges = Edges.Where(e => e.Start.Equals(currentVertex));
                        foreach (Edge edge in startEdges)
                        {
                            Neighbors.Enqueue(edge.End);
                        }
                    }
                }

                //InDirection2: 
                Neighbors = new Queue<Vertex>();
                currentVertex = Vertices[0];
                InDirection2[0] = true;

                foreach (Edge edge in currentVertex.Edges)
                {
                    Neighbors.Enqueue(edge.Start);
                }

                while (InDirection1.Contains(false))
                {
                    if (Neighbors.Count == 0)
                    {
                        break;
                    }
                    currentVertex = Neighbors.Dequeue();
                    if (InDirection1[Vertices.IndexOf(currentVertex)])
                    {
                        continue;
                    }
                    else
                    {
                        InDirection1[Vertices.IndexOf(currentVertex)] = true;
                        foreach (Edge edge in currentVertex.Edges)
                        {
                            Neighbors.Enqueue(edge.Start);
                        }

                    }
                }

                IsConnected = true;

                if (IsDirected)
                {
                    for (int i = 0; i < Vertices.Count; i++)
                    {
                        if (!InDirection1[i] && !InDirection2[i])
                        {
                            IsConnected = false;
                            break;
                        }
                    }
                }

                else
                {
                    IsConnected = !(InDirection1.Contains(false) || InDirection1.Contains(false));
                }
            }

            return IsConnected;

        }
    }
}
