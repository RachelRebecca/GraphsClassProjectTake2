using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GraphsClassProjectTakeTwo
{
    public partial class GraphProject : Form
    {
        // an array of two nodes (a start and end) for the purposes of clicking on two separate nodes and then doing something
        private Vertex[] SelectedNodes = new Vertex[] { null, null };

        /// <summary>
        /// On click label near the nodes, check CurrentGraphOperation and if choosing a node is relevent, do the graph operation requested
        /// </summary>
        /// <param name="sender">The label near the node being selected</param>
        /// <param name="e">The Event Argument</param>
        private void Label_Click(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            switch (CurrentGraphOperation)
            {
                case GraphOperations.DIJKSTRA:
                    DijkstraLabelClick(label);
                    break;
                case GraphOperations.PRIM:
                    PrimLabelClick(label);
                    break;
                case GraphOperations.REMOVE_EDGE:
                    RemoveEdgeLabelClick(label);
                    break;
                case GraphOperations.REMOVE_NODE:
                    RemoveNodeLabelClick(label);
                    break;
                case GraphOperations.ADD_EDGE:
                    AddEdgeLabelClick(label);
                    break;
                default:
                    // do nothing
                    break;
            }
        }

        /// <summary>
        /// On click label near a node after clicking on Dijkstra,
        /// use SelectedNodes to find start and end, do the algorithm
        /// </summary>
        /// <param name="label">The label near selected node</param>
        private void DijkstraLabelClick(Label label)
        {
            Vertex start = SelectedNodes[0];
            Vertex end = SelectedNodes[1];

            if (SelectedNodes[0] == null)
            {
                // you haven't yet selected a node
                string message = "You chose " + label.Text + " as your starting node\n" +
                    "Please click on another label near the ending node you want to use for the algorithm";
                SetStart(label, start, message);

            }
            else if (SelectedNodes[1] == null)
            {
                // you have already selected the starting node

                SetEnd(label, end);

                if (end != null)
                {
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
            }
            else
            {
                // do nothing, we only care about the starting and ending node
            }
        }

        /// <summary>
        /// On click label near a node after clicking on Prim, do the algorithm
        /// </summary>
        /// <param name="label">The label near selected node</param>
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
                }
            }
        }

        /// <summary>
        /// On click label near a node after clicking on RemoveNode,
        /// remove selected node and all its connecting edges
        /// </summary>
        /// <param name="label">The label near selected node</param>
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

        /// <summary>
        /// On click label near a node after clicking on RemoveEdge,
        /// use SelectedNodes to find start and end, remove edge if found
        /// </summary>
        /// <param name="label">The label near selected node</param>
        private void RemoveEdgeLabelClick(Label label)
        {
            Vertex start = SelectedNodes[0];
            Vertex end = SelectedNodes[1];

            if (SelectedNodes[0] == null)
            {
                // you haven't yet selected a node
                string message = "You chose " + label.Text + " as your starting node\n" +
                    "Please click on another label near the ending node of the edge you want to remove";
                SetStart(label, start, message);
            }
            else if (SelectedNodes[1] == null)
            {
                // you have already selected the starting node

                SetEnd(label, end);

                if (end != null)
                {
                    ResetPanel();

                    Edge edge = CurrentGraph.Edges.Find(e => (e.Start.Name.Equals(start.Name) && e.End.Name.Equals(end.Name))
                        || (!CurrentGraph.IsDirected && e.End.Name.Equals(start.Name) && e.Start.Name.Equals(end.Name)));

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

        /// <summary>
        /// On click label near a node after clicking on AddEdge 
        /// Since requires clicking on two labels, check to see which one is start and end using SelectedNodes
        /// </summary>
        /// <param name="label">The label near selected node</param>
        private void AddEdgeLabelClick(Label label)
        {
            Vertex start = SelectedNodes[0];
            Vertex end = SelectedNodes[1];
            if (SelectedNodes[0] == null)
            {
                // you haven't yet selected a node
                string message = "You chose " + label.Text + " as your starting node\nPlease click on another label near the ending node of the edge you want to add";
                SetStart(label, start, message);
            }
            else if (SelectedNodes[1] == null)
            {
                // you have already selected the starting node
                SetEnd(label, end);

                if (end != null)
                {
                    double weight = 1;

                    // prompt the user for the weight of an edge in a weighted graph
                    if (CurrentGraph.IsWeighted)
                    {
                        string promptValue = Prompt.ShowDialog("Enter weight of the new edge", "Add Edge Weight");
                        if (double.TryParse(promptValue, out double result))
                        {
                            weight = result;
                        }
                        else
                        {
                            MessageBox.Show("Couldn't successfully save the weight of the edge, using the default (weight = 1)");
                        }

                    }

                    // add new edge
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

        /// <summary>
        /// Given label near node, set the starting Vertex (found in Vertices), display message to user
        /// </summary>
        /// <param name="label">The label near selected node</param>
        /// <param name="start">The starting Vertex</param>
        /// <param name="message">Message to display to user</param>
        /// <returns>The starting Vertex, which is null if Vertex is not found</returns>
        private Vertex SetStart(Label label, Vertex start, string message)
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

        /// <summary>
        /// Given label near node, sets the ending Vertex (found in Vertices)
        /// </summary>
        /// <param name="label">The label near selected node</param>
        /// <param name="end">The ending Vertex</param>
        private void SetEnd(Label label, Vertex end)
        {
            int terminalIndex = CurrentGraph.Vertices.FindIndex(item => label.Text.Equals(item.Name));

            if (terminalIndex >= 0)
            {
                end = CurrentGraph.Vertices[terminalIndex];

                SelectedNodes[1] = end;
            }
            else
            {
                MessageBox.Show("Something went wrong, the Vertex couldn't be found");
            }
        }
    }
}
