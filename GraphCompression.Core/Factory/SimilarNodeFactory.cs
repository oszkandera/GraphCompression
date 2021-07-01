using GraphCompression.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace GraphCompression.Core.Factory
{
    public static class SimilarNodeFactory
    {
        public static SimilarNode CreateSimilarNodeFromKeyValuePairs(KeyValuePair<int, List<int>> node1KeyValuePair, KeyValuePair<int, List<int>>? node2KeyValuePair = null)
        {
            var similarNode = new SimilarNode
            {
                Node1 = node1KeyValuePair.Key,
                Neighbors1 = new SortedList<int, int>(node1KeyValuePair.Value.ToDictionary(x => x)),
                Node2 = node2KeyValuePair == null ? default(int?) : node2KeyValuePair.Value.Key,
                Neighbors2 = node2KeyValuePair == null ? new SortedList<int, int>() : new SortedList<int, int>(node2KeyValuePair.Value.Value.ToDictionary(x => x))
            };

            return similarNode;
        }
    }
}
