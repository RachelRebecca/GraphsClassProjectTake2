using System;
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

        private AlgorithmType CurrentAlgorithm = AlgorithmType.NONE;

        private List<Vertex> SelectedDijkstraNodes = new List<Vertex>();

        public GraphProject()
        {
            InitializeComponent();

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

            CurrentAlgorithm = AlgorithmType.NONE;

            ShowGraph(graph);

            ShowWeights(graph);

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
                Console.WriteLine("label location: " + location + " label name: " + label.Text);

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
            CurrentAlgorithm = AlgorithmType.KRUSKAL;

            if (CurrentGraph == null)
            {
                MessageBox.Show("There is no graph showing yet.");
            }
            else if (CurrentGraph.IsWeighted && !CurrentGraph.IsDirected)
            {
                CreateLinesBetweenNodes(CurrentGraph);

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
            else
            {
                CreateLinesBetweenNodes(CurrentGraph);

                MessageBox.Show("Kruskal's Algorithm is not available for selected graph.");
            }
        }

        private void Topological_Click(object sender, EventArgs e)
        {
            CurrentAlgorithm = AlgorithmType.TOPOLOGICAL;

            if (CurrentGraph == null)
            {
                MessageBox.Show("There is no graph showing yet.");
            }
            else if (!CurrentGraph.IsDirected)
            {
                CreateLinesBetweenNodes(CurrentGraph);

                MessageBox.Show("Topological Sort is not available for selected graph.");
            }
            else
            {
                CreateLinesBetweenNodes(CurrentGraph);

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
            CurrentAlgorithm = AlgorithmType.PRIM;

            if (CurrentGraph == null)
            {
                MessageBox.Show("There is no graph showing yet.");
            }
            else if (CurrentGraph.IsWeighted && !CurrentGraph.IsDirected)
            {
                CreateLinesBetweenNodes(CurrentGraph);

                MessageBox.Show("Click on the label near the node that you want to use for the algorithm");
            }
            else
            {
                CreateLinesBetweenNodes(CurrentGraph);

                MessageBox.Show("Prim's Algorithm is not available for selected graph.");
            }
        }

        private void Dijkstra_Click(object sender, EventArgs e)
        {
            CurrentAlgorithm = AlgorithmType.DIJKSTRA;

            SelectedDijkstraNodes = new List<Vertex>();

            if (CurrentGraph == null)
            {
                MessageBox.Show("There is no graph showing yet.");
            }
            else if (!CurrentGraph.IsWeighted)
            {
                CreateLinesBetweenNodes(CurrentGraph);

                MessageBox.Show("Dijkstra's Algorithm is not available for selected graph.");
            }
            else
            {
                CreateLinesBetweenNodes(CurrentGraph);

                MessageBox.Show("Click on the label near the starting node that you want to use for the algorithm");

            }
        }

        private void Label_Click(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            if (CurrentAlgorithm == AlgorithmType.NONE || CurrentAlgorithm == AlgorithmType.TOPOLOGICAL || CurrentAlgorithm == AlgorithmType.KRUSKAL)
            {
                // do nothing
            }

            else if (CurrentAlgorithm == AlgorithmType.PRIM)
            {
                CreateLinesBetweenNodes(CurrentGraph);

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
            else if (CurrentAlgorithm == AlgorithmType.DIJKSTRA)
            {
                if (SelectedDijkstraNodes.Count == 0)
                {
                    // you haven't yet selected a node

                    CreateLinesBetweenNodes(CurrentGraph);

                    int initialIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                    if (initialIndex >= 0)
                    {
                        Vertex start = CurrentGraph.Vertices[initialIndex];

                        SelectedDijkstraNodes.Add(start);

                        MessageBox.Show("You chose " + label.Text + " as your starting node\nPlease click on another label near the ending node you want to use for the algorithm");
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong, the Vertex couldn't be found");
                    }

                }
                else if (SelectedDijkstraNodes.Count == 1)
                {
                    // you have already selected the starting node

                    int terminalIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                    if (terminalIndex >= 0)
                    {
                        Vertex end = CurrentGraph.Vertices[terminalIndex];

                        SelectedDijkstraNodes.Add(end);

                        try
                        {
                            List<Vertex> output = CurrentGraph.Dijkstra(SelectedDijkstraNodes[0], end, out double shortestDistance);

                            // Draw path one by one using red lines
                            DrawRedLines(output, 500);

                            MessageBox.Show("Shortest distance: " + shortestDistance);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message);
                        }

                        SelectedDijkstraNodes = new List<Vertex>();

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

        }

        private void DrawRedLines(List<Edge> input)
        {
            SetUpGraphicsAndPen(CurrentGraph.IsDirected, out Graphics graphics, out Pen pen, Color.Red);

            foreach (Edge edge in input)
            {
                Point initialLocation = new Point((int)(edge.Start.XCoord * panelGraph.Width), (int)(edge.Start.YCoord * panelGraph.Height));
                Point terminalLocation = new Point((int)(edge.End.XCoord * panelGraph.Width), (int)(edge.End.YCoord * panelGraph.Height));
                graphics.DrawLine(pen, initialLocation, terminalLocation);
            }
        }
        private void DrawRedLines(List<Vertex> input, int sleepTime)
        {
            SetUpGraphicsAndPen(CurrentGraph.IsDirected, out Graphics graphics, out Pen pen, Color.Red);

            Vertex startingVertex, endingVertex;

            for (int i = 0; i < input.Count - 1; i++)
            {
                startingVertex = input[i];
                endingVertex = input[i + 1];

                Point initialLocation = new Point((int)(startingVertex.XCoord * panelGraph.Width), (int)(startingVertex.YCoord * panelGraph.Height));
                Point terminalLocation = new Point((int)(endingVertex.XCoord * panelGraph.Width), (int)(endingVertex.YCoord * panelGraph.Height));
                graphics.DrawLine(pen, initialLocation, terminalLocation);

                System.Threading.Thread.Sleep(sleepTime);
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
    }
}
