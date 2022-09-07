using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphsClassProjectTakeTwo
{
    internal class EdgeGUI
    {
        // todo: make this inherit from a control (like a button) which has an onHover method already
        
        private Edge edge;
        private Panel panel;

        public EdgeGUI(Edge edge, Panel panel)
        {
            this.edge = edge;
            this.panel = panel;
        }

        // methods

        public void CreateGraphicForEdge()
        {
            panel.CreateGraphics();
        }

        public void onHover()
        {
            //  display the weight
        }
    }
}
