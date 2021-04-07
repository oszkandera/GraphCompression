using GraphCompression.Core.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GraphCompression.Core.Factory
{
    public static class CompressedNodeFactory
    {
        public static CompressedNode CreateCompressedNodeFromSimilarNodeWithoutReference(SimilarNode similarNode)
        {
            var extraNodes = similarNode.Neighbors1.Select(x => x.Value);
            var compressedNode = new CompressedNode
            {
                Id = similarNode.Node1,
                ExtraNodes = new SortedSet<int>(extraNodes)
            };

            return compressedNode;
        }

        public static CompressedNode CreateCompressedNode(BitArray referenceList, SortedSet<int> extraNodes, SimilarNode similarNode)
        {
            var compressedNode = new CompressedNode
            {
                Id = similarNode.Node1,
                ReferenceId = similarNode.Node2,
                ReferenceList = referenceList,
                ExtraNodes = extraNodes,
            };

            return compressedNode;
        }
    }
}
