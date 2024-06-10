using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassIdNet
{
    public static class GraphExtensions
    {
        // Extension method for Select
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            foreach (TSource item in source)
            {
                yield return selector(item);
            }
        }

        // Extension method for Where
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach (TSource item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        // Extension method for OrderBy
        public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.OrderBy(keySelector);
        }

        // Extension method for filtering nodes by class name
        public static IEnumerable<Node> WithClass(this DirectedGraph graph, string className)
        {
            if (graph.Classes.ContainsKey(className))
            {
                return graph.Classes[className].AllNodes.Values;
            }
            return Enumerable.Empty<Node>();
        }

        // Extension methods
        public static List<Node> FindByClass(this IEnumerable<Node> nodes, string className)
        {
            return nodes.Where(n => n.Class.ClassName == className).ToList();
        }
    }
}
