using System.Collections.Generic;


namespace GraphCompression.Core.Interfaces.Code
{
    public interface IGraphSortingAlgorithm
    {
        List<KeyValuePair<int, List<int>>> GetSortedGraphStructure(Dictionary<int, List<int>> originalGraphStructure);
    }
}
