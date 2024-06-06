using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassIdNet
{
    public class NodeIndex : GObject
    {
        public string ClassName { get; }
        public Dictionary<string, Node> AllNodes { get; set; }

        public NodeIndex(string className)
        {
            this.ClassName = className;
            this.AllNodes = new Dictionary<string, Node>();
        }
    }
}
