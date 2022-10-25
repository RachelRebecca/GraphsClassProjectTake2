﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GraphsClassProjectTakeTwo
{
    public partial class GraphProject : Form
    {
        public SqlConnection sqlCon;

        public List<String> GraphNames { get; set; } // Stored Procedure needs to split into two : one for names, and one that returns flags for each name

        public List<Button> GraphNameButtons { get; set; }

        private Graph CurrentGraph = null;

        private GraphOptions CurrentGraphOperation = GraphOptions.NONE;

        private Vertex[] SelectedNodes = new Vertex[] { null, null };

        private ToolTip toolTip;

        public GraphProject()
        {
            InitializeComponent();

            toolTip = new ToolTip();

            sqlCon = this.MakeSQLConnection();

            GraphNames = this.GetGraphNames(sqlCon);

            GraphNameButtons = new List<Button>();

            SetUpGraphNameButtons();

            //todo: getdata should actually happen when clicking on a specific graph name
        }

        public SqlConnection MakeSQLConnection()
        {
            var server = ConfigurationManager.AppSettings["SERVER"];
            var database = ConfigurationManager.AppSettings["DATABASE"];
            String strConnect = $"Server={server};Database={database};Trusted_Connection=True;";
            SqlConnection sqlCon = new SqlConnection(strConnect);
            sqlCon.Open();

            return sqlCon;

        }

        private List<String> GetGraphNames(SqlConnection sqlCon)
        {
            SqlCommand getAllGraphs = new SqlCommand("spGetGraphNames", sqlCon);
            getAllGraphs.CommandType = CommandType.StoredProcedure;
            getAllGraphs.ExecuteNonQuery();
            SqlDataAdapter da1 = new SqlDataAdapter(getAllGraphs);
            DataSet dataset1 = new DataSet();
            da1.Fill(dataset1, "Graphs");


            List<String> GraphNames = new List<String>();

            var nrGraphs = dataset1.Tables["Graphs"].Rows.Count;

            for (int row = 0; row < nrGraphs; ++row)
            {
                GraphNames.Add((String)dataset1.Tables["Graphs"].Rows[row].ItemArray[0]);
            }

            return GraphNames;
        }


        private void SetUpGraphNameButtons()
        {
            int x = 30;
            int y = 0;
            foreach (String name in GraphNames)
            {
                Button button = new Button();
                button.Name = name; // All button names are unique because in the SQL code, graph names are unique
                button.Text = name;
                button.Click += new EventHandler(btn_Click);
                button.Location = new Point(x, y);
                GraphNameButtons.Add(button);

                y += 100;

                panelGraphButtons.Controls.Add(button);
            }
            panelGraphButtons.Refresh();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            panelGraph.Controls.Clear();

            panelGraph.Refresh();

            panelGraph.BackColor = Color.Gray;

            Button button = (Button)sender;

            Graph graph = new Graph(button.Name, sqlCon);

            CurrentGraph = graph;

            CurrentGraphOperation = GraphOptions.NONE;

            ShowGraph(graph);

            SetUpAlgorithmButtons(graph);

            ShowWeights(graph);
        }

        private void SetUpAlgorithmButtons(Graph graph)
        {
            Topological.Enabled = true;
            Kruskal.Enabled = true;
            Prim.Enabled = true;
            Dijkstra.Enabled = true;
            removeEdge.Enabled = true;
            removeNode.Enabled = true;
            addEdge.Enabled = true;

            if (!graph.IsWeighted)
            {
                Dijkstra.Enabled = false;
            }
            if (graph.IsDirected)
            {
                Kruskal.Enabled = false;
                Prim.Enabled = false;
            }

            if (!CurrentGraph.IsDirected)
            {
                Topological.Enabled = false;

            }

        }

        private void ShowWeights(Graph graph)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Edges", typeof(string));
            table.Columns.Add("Weights", typeof(double));

            foreach (Edge edge in graph.Edges)
            {
                String edgeName = edge.Start.Name + edge.End.Name;
                double weight = edge.Weight;
                table.Rows.Add(new object[] { edgeName, weight });
            }

            tableEdgesWeights.DataSource = table;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            Control ctrl = this.GetChildAtPoint(e.Location);
            List<Control> algorithms = new List<Control>() { Dijkstra, Prim, Kruskal, Topological };
            List<Control> graphOperations = new List<Control>() { removeEdge, addEdge, removeNode };
            if (ctrl != null && algorithms.Contains(ctrl) && !ctrl.Enabled)
            {
                toolTip.SetToolTip(ctrl, "This algorithm is not available.");
                toolTip.Show(toolTip.GetToolTip(ctrl), ctrl, ctrl.Width / 2, ctrl.Height / 2);
                Console.WriteLine(toolTip);
            }
            else if (ctrl != null && graphOperations.Contains(ctrl) && !ctrl.Enabled)
            {
                toolTip.SetToolTip(ctrl, "This graph operation is not available.");
                toolTip.Show(toolTip.GetToolTip(ctrl), ctrl, ctrl.Width / 2, ctrl.Height / 2);
                Console.WriteLine(toolTip);
            }
            else
            {
                toolTip.Hide(this);
            }
        }

        private void TableWeights_CellClick(object sender, DataGridViewCellEventArgs evt)
        {
            CreateLinesBetweenNodes(CurrentGraph);

            DataGridView dgv = (DataGridView)sender;
            if (dgv != null && dgv.CurrentRow.Selected)
            {
                DataTable table = (DataTable)dgv.DataSource;
                String edgeInTable = (String)(table.Rows[dgv.CurrentRow.Index]["Edges"]);
                Edge edge = CurrentGraph.Edges.Find(e => (e.Start.Name + e.End.Name).Equals(edgeInTable) || (e.End.Name + e.Start.Name).Equals(edgeInTable));
                if (edge != null)
                {
                    SetUpGraphicsAndPen(CurrentGraph.IsDirected, out Graphics graphics, out Pen pen, Color.Yellow);
                    pen.Width = 1;

                    Point initialLocation = new Point((int)(edge.Start.XCoord * panelGraph.Width), (int)(edge.Start.YCoord * panelGraph.Height));
                    Point terminalLocation = new Point((int)(edge.End.XCoord * panelGraph.Width), (int)(edge.End.YCoord * panelGraph.Height));
                    graphics.DrawLine(pen, initialLocation, terminalLocation);
                }
            }

            ResetTableWeightsSelected();
        }

        private void ShowGraph(Graph graph)
        {
            CreateLabelType(graph);
            CreateLabelsAndNodes(graph);
            CreateLinesBetweenNodes(graph);
        }

        private void CreateLabelType(Graph graph)
        {
            Label labelGraphType = new Label();
            labelGraphType.Location = new Point(15, 20);

            String type = "";
            switch (graph.IsWeighted)
            {
                case true:
                    type += "Weighted ";
                    break;
                case false:
                    type += "Unweighted ";
                    break;
            }
            switch (graph.IsDirected)
            {
                case true:
                    type += "Digraph";
                    break;
                case false:
                    type += "Graph";
                    break;
            }

            labelGraphType.Text = type;
            labelGraphType.Size = new Size(150, 15);
            labelGraphType.Refresh();
            panelGraph.Controls.Add(labelGraphType);
        }


        private void CreateLabelsAndNodes(Graph graph)
        {
            List<Label> labelNodes = new List<Label>();
            for (int nodeNumber = 0; nodeNumber < graph.Vertices.Count; nodeNumber++)
            {
                Label label = new Label();
                label.Text = graph.Vertices[nodeNumber].Name;
                label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                Graphics graphics = panelGraph.CreateGraphics();
                Pen pen = new Pen(Color.Black);
                Point location = new Point((int)(graph.Vertices[nodeNumber].XCoord * panelGraph.Width), (int)(graph.Vertices[nodeNumber].YCoord * panelGraph.Height));
                Console.WriteLine("graphic location: " + location + " node name: " + graph.Vertices[nodeNumber].Name);
                graphics.DrawEllipse(pen, location.X - 5, location.Y - 5, 10, 10);

                label.Location = GetNewXAndY(location);
                label.Font = new Font("Arial", 8);
                label.Size = new Size(20, 15);
                label.ForeColor = Color.White;
                label.Visible = true;
                label.Click += new EventHandler(Label_Click); ;
                label.SendToBack();
                label.Refresh();

                labelNodes.Add(label);
            }

            foreach (Label label in labelNodes)
            {
                panelGraph.Controls.Add(label);
                label.Refresh();
            }
        }

        private Point GetNewXAndY(Point location)
        {
            int xCoord;
            int yCoord;

            if (location.X >= 200)
                xCoord = Math.Min(location.X + 10, panelGraph.Width);
            else
                xCoord = Math.Max(location.X - 15, 0);
            if (location.Y >= 200)
                yCoord = Math.Min(location.Y + 15, panelGraph.Height);
            else
                yCoord = Math.Max(location.Y - 15, 0);
            return new Point(xCoord, yCoord);
        }

        private void CreateLinesBetweenNodes(Graph graph)
        {
            SetUpGraphicsAndPen(graph.IsDirected, out Graphics graphics, out Pen pen, Color.Black);

            foreach (Edge edge in graph.Edges)
            {
                pen.Color = Color.Black;

                Point initialLocation = new Point((int)(edge.Start.XCoord * panelGraph.Width), (int)(edge.Start.YCoord * panelGraph.Height));
                Point terminalLocation = new Point((int)(edge.End.XCoord * panelGraph.Width), (int)(edge.End.YCoord * panelGraph.Height));
                graphics.DrawLine(pen, initialLocation, terminalLocation);
            }
        }

        private void Kruskal_Click(object sender, EventArgs e)
        {
            CurrentGraphOperation = GraphOptions.KRUSKAL;

            if (CurrentGraph != null && !CurrentGraph.IsDirected)
            {
                CreateLinesBetweenNodes(CurrentGraph);

                ResetTableWeightsSelected();

                try
                {
                    List<Edge> edges = CurrentGraph.Kruskal();
                    // draw minimum spanning graph edges in red
                    DrawRedLines(edges);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        private void Topological_Click(object sender, EventArgs e)
        {
            CurrentGraphOperation = GraphOptions.TOPOLOGICAL;

            if (CurrentGraph != null && CurrentGraph.IsDirected)
            {
                CreateLinesBetweenNodes(CurrentGraph);

                ResetTableWeightsSelected();

                string topologicalOutput = "";

                try
                {
                    List<Vertex> output = CurrentGraph.TopologicalSort();


                    foreach (Vertex vertex in output)
                    {
                        topologicalOutput += vertex.Name + " ";
                    }
                }
                catch (Exception exception)
                {
                    topologicalOutput = exception.Message;
                }

                MessageBox.Show("Topological sort of " + CurrentGraph.Name + ":\n\n" + topologicalOutput);

            }
        }

        private void Prim_Click(object sender, EventArgs e)
        {
            CurrentGraphOperation = GraphOptions.PRIM;

            if (CurrentGraph != null && !CurrentGraph.IsDirected)
            {
                CreateLinesBetweenNodes(CurrentGraph);

                ResetTableWeightsSelected();

                MessageBox.Show("Click on the label near the node that you want to use for the algorithm");
            }

        }

        private void Dijkstra_Click(object sender, EventArgs e)
        {
            CurrentGraphOperation = GraphOptions.DIJKSTRA;

            SelectedNodes = new Vertex[] {null, null};

            if (CurrentGraph != null && CurrentGraph.IsWeighted)
            {
                CreateLinesBetweenNodes(CurrentGraph);

                ResetTableWeightsSelected();

                MessageBox.Show("Click on the label near the starting node that you want to use for the algorithm");

            }
        }

        private void Label_Click(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            if (CurrentGraphOperation == GraphOptions.PRIM)
            {
                CreateLinesBetweenNodes(CurrentGraph);

                ResetTableWeightsSelected();

                int initialIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                if (initialIndex < 0)
                {
                    MessageBox.Show("Something went wrong, the Vertex couldn't be found");
                }
                else
                {
                    Vertex start = CurrentGraph.Vertices[initialIndex];

                    try
                    {
                        List<Edge> output = CurrentGraph.Prim(start);

                        // draw minimum spanning graph edges in red
                        DrawRedLines(output);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        Console.WriteLine(exception.StackTrace);
                    }
                }

            }
            else if (CurrentGraphOperation == GraphOptions.DIJKSTRA)
            {
                if (SelectedNodes[0] == null)
                {
                    // you haven't yet selected a node

                    CreateLinesBetweenNodes(CurrentGraph);

                    ResetTableWeightsSelected();

                    int initialIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                    if (initialIndex >= 0)
                    {
                        Vertex start = CurrentGraph.Vertices[initialIndex];

                        SelectedNodes[0] = start;

                        MessageBox.Show("You chose " + label.Text + " as your starting node\nPlease click on another label near the ending node you want to use for the algorithm");
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong, the Vertex couldn't be found");
                    }

                }
                else if (SelectedNodes[1] == null)
                {
                    // you have already selected the starting node

                    int terminalIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                    if (terminalIndex >= 0)
                    {
                        Vertex end = CurrentGraph.Vertices[terminalIndex];

                        SelectedNodes[1] = end;

                        try
                        {
                            List<Vertex> output = CurrentGraph.Dijkstra(SelectedNodes[0], end, out double shortestDistance);

                            // Draw path one by one using red lines
                            DrawRedLines(output, 500);

                            MessageBox.Show("Shortest distance: " + shortestDistance);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message);
                        }

                        SelectedNodes = new Vertex[] { null, null };

                    }
                    else
                    {
                        MessageBox.Show("Something went wrong, the Vertex couldn't be found");
                    }

                }
                else
                {
                    // do nothing, we only care about the starting and ending node
                }

            }
            else if (CurrentGraphOperation == GraphOptions.REMOVE_NODE)
            {
                int initialIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                if (initialIndex < 0)
                {
                    MessageBox.Show("Something went wrong, the Vertex couldn't be found");
                }
                else
                {
                    ResetPanel();

                    Vertex nodeToBeDeleted = CurrentGraph.Vertices[initialIndex];
                    nodeToBeDeleted.Outdegree = 0;
                    nodeToBeDeleted.Indegree = 0;

                    List<Edge> edgesToBeRemoved = new List<Edge>();

                    foreach (Edge edge in CurrentGraph.Edges)
                    {
                        if (edge.Start.Equals(nodeToBeDeleted))
                        {
                            edgesToBeRemoved.Add(edge);
                            edge.End.Indegree--;
                            Edge deletedEdge = edge.End.Edges.Find(edg => edg.Start.Equals(nodeToBeDeleted) || edg.End.Equals(nodeToBeDeleted));
                            edge.End.Edges.Remove(deletedEdge);
                        }
                        else if (edge.End.Equals(nodeToBeDeleted))
                        {
                            edgesToBeRemoved.Add(edge);
                            edge.Start.Outdegree--;
                        }

                    }

                    CurrentGraph.Vertices.Remove(nodeToBeDeleted);
                    nodeToBeDeleted.Edges.Clear();
                    CurrentGraph.Edges.RemoveAll(edge => edgesToBeRemoved.Contains(edge));

                    ResetForm();

                }
            }
            else if (CurrentGraphOperation == GraphOptions.REMOVE_EDGE)
            {
                if (SelectedNodes[0] == null)
                {
                    int initialIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                    if (initialIndex >= 0)
                    {
                        Vertex start = CurrentGraph.Vertices[initialIndex];

                        SelectedNodes[0] = start;

                        MessageBox.Show("You chose " + label.Text + " as your starting node\nPlease click on another label near the ending node of the edge you want to remove");
                    }


                    else
                    {
                        MessageBox.Show("Something went wrong, the Vertex couldn't be found");
                    }

                }
                else if (SelectedNodes[1] == null)
                {
                    // you have already selected the starting node

                    int terminalIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                    if (terminalIndex >= 0)
                    {
                        ResetPanel();

                        Vertex end = CurrentGraph.Vertices[terminalIndex];

                        SelectedNodes[1] = end;

                        Edge edge = CurrentGraph.Edges.Find(edg => (edg.Start.Name.Equals(SelectedNodes[0].Name) && edg.End.Name.Equals(end.Name))
                        || (!CurrentGraph.IsDirected && edg.End.Name.Equals(SelectedNodes[0].Name) && edg.Start.Name.Equals(end.Name)));

                        if (edge != null)
                        {
                            edge.Start.Indegree--;
                            edge.End.Indegree++;
                            edge.End.Edges.Remove(edge);
                            CurrentGraph.Edges.Remove(edge);
                        }


                        ResetForm();
                    }
                }
            }
            else if (CurrentGraphOperation == GraphOptions.ADD_EDGE)
            {
                if (SelectedNodes[0] == null)
                {
                    int initialIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                    if (initialIndex >= 0)
                    {
                        Vertex start = CurrentGraph.Vertices[initialIndex];

                        SelectedNodes[0] = start;

                        MessageBox.Show("You chose " + label.Text + " as your starting node\nPlease click on another label near the ending node of the edge you want to add");
                    }

                    else
                    {
                        MessageBox.Show("Something went wrong, the Vertex couldn't be found");
                    }

                }
                else if (SelectedNodes[1] == null)
                {
                    // you have already selected the starting node

                    int terminalIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                    if (terminalIndex >= 0)
                    {
                        ResetPanel();

                        Vertex end = CurrentGraph.Vertices[terminalIndex];

                        SelectedNodes[1] = end;

                        // TODO: user input to select weight
                        Edge edge = new Edge(SelectedNodes[0], end, 1);
                        SelectedNodes[0].Outdegree++;
                        end.Indegree++;
                        CurrentGraph.Edges.Add(edge);
                        end.Edges.Add(edge);

                        ResetForm();
                    }
                }
            }
        }

        private void ResetPanel()
        {
            panelGraph.Controls.Clear();

            panelGraph.Refresh();

            panelGraph.BackColor = Color.Gray;
        }

        private void ResetForm()
        {
            ShowGraph(CurrentGraph);

            CreateLinesBetweenNodes(CurrentGraph);

            ShowWeights(CurrentGraph);

            ResetTableWeightsSelected();

            SelectedNodes = new Vertex[] { null, null };
        }

        private void DrawRedLines(List<Edge> input)
        {
            SetUpGraphicsAndPen(CurrentGraph.IsDirected, out Graphics graphics, out Pen pen, Color.Red);

            List<string> edgeNames = new List<string>();

            foreach (Edge edge in input)
            {
                edgeNames.Add(edge.Start.Name + edge.End.Name);
                Point initialLocation = new Point((int)(edge.Start.XCoord * panelGraph.Width), (int)(edge.Start.YCoord * panelGraph.Height));
                Point terminalLocation = new Point((int)(edge.End.XCoord * panelGraph.Width), (int)(edge.End.YCoord * panelGraph.Height));
                graphics.DrawLine(pen, initialLocation, terminalLocation);
            }

            DataTable table = (DataTable)tableEdgesWeights.DataSource;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (edgeNames.Contains((String)table.Rows[i]["Edges"]))
                {
                    tableEdgesWeights.Rows[i].Selected = true;
                }
            }
        }
        private void DrawRedLines(List<Vertex> input, int sleepTime)
        {
            SetUpGraphicsAndPen(CurrentGraph.IsDirected, out Graphics graphics, out Pen pen, Color.Red);

            Vertex startingVertex, endingVertex;

            List<string> edgeNames = new List<string>();

            for (int i = 0; i < input.Count - 1; i++)
            {
                startingVertex = input[i];
                endingVertex = input[i + 1];

                edgeNames.Add(startingVertex.Name + endingVertex.Name);
                Point initialLocation = new Point((int)(startingVertex.XCoord * panelGraph.Width), (int)(startingVertex.YCoord * panelGraph.Height));
                Point terminalLocation = new Point((int)(endingVertex.XCoord * panelGraph.Width), (int)(endingVertex.YCoord * panelGraph.Height));
                input.ForEach(e => Console.WriteLine(e.Name));
                Console.WriteLine("Start: " + startingVertex.Name + " End: " + endingVertex.Name);

                graphics.DrawLine(pen, initialLocation, terminalLocation);

                System.Threading.Thread.Sleep(sleepTime);
            }

            DataTable table = (DataTable)tableEdgesWeights.DataSource;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (edgeNames.Contains((String)table.Rows[i]["Edges"]))
                {
                    tableEdgesWeights.Rows[i].Selected = true;
                }
            }

        }

        private void SetUpGraphicsAndPen(bool useArrowCap, out Graphics graphics, out Pen pen, Color penColor)
        {
            graphics = panelGraph.CreateGraphics();
            pen = new Pen(penColor);
            if (useArrowCap)
            {
                AdjustableArrowCap adjustableArrowCap = new AdjustableArrowCap(3, 3);
                pen.CustomEndCap = adjustableArrowCap;
            }
        }

        private void ResetTableWeightsSelected()
        {
            foreach (DataGridViewRow row in tableEdgesWeights.Rows)
            {
                row.Selected = false;
            }
        }

        private void RemoveNode_Click(object sender, EventArgs e)
        {
            CurrentGraphOperation = GraphOptions.REMOVE_NODE;

            DoGraphOperation("node that you want to remove");
        }

        private void RemoveEdge_Click(object sender, EventArgs e)
        {
            CurrentGraphOperation = GraphOptions.REMOVE_EDGE;

            DoGraphOperation("starting node of the edge that you want to remove");

        }

        private void AddEdge_Click(object sender, EventArgs e)
        {
            CurrentGraphOperation = GraphOptions.ADD_EDGE;

            DoGraphOperation("starting node that you want to add an edge to");

        }

        private void DoGraphOperation(string operation)
        {
            if (CurrentGraph != null)
            {
                CreateLinesBetweenNodes(CurrentGraph);

                SelectedNodes = new Vertex[] { null, null };

                ResetTableWeightsSelected();

                MessageBox.Show("Click on the label near the " + operation);
            }
        }
    }
}
