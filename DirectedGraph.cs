using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassIdNet
{
    public class DirectedGraph : GObject
    {
        public Dictionary<string, ClassSubgraph> Classes { get; set; }
        public Dictionary<((string Class, string Id) Source, (string Class, string Id) Goal), DirectedLink> Links { get; set; }

        public DirectedGraph()
        {
            Classes = new Dictionary<string, ClassSubgraph>();
            Links = new Dictionary<((string Class, string Id), (string Class, string Id)), DirectedLink>();
        }

        public void AddNode(Node node)
        {
            AddClass(node.Class.ClassName);

            ClassSubgraph classSubgraph = this.Classes[node.Class.ClassName];
            if (!classSubgraph.AllNodes.ContainsKey(node.Id))
            {
                classSubgraph.AllNodes[node.Id] = node;
            }
        }

        public void AddLink(DirectedLink link)
        {
            AddNode(link.Source);
            AddNode(link.Goal);

            if (link.Source == link.Goal)
            {
                throw new InvalidOperationException("Cannot add a link from a node to itself.");
            }

            var key = ((link.Source.Class.ClassName, link.Source.Id), (link.Goal.Class.ClassName, link.Goal.Id));

            if (Links.ContainsKey(key))
            {
                // Ignore adding the link if it already exists
                return;
            }

            Links[key] = link;

            if (!link.Source.Goals.ContainsKey(link.Goal.Class.ClassName))
            {
                link.Source.Goals[link.Goal.Class.ClassName] = new HashSet<string>();
            }
            link.Source.Goals[link.Goal.Class.ClassName].Add(link.Goal.Id);

            if (!link.Goal.Sources.ContainsKey(link.Source.Class.ClassName))
            {
                link.Goal.Sources[link.Source.Class.ClassName] = new HashSet<string>();
            }
            link.Goal.Sources[link.Source.Class.ClassName].Add(link.Source.Id);
        }

        public void EnsureNodeExists(string className, string id)
        {
            AddClass(className);

            if (!Classes[className].AllNodes.ContainsKey(id))
            {
                Node newNode = new Node(className, id, this);
                Classes[className].AllNodes[id] = newNode;
            }
        }

        public ClassSubgraph GetClass(string className)
        {
            AddClass(className);
            return Classes[className];
        }

        public void AddClass(string className)
        {
            if (!this.Classes.ContainsKey(className))
            {
                this.Classes[className] = new ClassSubgraph(className, this);
            }
        }

        public bool Contains(DirectedGraph graph)
        {
            foreach (var classEntry in graph.Classes)
            {
                if (!this.Classes.ContainsKey(classEntry.Key))
                {
                    return false;
                }

                foreach (var nodeEntry in classEntry.Value.AllNodes)
                {
                    if (!this.Classes[classEntry.Key].AllNodes.ContainsKey(nodeEntry.Key))
                    {
                        return false;
                    }
                }
            }

            foreach (var link in graph.Links)
            {
                if (!this.Links.ContainsKey(link.Key))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Equals(DirectedGraph graph)
        {
            return this.Contains(graph) && graph.Contains(this);
        }

        public DirectedGraph Copy()
        {
            DirectedGraph copy = new DirectedGraph();

            foreach (var classEntry in this.Classes)
            {
                copy.AddClass(classEntry.Key);
                foreach (var nodeEntry in classEntry.Value.AllNodes)
                {
                    Node newNode = new Node(classEntry.Key, nodeEntry.Key, copy);
                    newNode.__text__ = nodeEntry.Value.__text__;
                    newNode.__vector__ = new List<float>(nodeEntry.Value.__vector__);
                    foreach (var extraEntry in nodeEntry.Value.Extra)
                    {
                        newNode.Extra[extraEntry.Key] = new List<string>(extraEntry.Value);
                    }
                    copy.AddNode(newNode);
                }
            }

            foreach (var linkEntry in this.Links)
            {
                Node sourceNode = copy.Classes[linkEntry.Key.Source.Class].AllNodes[linkEntry.Key.Source.Id];
                Node goalNode = copy.Classes[linkEntry.Key.Goal.Class].AllNodes[linkEntry.Key.Goal.Id];
                DirectedLink newLink = new DirectedLink(sourceNode, goalNode);
                copy.AddLink(newLink);
            }

            return copy;
        }

        public void Merge(DirectedGraph graph)
        {
            foreach (var classEntry in graph.Classes)
            {
                this.AddClass(classEntry.Key);
                foreach (var nodeEntry in classEntry.Value.AllNodes)
                {
                    if (!this.Classes[classEntry.Key].AllNodes.ContainsKey(nodeEntry.Key))
                    {
                        Node newNode = new Node(classEntry.Key, nodeEntry.Key, this);
                        newNode.__text__ = nodeEntry.Value.__text__;
                        newNode.__vector__ = new List<float>(nodeEntry.Value.__vector__);
                        foreach (var extraEntry in nodeEntry.Value.Extra)
                        {
                            newNode.Extra[extraEntry.Key] = new List<string>(extraEntry.Value);
                        }
                        this.AddNode(newNode);
                    }
                    else
                    {
                        // Merge nodes with the same ID
                        Node existingNode = this.Classes[classEntry.Key].AllNodes[nodeEntry.Key];
                        Node newNode = nodeEntry.Value;
                        existingNode.Merge(newNode);
                    }
                }
            }

            foreach (var linkEntry in graph.Links)
            {
                if (!this.Links.ContainsKey(linkEntry.Key))
                {
                    Node sourceNode = this.Classes[linkEntry.Key.Source.Class].AllNodes[linkEntry.Key.Source.Id];
                    Node goalNode = this.Classes[linkEntry.Key.Goal.Class].AllNodes[linkEntry.Key.Goal.Id];
                    DirectedLink newLink = new DirectedLink(sourceNode, goalNode);
                    this.AddLink(newLink);
                }
            }
        }
    }
}

