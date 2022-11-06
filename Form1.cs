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
        // the SQL Connection which gets created in the form and passed to each Graph
        public SqlConnection SqlCon;

        // the names of all the graphs
        public List<string> GraphNames { get; set; }

        public GraphProject()
        {
            InitializeComponent();

            SqlCon = MakeSQLConnection();

            GraphNames = GetGraphNames(SqlCon);

            SetUpGraphNameButtons();
        }

        /// <summary>
        /// Make the SQL connection using App.config and open it
        /// </summary>
        /// <returns>the open SQL connection</returns>
        public SqlConnection MakeSQLConnection()
        {
            var server = ConfigurationManager.AppSettings["SERVER"];
            var database = ConfigurationManager.AppSettings["DATABASE"];
            string strConnect = $"Server={server};Database={database};Trusted_Connection=True;";
            SqlConnection sqlCon = new SqlConnection(strConnect);
            sqlCon.Open();

            return sqlCon;

        }

        /// <summary>
        /// Use stored procedure to get all the existing graph names from the database
        /// </summary>
        /// <param name="sqlCon"></param>
        /// <returns>List containing all the graph names</returns>
        private List<string> GetGraphNames(SqlConnection sqlCon)
        {
            SqlCommand getAllGraphs = new SqlCommand("spGetGraphNames", sqlCon)
            {
                CommandType = CommandType.StoredProcedure
            };
            getAllGraphs.ExecuteNonQuery();
            SqlDataAdapter da1 = new SqlDataAdapter(getAllGraphs);
            DataSet dataset1 = new DataSet();
            da1.Fill(dataset1, "Graphs");


            List<string> GraphNames = new List<string>();

            var nrGraphs = dataset1.Tables["Graphs"].Rows.Count;

            for (int row = 0; row < nrGraphs; ++row)
            {
                GraphNames.Add((string)dataset1.Tables["Graphs"].Rows[row].ItemArray[0]);
            }

            return GraphNames;
        }

        /// <summary>
        /// Creates a new Button for every graph
        /// </summary>
        private void SetUpGraphNameButtons()
        {
            int x = 30;
            int y = 0;
            foreach (string name in GraphNames)
            {
                Button button = new Button
                {
                    Name = name, // All button names are unique because in the SQL code, graph names are unique
                    Text = name
                };
                button.Click += new EventHandler(GraphButton_Click);
                button.Location = new Point(x, y);

                y += 100;

                panelGraphButtons.Controls.Add(button);
            }
            panelGraphButtons.Refresh();
        }

        /// <summary>
        /// All the methods done when user clicks on a graph
        /// </summary>
        /// <param name="sender">The graph button being clicked on</param>
        /// <param name="e">The event argument</param>
        private void GraphButton_Click(object sender, EventArgs e)
        {
            ResetPanel();

            Button button = (Button)sender;

            HighlightSelectedGraph(button);

            Graph graph = new Graph(button.Name, SqlCon);

            CurrentGraph = graph;

            CurrentGraphOperation = GraphOptions.NONE;

            ShowGraph(graph);

            SetUpGraphOperationButtons(graph);

            ShowWeights(graph);
        }

        private void HighlightSelectedGraph(Button button)
        {
            if (CurrentGraph != null)
            {
                foreach (Control ctrl in panelGraphButtons.Controls)
                {
                    ctrl.BackColor = Button.DefaultBackColor;
                    ctrl.ForeColor = Button.DefaultForeColor;
                }
            }

            button.BackColor = Color.LightPink;
            button.ForeColor = Color.White;
        }

        private void ShowWeights(Graph graph)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Edges", typeof(string));
            table.Columns.Add("Weights", typeof(double));

            foreach (Edge edge in graph.Edges)
            {
                string edgeName = edge.Start.Name + edge.End.Name;
                double weight = edge.Weight;
                table.Rows.Add(new object[] { edgeName, weight });
            }

            tableEdgesWeights.DataSource = table;
        }

        private void TableWeights_CellClick(object sender, DataGridViewCellEventArgs evt)
        {
            CreateLinesBetweenNodes(CurrentGraph);

            DataGridView dgv = (DataGridView)sender;
            if (dgv != null && dgv.Rows.Count > 0 && dgv.CurrentRow.Selected)
            {
                DataTable table = (DataTable)dgv.DataSource;
                string edgeInTable = (string)table.Rows[dgv.CurrentRow.Index]["Edges"];
                Edge edge = CurrentGraph.Edges.Find(e => (e.Start.Name + e.End.Name).Equals(edgeInTable) || (e.End.Name + e.Start.Name).Equals(edgeInTable));

                if (CurrentGraphOperation == GraphOptions.REMOVE_EDGE)
                {
                    RemoveEdgeTableClick(edge);
                }
                else
                {
                    if (edge != null)
                    {
                        SetUpGraphicsAndPen(CurrentGraph.IsDirected, out Graphics graphics, out Pen pen, Color.Yellow);
                        pen.Width = 1;

                        Point initialLocation = new Point((int)(edge.Start.XCoord * panelGraph.Width), (int)(edge.Start.YCoord * panelGraph.Height));
                        Point terminalLocation = new Point((int)(edge.End.XCoord * panelGraph.Width), (int)(edge.End.YCoord * panelGraph.Height));
                        graphics.DrawLine(pen, initialLocation, terminalLocation);
                    }
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
            Label labelGraphType = new Label
            {
                Location = new Point(15, 20)
            };

            string type = "";
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
                Label label = new Label
                {
                    Text = graph.Vertices[nodeNumber].Name,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                Graphics graphics = panelGraph.CreateGraphics();
                Pen pen = new Pen(Color.Black);
                Point location = new Point((int)(graph.Vertices[nodeNumber].XCoord * panelGraph.Width), (int)(graph.Vertices[nodeNumber].YCoord * panelGraph.Height));
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

        private void RemoveEdgeTableClick(Edge edge)
        {
            if (edge != null)
            {
                ResetPanel();

                edge.Start.Outdegree--;
                edge.End.Indegree--;
                edge.End.Edges.Remove(edge);
                CurrentGraph.Edges.Remove(edge);

                ResetForm();
            }
            else
            {
                MessageBox.Show("Something went wrong, edge couldn't be found.");
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
                if (edgeNames.Contains((string)table.Rows[i]["Edges"]))
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
                graphics.DrawLine(pen, initialLocation, terminalLocation);

                System.Threading.Thread.Sleep(sleepTime);
            }

            DataTable table = (DataTable)tableEdgesWeights.DataSource;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (edgeNames.Contains((string)table.Rows[i]["Edges"]))
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
    }
}
