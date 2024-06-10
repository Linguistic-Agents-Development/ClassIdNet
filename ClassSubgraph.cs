using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassIdNet
{
    public class ClassSubgraph : GObject
    {
        public string ClassName { get; }
        public DirectedGraph Graph { get; }
        public Dictionary<string, Node> AllNodes { get; set; }

        // Constructor
        public ClassSubgraph(string className, DirectedGraph graph)
        {
            this.ClassName = className;
            this.Graph = graph;
            this.AllNodes = new Dictionary<string, Node>();
        }
    }
}
