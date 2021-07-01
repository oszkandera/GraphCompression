using GraphCompression.Core.Factory;
using GraphCompression.Core.Interfaces.Algorithms;
using GraphCompression.Core.Models;
using System.Collections.Generic;

namespace GraphCompression.Core.Algorithms
{
    /// <summary>
    /// Class contains algorithm for similar node search
    /// </summary>
    public class SimilarNodeProcessor : ISimilarNodeProcessor
    {
        private readonly IReferenceImmersionValidator _referenceImmersionValidator;

        public SimilarNodeProcessor(IReferenceImmersionValidator referenceImmersionValidator)
        {
            _referenceImmersionValidator = referenceImmersionValidator;
        }

        /// <summary>
        /// Method creates list of similar nodes of original graph
        /// </summary>
        /// <param name="sortedGraphStructure">Sorted original graph structure</param>
        /// <param name="compressParameters">Parameters of compression</param>
        /// <returns>List of <c>SimilarNode</c>.</returns>
        public IEnumerable<SimilarNode> CreateListOfSimilarNodes(List<KeyValuePair<int, List<int>>> sortedGraphStructure, CompressParameters compressParameters)
        {
            var similarNodes = new List<SimilarNode>();
            var referenceChain = new Dictionary<int, HashSet<int>>();

            for (int findingNodeIndex = 0; findingNodeIndex < sortedGraphStructure.Count - 1; findingNodeIndex++)
            {
                var findingNode = sortedGraphStructure[findingNodeIndex];

                int maxSameNodeCount = 0;
                KeyValuePair<int, List<int>>? mostSimilarNode = null;

                for (int wantedNodeIndex = findingNodeIndex + 1; wantedNodeIndex < sortedGraphStructure.Count; wantedNodeIndex++)
                {
                    var wantedNode = sortedGraphStructure[wantedNodeIndex];

                    var findingNodesCount = findingNode.Value.Count;
                    var wantedNodesCount = wantedNode.Value.Count;

                    var sameNodesCount = 0;
                    var mostSimilarNodeFound = false;

                    for (int findingNodeNeighborIndex = 0; findingNodeNeighborIndex < findingNodesCount; findingNodeNeighborIndex++)
                    {
                        var findingNeighbor = findingNode.Value[findingNodeNeighborIndex];

                        for (int wantedNodeNeighborIndex = 0; wantedNodeNeighborIndex < wantedNodesCount; wantedNodeNeighborIndex++)
                        {
                            var wantedNeighbor = wantedNode.Value[wantedNodeNeighborIndex];

                            //Možnost časové optimalizace
                            if (compressParameters.WithTimeOptimalization && findingNeighbor > wantedNeighbor) break;

                            if (findingNeighbor == wantedNeighbor) sameNodesCount++;
                        }

                        if (sameNodesCount == findingNodesCount) // Našli jsme uzel, ktery pojme všechny sousedy hledaneho uzlu
                        {
                            var canCreateReference = _referenceImmersionValidator.ValidateReferenceImmersion(referenceChain, findingNode.Key, compressParameters.MaxReferenceChainSize);
                            if (canCreateReference)
                            {
                                mostSimilarNode = wantedNode;
                                mostSimilarNodeFound = true;
                                break;
                            }
                        }
                    }

                    if (mostSimilarNodeFound)
                    {
                        break;
                    }

                    if (sameNodesCount > maxSameNodeCount)
                    {
                        maxSameNodeCount = sameNodesCount;
                        mostSimilarNode = wantedNode;
                    }
                }

                if (mostSimilarNode.HasValue && _referenceImmersionValidator.ValidateReferenceImmersion(referenceChain, findingNode.Key, compressParameters.MaxReferenceChainSize))
                {
                    if (referenceChain.ContainsKey(mostSimilarNode.Value.Key))
                    {
                        referenceChain[mostSimilarNode.Value.Key].Add(findingNode.Key);
                    }
                    else
                    {
                        referenceChain.Add(mostSimilarNode.Value.Key, new HashSet<int> { findingNode.Key });
                    }
                }
                else
                {
                    mostSimilarNode = null;
                }

                var similarNode = SimilarNodeFactory.CreateSimilarNodeFromKeyValuePairs(findingNode, mostSimilarNode);

                similarNodes.Add(similarNode);
            }

            //Pridani posledniho uzlu, ktery byl vynechan v cyklu protoze nikdy nebude referencovat žádný jiný uzel (zabránení cyklení)
            var lastNode = SimilarNodeFactory.CreateSimilarNodeFromKeyValuePairs(sortedGraphStructure[sortedGraphStructure.Count - 1]);
            similarNodes.Add(lastNode);

            return similarNodes;
        }
    }
}
