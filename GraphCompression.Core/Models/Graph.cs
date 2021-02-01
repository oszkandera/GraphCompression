using System;
using System.Collections.Generic;

namespace GraphCompression.Core.Models
{
    public class Graph : GraphBase
    {
        public override void AddNode(int node)
        {
            if (RawGraphStructure.ContainsKey(node))
            {
                throw new Exception("NodeAlreadyExists");
            }

            RawGraphStructure.Add(node, new List<int>());
        }

        public override void AddEdge(int nodeX, int nodeY)
        {
            if (!RawGraphStructure.ContainsKey(nodeX)) RawGraphStructure.Add(nodeX, new List<int>());

            if (!RawGraphStructure[nodeX].Contains(nodeY)) RawGraphStructure[nodeX].Add(nodeY);
        }

        public override void AddBidirectionEdge(int nodeX, int nodeY)
        {
            AddEdge(nodeX, nodeY);
            AddEdge(nodeY, nodeX);
        }

        public override Node GetNode(int node)
        {
            if (!RawGraphStructure.ContainsKey(node))
            {
                throw new ArgumentException("Graph does not contain node with this identificator");
            }

            var result = new Node
            {
                NodeIdentifier = node,
                Neighbors = RawGraphStructure[node]
            };

            return result;
        }
    }
}
