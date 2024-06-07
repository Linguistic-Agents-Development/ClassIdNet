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

        public Node(string Class, string Id, DirectedGraph graph)
        {
            this.Class = Class;
            this.Id = Id;
            this.Graph = graph;
        }

        public void AddLinkTo(string targetClass, string targetId)
        {
            Graph.EnsureNodeExists(targetClass, targetId);
            if (!Graph.Links.ContainsKey(((this.Class, this.Id), (targetClass, targetId))))
            {
                Node targetNode = Graph.Nodes[targetClass].AllNodes[targetId];
                DirectedLink link = new DirectedLink(this, targetNode);
                Graph.AddLink(link);
            }
        }

        public void AddLinkFrom(string sourceClass, string sourceId)
        {
            Graph.EnsureNodeExists(sourceClass, sourceId);
            if (!Graph.Links.ContainsKey(((sourceClass, sourceId), (this.Class, this.Id))))
            {
                Node sourceNode = Graph.Nodes[sourceClass].AllNodes[sourceId];
                DirectedLink link = new DirectedLink(sourceNode, this);
                Graph.AddLink(link);
            }
        }

        public void AddExtra(string key, string value)
        {
            if (!Extra.ContainsKey(key))
            {
                Extra[key] = new List<string>();
            }
            Extra[key].Add(value);
        }

        public void AddExtra(string key, List<string> values)
        {
            if (!Extra.ContainsKey(key))
            {
                Extra[key] = new List<string>();
            }
            Extra[key].AddRange(values);
        }
    }
}