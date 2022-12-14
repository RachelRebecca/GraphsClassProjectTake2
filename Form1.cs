using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace GraphsClassProjectTakeTwo
{
    /// <summary>
    /// Form for the Graph Project 
    ///     - Displays Graph Buttons containing the names of all existing graphs in the database
    ///     - Displays a Panel showing the nodes, labels, and graph type of the selected graph
    ///     - Displays graph operation Buttons of things that could be done to the selected graph
    ///         - Dijkstra's Algorithm, Kruskal's and Prim's MST algorithm, Topological Sort
    ///         - Remove edge, Remove node, Add edge 
    ///     - Displays a DataTable showing the selected graph's edges and their corresponding weights
    ///    
    /// Form is split into partial classes - this file contains the constructor and the Graph Buttons methods
    /// Other files are: EdgesWeightsTableForm, GraphOperationsForm, GraphPanelForm, NodeLabelForm
    /// </summary>
    public partial class GraphProject : Form
    {
        // The SQL Connection which gets created in the form and passed to each Graph
        public SqlConnection SqlCon;

        // The names of all the graphs
        public List<string> GraphNames { get; set; }

        /// <summary>
        /// Constructor, makes the SQL connection, initializes the Graph Buttons
        /// </summary>
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
        /// <returns>The open SQL connection</returns>
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
        /// Uses stored procedure to get all the existing graph names
        /// </summary>
        /// <param name="sqlCon">The SQL connection getting the names from the database</param>
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
        private void GraphButton_Click(object sender, EventArgs e)
        {
            ResetPanel();

            Button button = (Button)sender;

            HighlightSelectedGraphButton(button);

            Graph graph = new Graph(button.Name, SqlCon);

            CurrentGraph = graph;

            CurrentGraphOperation = GraphOperations.NONE;

            ShowGraph(graph);

            SetUpGraphOperationButtons(graph);

            CreateEdgesWeightsTable(graph);
        }

        /// <summary>
        /// Highlight the selected graph button
        /// </summary>
        /// <param name="button">The selected graph Button</param>
        private void HighlightSelectedGraphButton(Button button)
        {
            // remove any existing highlights
            if (CurrentGraph != null)
            {
                foreach (Control ctrl in panelGraphButtons.Controls)
                {
                    ctrl.BackColor = Button.DefaultBackColor;
                    ctrl.ForeColor = Button.DefaultForeColor;
                }
            }

            // highlight this button
            button.BackColor = Color.LightPink;
            button.ForeColor = Color.White;
        }

        /// <summary>
        /// A complete reset of panelGraph, Edges-Weights table, and SelectedNodes array
        /// </summary>
        private void ResetForm()
        {
            ShowGraph(CurrentGraph);

            CreateEdgesWeightsTable(CurrentGraph);

            SelectedNodes = new Vertex[] { null, null };
        }
    }
}
