using GraphCompression.Core.Interfaces.Model;
using System.Collections;
using System.Collections.Generic;
namespace GraphCompression.Core.Models
{
    public abstract class GraphBase : IGraph
    {
        /// <summary>
        /// Returns number of nodes in graph
        /// </summary>
        public int Size => Graph == null ? 0 : Graph.Count;

        private Dictionary<int, List<int>> _graph;
        protected Dictionary<int, List<int>> Graph => _graph != null ? _graph : _graph = new Dictionary<int, List<int>>();

        public abstract void AddEdge(int nodeX, int nodeY);
        public abstract void AddBidirectionEdge(int nodeX, int nodeY);

        public abstract void AddNode(int node);
        public abstract Node GetNode(int node);

        public IEnumerator<Node> GetEnumerator()
        {
            foreach(var node in Graph)
            {
                yield return new Node
                {
                    NodeIdentifier = node.Key,
                    Neighbors = node.Value
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
