using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassIdNet
{
    public class DirectedGraph : GObject
    {
        public Dictionary<string, NodeIndex> Nodes { get; set; } = new Dictionary<string, NodeIndex>();
        public Dictionary<((string Class, string Id) Source, (string Class, string Id) Goal), DirectedLink> Links { get; set; } = new Dictionary<((string Class, string Id), (string Class, string Id)), DirectedLink>();

        public void AddNode(Node node)
        {
            if (!this.Nodes.ContainsKey(node.Class))
            {
                this.Nodes[node.Class] = new NodeIndex(node.Class);
            }

            NodeIndex nodeIndex = this.Nodes[node.Class];
            if (!nodeIndex.AllNodes.ContainsKey(node.Id))
            {
                nodeIndex.AllNodes[node.Id] = node;
            }
        }

        public void AddLink(DirectedLink link)
        {
            // Ensure nodes are registered in the graph
            AddNode(link.Source);
            AddNode(link.Goal);

            // Prevent self-loop
            if (link.Source == link.Goal)
            {
                throw new InvalidOperationException("Cannot add a link from a node to itself.");
            }

            var key = ((link.Source.Class, link.Source.Id), (link.Goal.Class, link.Goal.Id));

            // Check if the link already exists
            if (Links.ContainsKey(key))
            {
                throw new InvalidOperationException("A link between these nodes already exists.");
            }

            Links[key] = link;

            // Update source node's goals
            if (!link.Source.Goals.ContainsKey(link.Goal.Class))
            {
                link.Source.Goals[link.Goal.Class] = new HashSet<string>();
            }
            link.Source.Goals[link.Goal.Class].Add(link.Goal.Id);

            // Update goal node's sources
            if (!link.Goal.Sources.ContainsKey(link.Source.Class))
            {
                link.Goal.Sources[link.Source.Class] = new HashSet<string>();
            }
            link.Goal.Sources[link.Source.Class].Add(link.Source.Id);
        }

        public void EnsureNodeExists(string nodeClass, string nodeId)
        {
            if (!Nodes.ContainsKey(nodeClass))
            {
                Nodes[nodeClass] = new NodeIndex(nodeClass);
            }

            if (!Nodes[nodeClass].AllNodes.ContainsKey(nodeId))
            {
                Node newNode = new Node(nodeClass, nodeId, this);
                Nodes[nodeClass].AllNodes[nodeId] = newNode;
            }
        }
    }
}
