using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassIdNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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

        public List<string> ToStringList()
        {
            List<string> lines = new List<string>();

            // Serialize Nodes
            foreach (string className in Nodes.Keys)
            {
                NodeIndex nodeIndex = Nodes[className];
                foreach (string nodeId in nodeIndex.AllNodes.Keys)
                {
                    Node node = nodeIndex.AllNodes[nodeId];
                    lines.Add($"[{node.Class} : {node.Id}]");

                    if (!string.IsNullOrEmpty(node.__text__))
                    {
                        lines.Add($"__text__ : \"{node.__text__}\"");
                    }

                    if (node.__vector__.Any())
                    {
                        lines.Add($"__vector__ : [{string.Join(", ", node.__vector__)}]");
                    }

                    foreach (string goalClass in node.Goals.Keys)
                    {
                        HashSet<string> goalIds = node.Goals[goalClass];
                        if (goalIds.Count == 1)
                        {
                            lines.Add($"{goalClass} : {goalIds.First()}");
                        }
                        else
                        {
                            lines.Add($"{goalClass} : [{string.join(", ", goalIds)}]");
                        }
                    }

                    foreach (string extraKey in node.Extra.Keys)
                    {
                        List<string> extraValues = node.Extra[extraKey];
                        lines.Add($"{extraKey} : [{string.join(", ", extraValues)}]");
                    }

                    lines.Add("");
                }
            }

            return lines;
        }

        public void Upload(List<string> lines)
        {
            Node currentNode = null;

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                {
                    continue; // Skip empty lines and comments
                }

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    // Deserialize Node
                    string nodeInfo = trimmedLine.Trim('[', ']').Trim();
                    string[] nodeParts = nodeInfo.Split(new[] { ':' }, 2);
                    string nodeClass = nodeParts[0].Trim();
                    string nodeId = nodeParts[1].Trim();

                    // Ensure the node exists or use the existing node
                    EnsureNodeExists(nodeClass, nodeId);
                    currentNode = Nodes[nodeClass].AllNodes[nodeId];
                }
                else if (currentNode != null && trimmedLine.Contains(":"))
                {
                    string[] parts = trimmedLine.Split(new[] { ':' }, 2);
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    if (key == "__text__")
                    {
                        currentNode.__text__ = value.Trim('"');
                    }
                    else if (key == "__vector__")
                    {
                        currentNode.__vector__ = value.Trim('[', ']').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(float.Parse).ToList();
                    }
                    else if (value.StartsWith("[") && value.EndsWith("]"))
                    {
                        // List of links or extra values
                        string[] items = value.Trim('[', ']').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                              .Select(item => item.Trim()).ToArray();

                        if (key.StartsWith("__") && key.EndsWith("__"))
                        {
                            currentNode.AddExtra(key, items.ToList());
                        }
                        else
                        {
                            foreach (string itemId in items)
                            {
                                EnsureNodeExists(key, itemId);
                                Node targetNode = Nodes[key].AllNodes[itemId];
                                DirectedLink link = new DirectedLink(currentNode, targetNode);
                                if (!Links.ContainsKey(((currentNode.Class, currentNode.Id), (key, itemId))))
                                {
                                    AddLink(link);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Single link or extra value
                        if (key.StartsWith("__") && key.EndsWith("__"))
                        {
                            currentNode.AddExtra(key, value);
                        }
                        else
                        {
                            EnsureNodeExists(key, value);
                            Node targetNode = Nodes[key].AllNodes[value];
                            DirectedLink link = new DirectedLink(currentNode, targetNode);
                            if (!Links.ContainsKey(((currentNode.Class, currentNode.Id), (key, value))))
                            {
                                AddLink(link);
                            }
                        }
                    }
                }
            }
        }
    }
}
