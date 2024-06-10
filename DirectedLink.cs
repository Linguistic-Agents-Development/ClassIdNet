using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassIdNet
{
    public class DirectedLink : GObject
    {
        public Node  Source { get; set; }
        public Node  Goal { get; set; }

        // Constructor
        public DirectedLink(Node source, Node goal)
        {
            this.Source = source;
            this.Goal = goal;
        }
    }
}

