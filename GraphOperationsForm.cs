using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GraphsClassProjectTakeTwo
{
    /// <summary>
    /// Partial class of the Form - this file contains the Graph Operations related methods
    /// Other files are: Form1, EdgesWeightsTableForm, GraphPanelForm, NodeLabelForm
    /// </summary>
    public partial class GraphProject : Form
    {
        //enum of all the things that can be done to a given graph
        public enum GraphOperations { PRIM, DIJKSTRA, TOPOLOGICAL, KRUSKAL, ADD_EDGE, REMOVE_EDGE, REMOVE_NODE, NONE }

        // ToolTip for the graph operation buttons
        private readonly ToolTip ToolTip = new ToolTip();

        // The current Graph showing in the main panel
        private Graph CurrentGraph = null;

        // The current Graph Operation being done to the graph
        private GraphOperations CurrentGraphOperation = GraphOperations.NONE;
  
        /// <summary>
        /// Sets the graph operation buttons to enabled or disabled based on given graph
        /// </summary>
        /// <param name="graph">The given graph determining if buttons should be enabled or disabled</param>
        private void SetUpGraphOperationButtons(Graph graph)
        {
            Topological.Enabled = true;
            Kruskal.Enabled = true;
            Prim.Enabled = true;
            Dijkstra.Enabled = true;
            RemoveEdge.Enabled = true;
            RemoveNode.Enabled = true;
            AddEdge.Enabled = true;

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

        /// <summary>
        /// When mouse moves over panelGraphOperations, determines if hovering over a graph operation button
        /// If yes, checks if button is disabled and displays a ToolTip explaining why it's disabled  
        /// </summary>
        /// <param name="sender">The control firing this event</param>
        /// <param name="e">The MouseEventArgument, used to determine location of mouse</param>
        private void GraphOperations_MouseMove(object sender, MouseEventArgs e)
        {
            Control ctrl = ((Panel)sender).GetChildAtPoint(e.Location);
            List<Control> graphOperations = new List<Control>() { RemoveEdge, AddEdge, RemoveNode, Dijkstra, Prim, Kruskal, Topological };
            if (ctrl != null && graphOperations.Contains(ctrl) && !ctrl.Enabled)
            {
                ToolTip.SetToolTip(ctrl, CurrentGraph == null
                    ? "There is no graph being displayed yet"
                    : "This algorithm is unavailable for selected graph");
                ToolTip.Show(ToolTip.GetToolTip(ctrl), ctrl, ctrl.Width / 2, ctrl.Height / 2);
            }
            else
            {
                ToolTip.Hide(this);
            }
        }

        /// <summary>
        /// On click Dijkstra, tell user to choose starting label for Dijkstra's Algorithm
        /// </summary>
        private void Dijkstra_Click(object sender, EventArgs e)
        {
            SetUpForGraphOperation(GraphOperations.DIJKSTRA);

            if (CurrentGraph != null && CurrentGraph.IsWeighted)
            {
                MessageBox.Show(CurrentGraph.Vertices.Count == 0 
                    ? "Graph is empty" 
                    : "Click on the label near the starting node that you want to use for the algorithm");
            }
        }

        /// <summary>
        /// On click Kruskal button, do Kruskal algorithm
        /// </summary>
        private void Kruskal_Click(object sender, EventArgs e)
        {
            SetUpForGraphOperation(GraphOperations.KRUSKAL);

            if (CurrentGraph != null && !CurrentGraph.IsDirected)
            {
                if (!CurrentGraph.IsConnected())
                {
                    MessageBox.Show("Graph is either empty or not connected");
                }
                else
                {
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
        }

        /// <summary>
        /// On click Topological button, do topological sort
        /// </summary>
        private void Topological_Click(object sender, EventArgs e)
        {
            SetUpForGraphOperation(GraphOperations.TOPOLOGICAL);

            if (CurrentGraph != null && CurrentGraph.IsDirected)
            {
                if (CurrentGraph.Vertices.Count == 0)
                {
                    MessageBox.Show("Graph is empty");
                }
                else
                {
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
        }

        /// <summary>
        /// On click Prim, tell user to choose starting label for Prim Algorithm
        /// </summary>
        private void Prim_Click(object sender, EventArgs e)
        {
            SetUpForGraphOperation(GraphOperations.PRIM);

            if (CurrentGraph != null && !CurrentGraph.IsDirected)
            {
                MessageBox.Show(!CurrentGraph.IsConnected()
                    ? "Graph is either empty or not connected."
                    : "Click on the label near the node that you want to use for the algorithm");
            }
        }

        /// <summary>
        /// On click RemoveEdge, tell user to choose starting label of edge to be removed or click the edge from the Edges-Weights table"
        /// </summary>
        private void RemoveEdge_Click(object sender, EventArgs e)
        {
            SetUpForGraphOperation(GraphOperations.REMOVE_EDGE);

            MessageBox.Show(CurrentGraph.Vertices.Count == 0
                ? "The graph is empty" 
                : "Click on the label near the starting node of the edge that you want to remove\n" +
                "Or click on the edge in the Edges-Weights table");

        }

        /// <summary>
        /// On click RemoveNode, tell user to choose label of node to be removed"
        /// </summary>
        private void RemoveNode_Click(object sender, EventArgs e)
        {
            SetUpForGraphOperation(GraphOperations.REMOVE_NODE);

            MessageBox.Show(CurrentGraph.Vertices.Count == 0 
                ? "The graph is empty" 
                : "Click on the label near the node that you want to remove");
        }

        /// <summary>
        /// On click AddEdge, tell user to choose starting label of edge to be added"
        /// </summary>
        private void AddEdge_Click(object sender, EventArgs e)
        {
            SetUpForGraphOperation(GraphOperations.ADD_EDGE);

            MessageBox.Show(CurrentGraph.Vertices.Count == 0 
                ? "The graph is empty" 
                : "Click on the label near the starting node that you want to add an edge to");
        }

        /// <summary>
        /// Sets up the Graph Operation and does some resetting
        /// </summary>
        /// <param name="operation">The current graph operation</param>
        private void SetUpForGraphOperation(GraphOperations operation)
        {
            if (CurrentGraph != null)
            {
                CurrentGraphOperation = operation;

                SelectedNodes = new Vertex[] { null, null };

                CreateLinesBetweenNodes(CurrentGraph);

                ResetEdgesWeightsTable();
            }
        }
    }
}