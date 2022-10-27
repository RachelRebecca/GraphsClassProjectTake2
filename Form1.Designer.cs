﻿namespace GraphsClassProjectTakeTwo
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
            this.tableEdgesWeights = new System.Windows.Forms.DataGridView();
            this.removeEdge = new System.Windows.Forms.Button();
            this.removeNode = new System.Windows.Forms.Button();
            this.addEdge = new System.Windows.Forms.Button();
            this.panelGraphOperations = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.tableEdgesWeights)).BeginInit();
            this.panelGraphOperations.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelGraphButtons
            // 
            this.panelGraphButtons.AutoScroll = true;
            this.panelGraphButtons.Location = new System.Drawing.Point(634, 12);
            this.panelGraphButtons.Name = "panelGraphButtons";
            this.panelGraphButtons.Size = new System.Drawing.Size(194, 330);
            this.panelGraphButtons.TabIndex = 0;
            // 
            // panelGraph
            // 
            this.panelGraph.BackColor = System.Drawing.Color.Gray;
            this.panelGraph.Location = new System.Drawing.Point(12, 12);
            this.panelGraph.Name = "panelGraph";
            this.panelGraph.Size = new System.Drawing.Size(600, 535);
            this.panelGraph.TabIndex = 0;
            // 
            // Topological
            // 
            this.Topological.Enabled = false;
            this.Topological.Location = new System.Drawing.Point(329, 5);
            this.Topological.Name = "Topological";
            this.Topological.Size = new System.Drawing.Size(104, 35);
            this.Topological.TabIndex = 1;
            this.Topological.Text = "Topological Sort";
            this.Topological.UseVisualStyleBackColor = true;
            this.Topological.Click += new System.EventHandler(this.Topological_Click);
            // 
            // Prim
            // 
            this.Prim.Enabled = false;
            this.Prim.Location = new System.Drawing.Point(481, 5);
            this.Prim.Name = "Prim";
            this.Prim.Size = new System.Drawing.Size(104, 35);
            this.Prim.TabIndex = 2;
            this.Prim.Text = "Prim\'s Algorithm";
            this.Prim.UseVisualStyleBackColor = true;
            this.Prim.Click += new System.EventHandler(this.Prim_Click);
            // 
            // Dijkstra
            // 
            this.Dijkstra.Enabled = false;
            this.Dijkstra.Location = new System.Drawing.Point(14, 5);
            this.Dijkstra.Name = "Dijkstra";
            this.Dijkstra.Size = new System.Drawing.Size(104, 35);
            this.Dijkstra.TabIndex = 1;
            this.Dijkstra.Text = "Dijkstra\'s Algorithm";
            this.Dijkstra.UseVisualStyleBackColor = true;
            this.Dijkstra.Click += new System.EventHandler(this.Dijkstra_Click);
            // 
            // Kruskal
            // 
            this.Kruskal.Enabled = false;
            this.Kruskal.Location = new System.Drawing.Point(171, 5);
            this.Kruskal.Name = "Kruskal";
            this.Kruskal.Size = new System.Drawing.Size(104, 35);
            this.Kruskal.TabIndex = 0;
            this.Kruskal.Text = "Kruskal\'s Algorithm";
            this.Kruskal.UseVisualStyleBackColor = true;
            this.Kruskal.Click += new System.EventHandler(this.Kruskal_Click);
            // 
            // tableEdgesWeights
            // 
            this.tableEdgesWeights.AllowUserToAddRows = false;
            this.tableEdgesWeights.AllowUserToDeleteRows = false;
            this.tableEdgesWeights.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.tableEdgesWeights.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.tableEdgesWeights.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tableEdgesWeights.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableEdgesWeights.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableEdgesWeights.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.tableEdgesWeights.Location = new System.Drawing.Point(634, 359);
            this.tableEdgesWeights.Name = "tableEdgesWeights";
            this.tableEdgesWeights.Size = new System.Drawing.Size(194, 250);
            this.tableEdgesWeights.TabIndex = 3;
            this.tableEdgesWeights.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.TableWeights_CellClick);
            // 
            // removeEdge
            // 
            this.removeEdge.Enabled = false;
            this.removeEdge.Location = new System.Drawing.Point(90, 46);
            this.removeEdge.Name = "removeEdge";
            this.removeEdge.Size = new System.Drawing.Size(101, 23);
            this.removeEdge.TabIndex = 4;
            this.removeEdge.Text = "Remove Edge";
            this.removeEdge.UseVisualStyleBackColor = true;
            this.removeEdge.Click += new System.EventHandler(this.RemoveEdge_Click);
            // 
            // removeNode
            // 
            this.removeNode.Enabled = false;
            this.removeNode.Location = new System.Drawing.Point(256, 46);
            this.removeNode.Name = "removeNode";
            this.removeNode.Size = new System.Drawing.Size(96, 23);
            this.removeNode.TabIndex = 5;
            this.removeNode.Text = "Remove Node";
            this.removeNode.UseVisualStyleBackColor = true;
            this.removeNode.Click += new System.EventHandler(this.RemoveNode_Click);
            // 
            // addEdge
            // 
            this.addEdge.Enabled = false;
            this.addEdge.Location = new System.Drawing.Point(423, 46);
            this.addEdge.Name = "addEdge";
            this.addEdge.Size = new System.Drawing.Size(69, 23);
            this.addEdge.TabIndex = 6;
            this.addEdge.Text = "Add Edge";
            this.addEdge.UseVisualStyleBackColor = true;
            this.addEdge.Click += new System.EventHandler(this.AddEdge_Click);
            // 
            // panelGraphOperations
            // 
            this.panelGraphOperations.Controls.Add(this.addEdge);
            this.panelGraphOperations.Controls.Add(this.removeNode);
            this.panelGraphOperations.Controls.Add(this.removeEdge);
            this.panelGraphOperations.Controls.Add(this.Topological);
            this.panelGraphOperations.Controls.Add(this.Prim);
            this.panelGraphOperations.Controls.Add(this.Dijkstra);
            this.panelGraphOperations.Controls.Add(this.Kruskal);
            this.panelGraphOperations.Location = new System.Drawing.Point(12, 543);
            this.panelGraphOperations.Name = "panelGraphOperations";
            this.panelGraphOperations.Size = new System.Drawing.Size(600, 86);
            this.panelGraphOperations.TabIndex = 7;
            this.panelGraphOperations.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GraphOperations_MouseMove);
            // 
            // GraphProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(837, 631);
            this.Controls.Add(this.panelGraphOperations);
            this.Controls.Add(this.tableEdgesWeights);
            this.Controls.Add(this.panelGraph);
            this.Controls.Add(this.panelGraphButtons);
            this.Name = "GraphProject";
            this.Text = "Graph Project";
            ((System.ComponentModel.ISupportInitialize)(this.tableEdgesWeights)).EndInit();
            this.panelGraphOperations.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelGraphButtons;
        private System.Windows.Forms.Panel panelGraph;
        private System.Windows.Forms.Button Topological;
        private System.Windows.Forms.Button Prim;
        private System.Windows.Forms.Button Dijkstra;
        private System.Windows.Forms.Button Kruskal;
        private System.Windows.Forms.DataGridView tableEdgesWeights;
        private System.Windows.Forms.Button removeEdge;
        private System.Windows.Forms.Button removeNode;
        private System.Windows.Forms.Button addEdge;
        private System.Windows.Forms.Panel panelGraphOperations;
    }
}

