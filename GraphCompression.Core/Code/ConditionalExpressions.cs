using GraphCompression.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace GraphCompression.Core.Code
{
    public static class ConditionalExpressions
    {
        public static bool SimilarNodeContainsReference(SimilarNode similarNode)
        {
            return similarNode.Node2.HasValue;
        }

        public static bool IsReferenceNeighborCountHigherThanMaxReferenceListSize(SimilarNode similarNode, int? maxReferenceListSize)
        {
            var numberOfNodesInNeighbor = similarNode.Neighbors2.Count;
            return maxReferenceListSize.HasValue && numberOfNodesInNeighbor > maxReferenceListSize.Value;
        }

        public static bool ContainsReferenceChainDictionaryReferencingItemWithValue(Dictionary<int, HashSet<int>> referenceChain, int referencingItem)
        {
            return !referenceChain.ContainsKey(referencingItem) || referenceChain[referencingItem] == null || !referenceChain[referencingItem].Any();
        }
    }
}
