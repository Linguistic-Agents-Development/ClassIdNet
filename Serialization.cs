using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassIdNet
{
    public static class Serialization
    {
        // Directed Graph Protocol
        public static List<string> SerializeGraph(DirectedGraph graph)
        {
            List<string> lines = new List<string>();

            // Serialize Nodes
            foreach (string className in graph.Classes.Keys)
            {
                ClassSubgraph classSubgraph = graph.Classes[className];
                foreach (string nodeId in classSubgraph.AllNodes.Keys)
                {
                    Node node = classSubgraph.AllNodes[nodeId];
                    lines.Add($"[{node.Class.ClassName} : {node.Id}]");

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
                        foreach (var goalId in goalIds)
                        {
                            lines.Add($"{goalClass} : {goalId}");
                        }
                    }

                    foreach (string extraKey in node.Extra.Keys)
                    {
                        List<string> extraValues = node.Extra[extraKey];
                        foreach (var extraValue in extraValues)
                        {
                            lines.Add($"{extraKey} : {extraValue}");
                        }
                    }

                    lines.Add("");
                }
            }

            return lines;
        }

        // Directed Graph Protocol
        public static DirectedGraph DeserializeGraph(List<string> lines)
        {
            DirectedGraph graph = new DirectedGraph();
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
                    graph.EnsureNodeExists(nodeClass, nodeId);
                    currentNode = graph.Classes[nodeClass].AllNodes[nodeId];
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
                                currentNode.AddLinkTo(key, itemId);
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
                            currentNode.AddLinkTo(key, value);
                        }
                    }
                }
            }

            return graph;
        }
    }
}
