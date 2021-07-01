using GraphCompression.Core.Models;
using System.Collections.Generic;

namespace GraphCompression.Core.Interfaces.Algorithms
{
    public interface ISimilarNodeProcessor
    {
        IEnumerable<SimilarNode> CreateListOfSimilarNodes(List<KeyValuePair<int, List<int>>> sortedGraphStructure, CompressParameters compressParameters);
    }
}
