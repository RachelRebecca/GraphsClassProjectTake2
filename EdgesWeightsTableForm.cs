using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace GraphsClassProjectTakeTwo
{
    /// <summary>
    /// Partial class of the Form - this file contains the Edges-Weights table related methods
    /// Other files are: Form1, GraphOperationsForm, GraphPanelForm, NodeLabelForm
    /// </summary>
    public partial class GraphProject : Form
    {
        /// <summary>
        /// Creates the Edges-Weights table for given Graph
        /// </summary>
        /// <param name="graph">The given Graph</param>
        private void CreateEdgesWeightsTable(Graph graph)
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
            tableEdgesWeights.Refresh();
        }

        /// <summary>
        /// Highlights the selected edge in the panelGraph 
        /// Removes selected edge if CurrentGraphOperation is GraphOperations.REMOVE_EDGE
        /// </summary>
        /// <param name="sender">The DataGridView firing the method</param>
        /// <param name="evt">The DataGridView Cell Event Argument</param>
        private void TableWeights_CellClick(object sender, DataGridViewCellEventArgs evt)
        {
            CreateLinesBetweenNodes(CurrentGraph);

            DataGridView dgv = (DataGridView)sender;
            if (dgv != null && dgv.Rows.Count > 0 && dgv.CurrentRow.Selected)
            {
                // find the edge in the table
                DataTable table = (DataTable)dgv.DataSource;
                string edgeInTable = (string)table.Rows[dgv.CurrentRow.Index]["Edges"];
                Edge edge = CurrentGraph.Edges.Find(e => (e.Start.Name + e.End.Name).Equals(edgeInTable) || (e.End.Name + e.Start.Name).Equals(edgeInTable));

                if (edge != null)
                {
                    if (CurrentGraphOperation == GraphOperations.REMOVE_EDGE)
                    {
                        RemoveEdgeTableClick(edge);
                    }
                    else
                    {
                        SetUpGraphicsAndPen(CurrentGraph.IsDirected, out Graphics graphics, out Pen pen, Color.Yellow);
                        pen.Width = 1;

                        Point initialLocation = new Point((int)(edge.Start.XCoord * panelGraph.Width), (int)(edge.Start.YCoord * panelGraph.Height));
                        Point terminalLocation = new Point((int)(edge.End.XCoord * panelGraph.Width), (int)(edge.End.YCoord * panelGraph.Height));
                        graphics.DrawLine(pen, initialLocation, terminalLocation);

                    }
                }
                else
                {
                    MessageBox.Show("Something went wrong, edge couldn't be found.");
                }
            }

            ResetEdgesWeightsTable();
        }

        /// <summary>
        /// Clicking on edge in Edges-Weights table after clicking on RemoveEdge, remove selected edge
        /// </summary>
        /// <param name="edge">Selected edge</param>
        private void RemoveEdgeTableClick(Edge edge)
        {
            ResetPanel();

            RemoveSelectedEdge(edge);

            ResetForm();
        }

        /// <summary>
        /// Reset the Edges-Weights table so that none are selected
        /// </summary>
        private void ResetEdgesWeightsTable()
        {
            foreach (DataGridViewRow row in tableEdgesWeights.Rows)
            {
                row.Selected = false;
            }
        }

        /// <summary>
        /// Highlight selected edges in the Edges-Weights table
        /// </summary>
        /// <param name="edgeNames">The selected edges</param>
        private void HighlightSelectedEdgesInTable(List<string> edgeNames)
        {
            DataTable table = (DataTable)tableEdgesWeights.DataSource;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (edgeNames.Contains((string)table.Rows[i]["Edges"]))
                {
                    tableEdgesWeights.Rows[i].Selected = true;
                }
            }
        }
    }
}
