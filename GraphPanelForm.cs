using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GraphsClassProjectTakeTwo
{
    public partial class GraphProject : Form
    {
        /// <summary>
        /// Shows the graph inside the panelGraph
        /// </summary>
        /// <param name="graph">The Graph to be displayed inside panelGraph</param>
        private void ShowGraph(Graph graph)
        {
            CreateLabelGraphType(graph);
            CreateLabelNodes(graph);
            CreateGraphicsNodes(graph);
            CreateLinesBetweenNodes(graph);
        }

        /// <summary>
        /// Adds a label containing the Graph Type to the top left corner of panelGraph
        /// </summary>
        /// <param name="graph">The current graph being displayed</param>
        private void CreateLabelGraphType(Graph graph)
        {
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
            Label labelGraphType = new Label
            {
                Location = new Point(15, 20),
                Text = type,
                Size = new Size(150, 15)
            };
            labelGraphType.Refresh();
            panelGraph.Controls.Add(labelGraphType);
        }

        /// <summary>
        /// Create the labels near the nodes for the given Graph
        /// </summary>
        /// <param name="graph">The given Graph</param>
        private void CreateLabelNodes(Graph graph)
        {
            List<Label> labelNodes = new List<Label>();
            for (int nodeNumber = 0; nodeNumber < graph.Vertices.Count; nodeNumber++)
            {
                Point location = new Point((int)(graph.Vertices[nodeNumber].XCoord * panelGraph.Width), (int)(graph.Vertices[nodeNumber].YCoord * panelGraph.Height));

                Label label = new Label
                {
                    Text = graph.Vertices[nodeNumber].Name,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = GetNewXAndY(location),
                    Font = new Font("Arial", 8),
                    Size = new Size(20, 15),
                    ForeColor = Color.White,
                    Visible = true
                };
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

        /// <summary>
        /// Create the nodes for the given Graph using Graphics
        /// </summary>
        /// <param name="graph">The given Graph</param>
        private void CreateGraphicsNodes(Graph graph)
        {
            for (int nodeNumber = 0; nodeNumber < graph.Vertices.Count; nodeNumber++)
            {
                Graphics graphics = panelGraph.CreateGraphics();
                Pen pen = new Pen(Color.Black);
                Point location = new Point((int)(graph.Vertices[nodeNumber].XCoord * panelGraph.Width), (int)(graph.Vertices[nodeNumber].YCoord * panelGraph.Height));
                graphics.DrawEllipse(pen, location.X - 5, location.Y - 5, 10, 10);
            }
        }

        /// <summary>
        /// Get the labelNode's X and Y coordinates 
        /// close enough to the graphics node that it is clear they are related,
        /// but far enough that it is not sitting on top of the Graphics node itself
        /// </summary>
        /// <param name="location">The location of the Graphics node</param>
        /// <returns></returns>
        private Point GetNewXAndY(Point location)
        {
            int xCoord;
            int yCoord;

            if (location.X >= 200)
                xCoord = Math.Min(location.X + 10, panelGraph.Width);
            else
                xCoord = Math.Max(location.X - 15, 0);
            if (location.Y >= 200)
                yCoord = Math.Min(location.Y + 10, panelGraph.Height);
            else
                yCoord = Math.Max(location.Y - 15, 0);
            return new Point(xCoord, yCoord);
        }

        /// <summary>
        /// Create the lines between all edges using Graphics for a given Graph using its Edges List
        /// </summary>
        /// <param name="graph">The given Graph</param>
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

        /// <summary>
        /// Given a list of edges, highlight them in Red
        /// </summary>
        /// <param name="edges">The list of edges</param>
        private void DrawRedLines(List<Edge> edges)
        {
            SetUpGraphicsAndPen(CurrentGraph.IsDirected, out Graphics graphics, out Pen pen, Color.Red);

            List<string> edgeNames = new List<string>();

            foreach (Edge edge in edges)
            {
                edgeNames.Add(edge.Start.Name + edge.End.Name);
                Point initialLocation = new Point((int)(edge.Start.XCoord * panelGraph.Width), (int)(edge.Start.YCoord * panelGraph.Height));
                Point terminalLocation = new Point((int)(edge.End.XCoord * panelGraph.Width), (int)(edge.End.YCoord * panelGraph.Height));
                graphics.DrawLine(pen, initialLocation, terminalLocation);
            }

            HighlightSelectedEdgesInTable(edgeNames);
        }

        /// <summary>
        /// Given a list of vertices, highlights them in Red one by one
        /// </summary>
        /// <param name="vertices">The list of vertices</param>
        /// <param name="sleepTime">The length of time for the animation (in ms)</param>
        private void DrawRedLines(List<Vertex> vertices, int sleepTime)
        {
            SetUpGraphicsAndPen(CurrentGraph.IsDirected, out Graphics graphics, out Pen pen, Color.Red);

            Vertex startingVertex, endingVertex;

            List<string> edgeNames = new List<string>();

            // go from start to second to last
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                // start = vertices at current index, end = the next vertex in vertices
                startingVertex = vertices[i];
                endingVertex = vertices[i + 1];

                edgeNames.Add(startingVertex.Name + endingVertex.Name);

                Point initialLocation = new Point((int)(startingVertex.XCoord * panelGraph.Width), (int)(startingVertex.YCoord * panelGraph.Height));
                Point terminalLocation = new Point((int)(endingVertex.XCoord * panelGraph.Width), (int)(endingVertex.YCoord * panelGraph.Height));
                graphics.DrawLine(pen, initialLocation, terminalLocation);

                System.Threading.Thread.Sleep(sleepTime);
            }

            HighlightSelectedEdgesInTable(edgeNames);
        }

        /// <summary>
        /// Sets up the Graphics and Pen given a pen color
        /// </summary>
        /// <param name="useArrowCap">If using arrow caps (if IsDirected) or not</param>
        /// <param name="graphics">out parameter for the Graphics</param>
        /// <param name="pen">out parameter for the Pen</param>
        /// <param name="penColor">The color of the Pen</param>
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

        /// <summary>
        /// Deletes all controls and Graphics in the panel 
        /// </summary>
        private void ResetPanel()
        {
            panelGraph.Controls.Clear();

            panelGraph.Refresh();

            panelGraph.BackColor = Color.Gray;
        }
    }
}
