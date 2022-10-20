using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphsClassProjectTakeTwo
{
    public partial class GraphProject : Form
    {
        public SqlConnection sqlCon;

        public List<String> GraphNames { get; set; } // Stored Procedure needs to split into two : one for names, and one that returns flags for each name

        public List<Button> GraphNameButtons { get; set; }

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
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            Graph graph = new Graph(button.Name, sqlCon);

            ShowGraph(graph);

        }

        private void ShowGraph(Graph graph)
        {
            CreateLabelNodes(graph);
            CreateGraphics(graph);
        }

        private void CreateLabelNodes(Graph graph)
        {
            List<Label> labelNodes = new List<Label>();
            for (int nodeNumber = 0; nodeNumber < graph.Vertices.Count; nodeNumber++)
            {
                Label label = new Label();
                label.Text = graph.Vertices[nodeNumber].Name;
                label.TextAlign = ContentAlignment.MiddleCenter;

                Graphics graphics = panelGraph.CreateGraphics();
                Pen pen = new Pen(Color.Black);
                Point location = new Point((int)graph.Vertices[nodeNumber].XCoord * panelGraph.Width, (int)(graph.Vertices[nodeNumber].YCoord * panelGraph.Height));
                graphics.DrawEllipse(pen, location.X - 5, location.Y - 5, 10, 10);

                label.Location = GetNewXAndY(location);
                label.Font = new Font("Arial", 8);
                label.Size = new Size(20, 15);
                label.ForeColor = Color.White;
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
                    xCoord = location.X + 10;
                else
                    xCoord = location.X - 15;
                if (location.Y >= 200)
                    yCoord = location.Y + 15;
                else
                    yCoord = location.Y - 15;
                return new Point(xCoord, yCoord);
            }


        private void CreateGraphics(Graph graph)
        {
            SetUpGraphicsAndPen(out Graphics graphics, out Pen pen, Color.Black);

            foreach (Edge edge in graph.Edges)
            {
                pen.Color = Color.Black;

                Point initialLocation = new Point((int)edge.Start.XCoord, (int)edge.Start.YCoord);
                Point terminalLocation = new Point((int)edge.End.XCoord, (int)edge.End.YCoord);
                graphics.DrawLine(pen, initialLocation, terminalLocation);
            }

            panelGraph.Refresh();
        }

        private void SetUpGraphicsAndPen(out Graphics graphics, out Pen pen, Color penColor)
        {
            graphics = panelGraph.CreateGraphics();
            pen = new Pen(penColor);
            AdjustableArrowCap adjustableArrowCap = new AdjustableArrowCap(3, 3);
            pen.CustomEndCap = adjustableArrowCap;
        }

        /*
                private void FillPanel(ParentGraph graph)
                {
                    ResetPanels();

                    CreateLabelType(graph);

                    CreateLabelNodes(graph);

                    CreateGraphics(graph);
                }

                 private void ResetPanels()
                {
                    LabelNodes = new List<Label>();
                    NodeCircleLocations = new List<Point>();
                    panelGraph.Controls.Clear();
                    panelGraph.Refresh();
                    ResetNodeSelectionPanel();
                }

                private void ResetNodeSelectionPanel()
                {
                    panelNodeSelection.Visible = false;
                    originDropDown.SelectedIndex = -1;
                    destDropDown.SelectedIndex = -1;
                    originDropDown.Items.Clear();
                    destDropDown.Items.Clear();
                    originDropDown.ResetText();
                    destDropDown.ResetText();
                }

          private void CreateLabelType(ParentGraph graph)
        {
            Label labelGraphType = new Label();
            labelGraphType.Location = new Point(15, 20);

            String type = "";
            switch (graph.Type)
            {
                case GraphType.WEIGHTED_DIGRAPH:
                    type = "Weighted Digraph";
                    break;
                case GraphType.DIGRAPH:
                    type = "Digraph";
                    break;
                case GraphType.WEIGHTED_GRAPH:
                    type = "Weighted Graph";
                    break;
                case GraphType.GRAPH:
                    type = "Graph";
                    break;
            }

            labelGraphType.Text = type;
            labelGraphType.Refresh();
            panelGraph.Controls.Add(labelGraphType);
        }


        */
    }
}
