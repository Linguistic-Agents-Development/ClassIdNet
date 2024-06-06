using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassIdNet
{
    public class Node : GObject
    {
        public string Class { get; set; }
        public string Id { get; set; }
        public Dictionary<string, HashSet<string>> Sources { get; set; } = new Dictionary<string, HashSet<string>>();
        public Dictionary<string, HashSet<string>> Goals { get; set; } = new Dictionary<string, HashSet<string>>();
        public DirectedGraph Graph { get; set; }
        public string __text__ { get; set; }
        public List<float> __vector__ { get; set; } = new List<float>();
        public Dictionary<string, List<string>> Extra { get; set; } = new Dictionary<string, List<string>>();

        // Constructor
        public Node(string Class, string Id, DirectedGraph graph)
        {
            this.Class = Class;
            this.Id = Id;
            this.Graph = graph;
        }

        // Method to add a single extra string value
        public void AddExtra(string key, string value)
        {
            if (!this.Extra.ContainsKey(key))
            {
                this.Extra[key] = new List<string>();
            }
            this.Extra[key].Add(value);
        }

        // Method to add multiple extra string values
        public void AddExtra(string key, List<string> values)
        {
            if (!this.Extra.ContainsKey(key))
            {
                this.Extra[key] = new List<string>();
            }
            this.Extra[key].AddRange(values);
        }

        // Method to add an outgoing link to a goal node
        public void AddLinkTo(Node goal)
        {
            DirectedLink link = new DirectedLink(this, goal);
            this.Graph.AddLink(link);
        }

        // Method to add an incoming link from a source node
        public void AddLinkFrom(Node source)
        {
            DirectedLink link = new DirectedLink(source, this);
            this.Graph.AddLink(link);
        }
    }

}