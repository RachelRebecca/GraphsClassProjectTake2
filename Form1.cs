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
using System.Windows.Forms.VisualStyles;

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
            panelGraphButtons.Refresh();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            panelGraph.Controls.Clear();

            Button button = (Button)sender;

            Graph graph = new Graph(button.Name, sqlCon);

            ShowGraph(graph);

        }

        private void ShowGraph(Graph graph)
        {
            CreateLabelType(graph);
            CreateLabelNodes(graph);
            CreateGraphics(graph);
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
            labelGraphType.BringToFront();
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
                label.BackColor = Color.Black;
                label.Visible = true;
                label.SendToBack();
                label.Refresh();

                labelNodes.Add(label);
            }

            foreach (Label label in labelNodes)
            {
                panelGraph.Controls.Add(label);
                label.Refresh();
            }
            panelGraph.Refresh();
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
            SetUpGraphicsAndPen(out Graphics graphics, out Pen pen, Color.Black);

            foreach (Edge edge in graph.Edges)
            {
                pen.Color = Color.Black;
                pen.Width = 3;

                Point initialLocation = new Point((int)edge.Start.XCoord, (int)edge.Start.YCoord);
                Point terminalLocation = new Point((int)edge.End.XCoord, (int)edge.End.YCoord);
                graphics.DrawLine(pen, initialLocation, terminalLocation);
            }
        }

        private void SetUpGraphicsAndPen(out Graphics graphics, out Pen pen, Color penColor)
        {
            graphics = panelGraph.CreateGraphics();
            pen = new Pen(penColor);
            AdjustableArrowCap adjustableArrowCap = new AdjustableArrowCap(3, 3);
            pen.CustomEndCap = adjustableArrowCap;
        }
    }
}
