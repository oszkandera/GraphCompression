using GraphCompression.Core.Models;

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
    }
}
