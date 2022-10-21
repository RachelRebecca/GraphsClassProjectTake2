namespace GraphsClassProjectTakeTwo
{
    partial class GraphProject
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelGraphButtons = new System.Windows.Forms.Panel();
            this.panelGraph = new System.Windows.Forms.Panel();
            this.Topological = new System.Windows.Forms.Button();
            this.Prim = new System.Windows.Forms.Button();
            this.Dijkstra = new System.Windows.Forms.Button();
            this.Kruskal = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableWeights = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelGraphButtons
            // 
            this.panelGraphButtons.AutoScroll = true;
            this.panelGraphButtons.Location = new System.Drawing.Point(634, 12);
            this.panelGraphButtons.Name = "panelGraphButtons";
            this.panelGraphButtons.Size = new System.Drawing.Size(160, 330);
            this.panelGraphButtons.TabIndex = 0;
            // 
            // panelGraph
            // 
            this.panelGraph.BackColor = System.Drawing.Color.Gray;
            this.panelGraph.Location = new System.Drawing.Point(12, 12);
            this.panelGraph.Name = "panelGraph";
            this.panelGraph.Size = new System.Drawing.Size(600, 550);
            this.panelGraph.TabIndex = 0;
            // 
            // Topological
            // 
            this.Topological.Location = new System.Drawing.Point(343, 574);
            this.Topological.Name = "Topological";
            this.Topological.Size = new System.Drawing.Size(104, 35);
            this.Topological.TabIndex = 1;
            this.Topological.Text = "Topological Sort";
            this.Topological.UseVisualStyleBackColor = true;
            this.Topological.Click += new System.EventHandler(this.Topological_Click);
            // 
            // Prim
            // 
            this.Prim.Location = new System.Drawing.Point(495, 574);
            this.Prim.Name = "Prim";
            this.Prim.Size = new System.Drawing.Size(104, 35);
            this.Prim.TabIndex = 2;
            this.Prim.Text = "Prim\'s Algorithm";
            this.Prim.UseVisualStyleBackColor = true;
            this.Prim.Click += new System.EventHandler(this.Prim_Click);
            // 
            // Dijkstra
            // 
            this.Dijkstra.Location = new System.Drawing.Point(23, 574);
            this.Dijkstra.Name = "Dijkstra";
            this.Dijkstra.Size = new System.Drawing.Size(104, 35);
            this.Dijkstra.TabIndex = 1;
            this.Dijkstra.Text = "Dijkstra\'s Algorithm";
            this.Dijkstra.UseVisualStyleBackColor = true;
            this.Dijkstra.Click += new System.EventHandler(this.Dijkstra_Click);
            // 
            // Kruskal
            // 
            this.Kruskal.Location = new System.Drawing.Point(188, 574);
            this.Kruskal.Name = "Kruskal";
            this.Kruskal.Size = new System.Drawing.Size(104, 35);
            this.Kruskal.TabIndex = 0;
            this.Kruskal.Text = "Kruskal\'s Algorithm";
            this.Kruskal.UseVisualStyleBackColor = true;
            this.Kruskal.Click += new System.EventHandler(this.Kruskal_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.tableWeights);
            this.panel1.Location = new System.Drawing.Point(634, 358);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(160, 251);
            this.panel1.TabIndex = 3;
            // 
            // tableWeights
            // 
            this.tableWeights.ColumnCount = 2;
            this.tableWeights.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableWeights.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableWeights.Location = new System.Drawing.Point(0, 0);
            this.tableWeights.Name = "tableWeights";
            this.tableWeights.RowCount = 2;
            this.tableWeights.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableWeights.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableWeights.Size = new System.Drawing.Size(159, 250);
            this.tableWeights.TabIndex = 0;
            // 
            // GraphProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(795, 621);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelGraph);
            this.Controls.Add(this.panelGraphButtons);
            this.Controls.Add(this.Topological);
            this.Controls.Add(this.Prim);
            this.Controls.Add(this.Dijkstra);
            this.Controls.Add(this.Kruskal);
            this.Name = "GraphProject";
            this.Text = "Graph Project";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelGraphButtons;
        private System.Windows.Forms.Panel panelGraph;
        private System.Windows.Forms.Button Topological;
        private System.Windows.Forms.Button Prim;
        private System.Windows.Forms.Button Dijkstra;
        private System.Windows.Forms.Button Kruskal;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableWeights;
    }
}

