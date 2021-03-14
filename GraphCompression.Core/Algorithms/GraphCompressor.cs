﻿using GraphCompression.Core.Code;
using GraphCompression.Core.Factory;
using GraphCompression.Core.Interfaces.Algorithms;
using GraphCompression.Core.Interfaces.Code;
using GraphCompression.Core.Interfaces.Model;
using GraphCompression.Core.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GraphCompression.Core.Algorithms
{
    /// <summary>
    /// Class <c>GraphCompressor</c> generates compress graph structure 
    /// </summary>
    public class GraphCompressor : IGraphCompressor
    {
        private readonly CompressParameters _parameters;

        /// <summary>
        /// Graph compressor constructor
        /// </summary>
        /// <param name="graphCompresorParameters">Parameters for compression</param>
        public GraphCompressor(CompressParameters graphCompresorParameters)
        {
            _parameters = graphCompresorParameters;
        }

        /// <summary>
        /// Generates graph with compressed structure
        /// </summary>
        /// <param name="originalGraph">Original graph</param>
        /// <param name="sortingAlgorithm">Class with algorithm which sorts nodes of original graph</param>
        /// <param name="similarNodeProcessor">Class with method which returns list of the most similar nodes of original graph</param>
        /// <returns>Compressed graph structure</returns>
        public CompressedGraph Compress(IGraph originalGraph, IGraphSortingAlgorithm sortingAlgorithm, ISimilarNodeProcessor similarNodeProcessor)
        {
            var sortedGraphStructure = sortingAlgorithm.GetSortedGraphStructure(originalGraph.RawGraphStructure);

            var similarNodes = similarNodeProcessor.CreateListOfSimilarNodes(sortedGraphStructure, _parameters);

            var compressedGraph = CreateCompressedGraph(similarNodes);

            return compressedGraph;
        }

        private CompressedGraph CreateCompressedGraph(IEnumerable<SimilarNode> similarNodes)
        {
            var compressedGraph = new CompressedGraph();

            foreach (var similarNode in similarNodes)
            {
                //If node not constains referenction or referenction has more neighbors than specified maximal neighbors count 
                if (!ConditionalExpressions.SimilarNodeContainsReference(similarNode) || 
                     ConditionalExpressions.IsReferenceNeighborCountHigherThanMaxReferenceListSize(similarNode, _parameters.MaxReferenceListSize))
                {
                    var compressedNodeWithoutReference = CompressedNodeFactory.CreateCompressedNodeFromSimilarNodeWithoutReference(similarNode);
                    compressedGraph.AddNode(compressedNodeWithoutReference);
                }
                else
                {
                    var (referenceList, extraNodes) = GetReferenceListAndExtraNodes(similarNode);

                    var compressedNode = new CompressedNode
                    {
                        Id = similarNode.Node1,
                        ReferenceId = similarNode.Node2,
                        ReferenceList = referenceList,
                        ExtraNodes = extraNodes,
                    };

                    compressedGraph.AddNode(compressedNode);
                }

            }

            return compressedGraph;
        }

        private (BitArray, SortedSet<int>) GetReferenceListAndExtraNodes(SimilarNode similarNode)
        {
            var referenceList = new BitArray(similarNode.Neighbors2.Count);

            var pairedReferencingNodeNeighbors = new HashSet<int>();

            for (int referencedNodeNeighborIndex = 0; referencedNodeNeighborIndex < similarNode.Neighbors2.Count; referencedNodeNeighborIndex++)
            {
                var referencesNodeNeighborKey = similarNode.Neighbors2.Keys[referencedNodeNeighborIndex];
                var referencedNodeNeighbor = similarNode.Neighbors2[referencesNodeNeighborKey];

                var isNodeInReferencingNode = false;

                for (int referencingNodeNeighorIndex = 0; referencingNodeNeighorIndex < similarNode.Neighbors1.Count; referencingNodeNeighorIndex++)
                {
                    var referencingNodeNeighborKey = similarNode.Neighbors1.Keys[referencingNodeNeighorIndex];
                    var referencingNodeNeighbor = similarNode.Neighbors1[referencingNodeNeighborKey];

                    if (referencingNodeNeighbor == referencedNodeNeighbor)
                    {
                        isNodeInReferencingNode = true;
                        pairedReferencingNodeNeighbors.Add(referencingNodeNeighbor);
                        break;
                    }

                }

                referenceList.Set(referencedNodeNeighborIndex, isNodeInReferencingNode);
            }

            var extraNodeList = similarNode.Neighbors1.Where(x => !pairedReferencingNodeNeighbors.Contains(x.Value)).Select(x => x.Value);
            var extraNodes = new SortedSet<int>(extraNodeList);

            return (referenceList, extraNodes);
        }
    }
}
