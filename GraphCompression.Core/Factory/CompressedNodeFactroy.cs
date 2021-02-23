using GraphCompression.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace GraphCompression.Core.Factory
{
    public static class CompressedNodeFactroy
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
    }
}
