using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GraphsClassProjectTakeTwo
{
    public partial class GraphProject : Form
    {
        //enum of all the things that can be done to a given graph
        public enum GraphOptions
        {
            PRIM, DIJKSTRA, TOPOLOGICAL, KRUSKAL, ADD_EDGE, REMOVE_EDGE, REMOVE_NODE, NONE
        }

        // ToolTip for the graph operation buttons
        private readonly ToolTip ToolTip = new ToolTip();

        // the current Graph showing in the main panel
        private Graph CurrentGraph = null;

        // the current Graph Operation being done to the graph
        private GraphOptions CurrentGraphOperation = GraphOptions.NONE;

        // an array of two nodes (a start and end) for the purposes of clicking on two separate nodes and then doing something
        private Vertex[] SelectedNodes = new Vertex[] { null, null };
        
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
        /// <param name="sender">the control firing this event</param>
        /// <param name="e">the MouseEventArgument, used to determine location of mouse</param>
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
            SetUpForGraphOperation(GraphOptions.DIJKSTRA);

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
            SetUpForGraphOperation(GraphOptions.KRUSKAL);

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
            SetUpForGraphOperation(GraphOptions.TOPOLOGICAL);

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
            SetUpForGraphOperation(GraphOptions.PRIM);

            if (CurrentGraph != null && !CurrentGraph.IsDirected)
            {
                MessageBox.Show(!CurrentGraph.IsConnected()
                    ? "Graph is either empty or not connected."
                    : "Click on the label near the node that you want to use for the algorithm");
            }
        }

        private void RemoveEdge_Click(object sender, EventArgs e)
        {
            CurrentGraphOperation = GraphOptions.REMOVE_EDGE;

            DoGraphOperation("starting node of the edge that you want to remove\nOr click on the edge in the Edges-Weights table");

        }

        private void RemoveNode_Click(object sender, EventArgs e)
        {
            CurrentGraphOperation = GraphOptions.REMOVE_NODE;

            DoGraphOperation("node that you want to remove");
        }

        private void AddEdge_Click(object sender, EventArgs e)
        {
            CurrentGraphOperation = GraphOptions.ADD_EDGE;

            DoGraphOperation("starting node that you want to add an edge to");

        }

        private void SetUpForGraphOperation(GraphOptions operation)
        {
            if (CurrentGraph != null)
            {
                CurrentGraphOperation = operation;

                SelectedNodes = new Vertex[] { null, null };

                CreateLinesBetweenNodes(CurrentGraph);

                ResetTableWeightsSelected();
            }
        }

        private void DoGraphOperation(string operation)
        {
            if (CurrentGraph != null)
            {
                CreateLinesBetweenNodes(CurrentGraph);

                SelectedNodes = new Vertex[] { null, null };

                ResetTableWeightsSelected();

                MessageBox.Show(CurrentGraph.Vertices.Count == 0 ? "The graph is empty" : "Click on the label near the " + operation);
            }
        }

        /// <summary>
        /// On click label near the nodes, check CurrentGraphOperation and if choosing a node is relevent, do the graph operation requested
        /// </summary>
        /// <param name="sender">The label near the node being selected</param>
        /// <param name="e">The Event Argument</param>
        private void Label_Click(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            if (CurrentGraphOperation == GraphOptions.PRIM)
            {
                PrimLabelClick(label);

            }
            else if (CurrentGraphOperation == GraphOptions.DIJKSTRA)
            {
                DijkstraLabelClick(label);

            }
            else if (CurrentGraphOperation == GraphOptions.REMOVE_NODE)
            {
                RemoveNodeLabelClick(label);
            }
            else if (CurrentGraphOperation == GraphOptions.REMOVE_EDGE)
            {
                RemoveEdgeLabelClick(label);
            }
            else if (CurrentGraphOperation == GraphOptions.ADD_EDGE)
            {
                AddEdgeLabelClick(label);
            }
        }

        private void AddEdgeLabelClick(Label label)
        {
            Vertex start = SelectedNodes[0];
            Vertex end = SelectedNodes[1];
            if (SelectedNodes[0] == null)
            {
                string message = "You chose " + label.Text + " as your starting node\nPlease click on another label near the ending node of the edge you want to add";
                start = GetStart(label, start, message);

            }
            else if (SelectedNodes[1] == null)
            {
                // you have already selected the starting node

                int terminalIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                if (terminalIndex >= 0)
                {
                    end = CurrentGraph.Vertices[terminalIndex];

                    SelectedNodes[1] = end;

                    // TODO: user input to select weight
                    double weight = 1;
                    if (CurrentGraph.IsWeighted)
                    {
                        string promptValue = Prompt.ShowDialog("Enter weight of the new edge", "Add Edge Weight");
                        if (Double.TryParse(promptValue, out double result))
                        {
                            weight = result;
                        }
                        else
                        {
                            MessageBox.Show("Couldn't successfully save the weight of the edge, using the default (weight = 1)");
                        }

                    }

                    ResetPanel();

                    Edge edge = new Edge(start, end, weight);
                    start.Outdegree++;
                    end.Indegree++;
                    CurrentGraph.Edges.Add(edge);
                    end.Edges.Add(edge);


                    ResetForm();
                }
            }
        }

        private void RemoveEdgeLabelClick(Label label)
        {
            Vertex start = SelectedNodes[0];
            Vertex end = SelectedNodes[1];
            if (SelectedNodes[0] == null)
            {
                string message = "You chose " + label.Text + " as your starting node\nPlease click on another label near the ending node of the edge you want to remove";

                GetStart(label, start, message);
            }
            else if (SelectedNodes[1] == null)
            {
                // you have already selected the starting node

                int terminalIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                if (terminalIndex >= 0)
                {
                    ResetPanel();

                    end = CurrentGraph.Vertices[terminalIndex];

                    SelectedNodes[1] = end;

                    Edge edge = CurrentGraph.Edges.Find(edg => (edg.Start.Name.Equals(start.Name) && edg.End.Name.Equals(end.Name))
                    || (!CurrentGraph.IsDirected && edg.End.Name.Equals(start.Name) && edg.Start.Name.Equals(end.Name)));

                    if (edge != null)
                    {
                        edge.Start.Outdegree--;
                        edge.End.Indegree--;
                        edge.End.Edges.Remove(edge);
                        CurrentGraph.Edges.Remove(edge);
                    }
                    ResetForm();
                }
            }
        }

        private void RemoveNodeLabelClick(Label label)
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

                        edge.End.Edges.Remove(edge);
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

        private void DijkstraLabelClick(Label label)
        {
            Vertex start = SelectedNodes[0];
            Vertex end = SelectedNodes[1];

            if (SelectedNodes[0] == null)
            {
                // you haven't yet selected a node

                string message = "You chose " + label.Text + " as your starting node\nPlease click on another label near the ending node you want to use for the algorithm";

                GetStart(label, start, message);

            }
            else if (SelectedNodes[1] == null)
            {
                // you have already selected the starting node

                int terminalIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

                if (terminalIndex >= 0)
                {
                    end = CurrentGraph.Vertices[terminalIndex];

                    SelectedNodes[1] = end;

                    try
                    {
                        List<Vertex> output = CurrentGraph.Dijkstra(start, end, out double shortestDistance);

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

        private Vertex GetStart(Label label, Vertex start, string message)
        {
            CreateLinesBetweenNodes(CurrentGraph);

            ResetTableWeightsSelected();

            int initialIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

            if (initialIndex >= 0)
            {
                start = CurrentGraph.Vertices[initialIndex];

                SelectedNodes[0] = start;

                MessageBox.Show(message);
            }

            else
            {
                MessageBox.Show("Something went wrong, the Vertex couldn't be found");
            }

            return start;
        }

        private void PrimLabelClick(Label label)
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

    }
}
