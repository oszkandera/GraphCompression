using GraphCompression.Core.Interfaces.Code;
using System.Collections.Generic;
using System.Linq;

namespace GraphCompression.Core.Code
{
    public class DegreeSortingAlgorithm : IGraphSortingAlgorithm
    {
        public List<KeyValuePair<int, List<int>>> GetSortedGraphStructure(Dictionary<int, List<int>> originalGraphStructure)
        {
            var sortedGraphStructure = new List<KeyValuePair<int, List<int>>>();

            if (originalGraphStructure == null)
            {
                return sortedGraphStructure;
            }

            PrepareStructure(originalGraphStructure);

            var sortedByDegree = originalGraphStructure.OrderBy(x => x.Value.Count);

            foreach (var node in sortedByDegree)
            {
                var sortedListOfNeighbors = node.Value.OrderBy(x => x).ToList();

                var nodeNeighborPair = KeyValuePair.Create(node.Key, sortedListOfNeighbors);

                sortedGraphStructure.Add(nodeNeighborPair);
            }

            return sortedGraphStructure;
        }

        private void PrepareStructure(Dictionary<int, List<int>> originalStructure)
        {
            foreach(var node in originalStructure)
            {
                if(node.Value == null)
                {
                    originalStructure[node.Key] = new List<int>();
                }
            }
        }
    }
}
