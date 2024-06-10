using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassIdNet
{
    public class Node : GObject
    {
        public ClassSubgraph Class { get; }
        public string Id { get; }
        public Dictionary<string, HashSet<string>> Sources { get; }
        public Dictionary<string, HashSet<string>> Goals { get; }
        public string __text__ { get; set; }
        public Dictionary<string, List<string>> Extra { get; }

        public Node(string className, string id, DirectedGraph graph)
        {
            ClassSubgraph classSubgraph = graph.GetClass(className);
            this.Class = classSubgraph;
            this.Id = id;
            this.Sources = new Dictionary<string, HashSet<string>>();
            this.Goals = new Dictionary<string, HashSet<string>>();
            this.__text__ = "";
            this.Extra = new Dictionary<string, List<string>>();
        }

        public void AddLinkTo(string goalClass, string goalId)
        {
            Class.Graph.EnsureNodeExists(goalClass, goalId);
            if (!Class.Graph.Links.ContainsKey(((this.Class.ClassName, this.Id), (goalClass, goalId))))
            {
                Node goalNode = Class.Graph.Classes[goalClass].AllNodes[goalId];
                DirectedLink link = new DirectedLink(this, goalNode);
                Class.Graph.AddLink(link);
            }
        }

        public void AddLinkFrom(string sourceClass, string sourceId)
        {
            Class.Graph.EnsureNodeExists(sourceClass, sourceId);
            if (!Class.Graph.Links.ContainsKey(((sourceClass, sourceId), (this.Class.ClassName, this.Id))))
            {
                Node sourceNode = Class.Graph.Classes[sourceClass].AllNodes[sourceId];
                DirectedLink link = new DirectedLink(sourceNode, this);
                Class.Graph.AddLink(link);
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

        public void Merge(Node other)
        {
            if (this.Id != other.Id || this.Class.ClassName != other.Class.ClassName)
            {
                throw new InvalidOperationException("Cannot merge nodes with different IDs or classes.");
            }

            // Merge links
            foreach (var goal in other.Goals)
            {
                if (!this.Goals.ContainsKey(goal.Key))
                {
                    this.Goals[goal.Key] = new HashSet<string>();
                }
                this.Goals[goal.Key].UnionWith(goal.Value);
            }

            foreach (var source in other.Sources)
            {
                if (!this.Sources.ContainsKey(source.Key))
                {
                    this.Sources[source.Key] = new HashSet<string>();
                }
                this.Sources[source.Key].UnionWith(source.Value);
            }

            // Merge extra fields
            foreach (var extraEntry in other.Extra)
            {
                if (!this.Extra.ContainsKey(extraEntry.Key))
                {
                    this.Extra[extraEntry.Key] = new List<string>();
                }
                this.Extra[extraEntry.Key].AddRange(extraEntry.Value);
                this.Extra[extraEntry.Key] = this.Extra[extraEntry.Key].Distinct().ToList();
            }

            // Merge __text__ fields
            if (!string.IsNullOrEmpty(other.__text__))
            {
                if (string.IsNullOrEmpty(this.__text__))
                {
                    this.__text__ = other.__text__;
                }
                else
                {
                    this.__text__ += "\n" + other.__text__;
                }
            }

            // Merge __vector__ fields
            this.__vector__.AddRange(other.__vector__);
            this.__vector__ = this.__vector__.Distinct().ToList();
        }
    }
}