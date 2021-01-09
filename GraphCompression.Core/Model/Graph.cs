using GraphCompression.Core.Interfaces.Model;
using System;
using System.Collections.Generic;

namespace GraphCompression.Core.Model
{
    public class Graph<T> : IGraph<T>
    {
        private readonly Dictionary<T, SortedList<T, T>> _graph;

        public Graph()
        {
            _graph = new Dictionary<T, SortedList<T, T>>();
        }

        public void AddNode(T node)
        {
            if (_graph.ContainsKey(node))
            {
                throw new Exception("NodeAlreadyExists");
            } 
            
            _graph.Add(node, new SortedList<T, T>());
        }

        public void AddEdge(T nodeX, T nodeY)
        {
            if (!_graph.ContainsKey(nodeX)) _graph.Add(nodeX, new SortedList<T, T>());
            if (!_graph.ContainsKey(nodeY)) _graph.Add(nodeY, new SortedList<T, T>());

            if (!_graph[nodeX].ContainsValue(nodeY)) _graph[nodeX].Add(nodeY, nodeY);
            if(!_graph[nodeY].ContainsValue(nodeX)) _graph[nodeY].Add(nodeX, nodeX);
        }

        public Node<T> GetNode(T node)
        {
            if (!_graph.ContainsKey(node))
            {
                throw new ArgumentException("Graph does not contain node with this identificator");
            }

            var result = new Node<T>
            {
                NodeIdentifier = node,
                Neighbors = _graph[node]
            };

            return result;
        }
    }
}
