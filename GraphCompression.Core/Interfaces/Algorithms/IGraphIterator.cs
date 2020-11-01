using System.Collections.Generic;

namespace GraphProcessor.Core.Interfaces.Algorithms
{
    public interface IGraphIterator
    {
        bool ExistsPathBetweenTwoEdges(Dictionary<int, HashSet<int>> graph, int from, int to);
    }
}
