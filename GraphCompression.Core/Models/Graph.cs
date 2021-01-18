using System;
using System.Collections.Generic;

namespace GraphCompression.Core.Models
{
    public class Graph : GraphBase
    {
        public override void AddNode(int node)
        {
            if (Graph.ContainsKey(node))
            {
                throw new Exception("NodeAlreadyExists");
            } 
            
            Graph.Add(node, new List<int>());
        }

        public override void AddEdge(int nodeX, int nodeY)
        {
            if (!Graph.ContainsKey(nodeX)) Graph.Add(nodeX, new List<int>());

            if (!Graph[nodeX].Contains(nodeY)) Graph[nodeX].Add(nodeY);
        }

        public override void AddBidirectionEdge(int nodeX, int nodeY)
        {
            AddEdge(nodeX, nodeY);
            AddEdge(nodeY, nodeX);
        }

        public override Node GetNode(int node)
        {
            if (!Graph.ContainsKey(node))
            {
                throw new ArgumentException("Graph does not contain node with this identificator");
            }

            var result = new Node
            {
                NodeIdentifier = node,
                Neighbors = Graph[node]
            };

            return result;
        }
    }
}
