using GraphCompression.Core.Interfaces.GraphLoader;
using GraphCompression.Core.Interfaces.Model;
using GraphCompression.Core.Models;
using GraphCompression.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphCompression.Core.GraphLoader
{
    public abstract class GraphLoaderBase : IAnonymizableGraphLoader
    {
        protected abstract List<Tuple<string, string>> ProcessRawData(string filePath);

        public (IGraph, Dictionary<string, int>) Load(string filePath)
        {
            return Load(filePath, false);
        }

        public (IGraph, Dictionary<string, int>) LoadAnonymized(string filePath)
        {
            return Load(filePath, true);
        }

        private (IGraph, Dictionary<string, int>) Load(string filePath, bool withAnonymization)
        {
            var graph = new Graph();

            var adjacencyList = ProcessRawData(filePath);

            var anonymizedNodePairMap = GetMapOfAnonymizedNodes(adjacencyList, withAnonymization);

            foreach (var nodePair in adjacencyList)
            {
                var nodeX = anonymizedNodePairMap[nodePair.Item1];
                var nodeY = anonymizedNodePairMap[nodePair.Item2];

                graph.AddBidirectionEdge(nodeX, nodeY);
            }


            return (graph, anonymizedNodePairMap);
        }

        private Dictionary<string, int> GetMapOfAnonymizedNodes(List<Tuple<string, string>> adjacencyList, bool withAnonymization)
        {
            var distinctNodes = GetDistinctNodes(adjacencyList);
            var countOfDistinctNodes = distinctNodes.Count;

            var stack = PrepareStack(countOfDistinctNodes);

            if (withAnonymization)
            {
                stack.Shuffle();
            }

            var anonymizedNodePair = new Dictionary<string, int>();

            foreach (var node in distinctNodes)
            {
                var nodeId = stack.Pop();
                anonymizedNodePair.Add(node, nodeId);
            }

            return anonymizedNodePair;
        }

        private List<string> GetDistinctNodes(List<Tuple<string, string>> adjacencyList)
        {
            var allXNodes = adjacencyList.Select(x => x.Item1);
            var allYNodes = adjacencyList.Select(x => x.Item2);

            var allNodes = allXNodes.Union(allYNodes);

            var distinctNodes = allNodes.Distinct().ToList();

            return distinctNodes;
        }

        private Stack<int> PrepareStack(int countOfDistinctNumbers)
        {
            var stack = new Stack<int>();

            for (int i = 0; i < countOfDistinctNumbers; i++)
            {
                stack.Push(i);
            }

            return stack;
        }
    }
}
