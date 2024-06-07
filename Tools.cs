using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClassIdNet
{

    public static class Tools
    {
        /// <summary>
        /// Saves the list of strings to a specified file.
        /// </summary>
        /// <param name="oList">The list of strings to save.</param>
        /// <param name="filename">The name of the file to save to.</param>
        public static void Save(List<string> oList, string filename)
        {
            File.WriteAllLines(filename, oList);
        }

        /// <summary>
        /// Loads a list of strings from a specified file.
        /// </summary>
        /// <param name="filename">The name of the file to load from.</param>
        /// <returns>A list of strings loaded from the file.</returns>
        public static List<string> Load(string filename)
        {
            return new List<string>(File.ReadAllLines(filename));
        }

        /// <summary>
        /// Prints the graph to the console.
        /// </summary>
        /// <param name="graph">The graph to print.</param>
        public static void PrintGraph(DirectedGraph graph)
        {
            foreach (var nodeIndex in graph.Nodes.Values)
            {
                foreach (var node in nodeIndex.AllNodes.Values)
                {
                    Console.WriteLine($"[{node.Class} : {node.Id}]");
                    if (!string.IsNullOrEmpty(node.__text__))
                    {
                        Console.WriteLine($"__text__ : \"{node.__text__}\"");
                    }
                    if (node.__vector__.Any())
                    {
                        Console.WriteLine($"__vector__ : [{string.Join(", ", node.__vector__)}]");
                    }
                    foreach (var goalClass in node.Goals)
                    {
                        var goalIds = goalClass.Value;
                        if (goalIds.Count == 1)
                        {
                            Console.WriteLine($"{goalClass.Key} : {goalIds.First()}");
                        }
                        else
                        {
                            Console.WriteLine($"{goalClass.Key} : [{string.Join(", ", goalIds)}]");
                        }
                    }
                    foreach (var extra in node.Extra)
                    {
                        Console.WriteLine($"{extra.Key} : [{string.Join(", ", extra.Value)}]");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
