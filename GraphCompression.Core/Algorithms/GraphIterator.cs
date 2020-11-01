using GraphProcessor.Core.Interfaces.Algorithms;
using System.Collections.Generic;
using System.Linq;

namespace GraphProcessor.Core.Algorithms
{
    public class GraphIterator : IGraphIterator
    {
        public bool ExistsPathBetweenTwoEdges(Dictionary<int, HashSet<int>> graph, int from, int to)
        {
            var queue = new Queue<int>();
            var visitedNodes = new HashSet<int>();
            AddToQueue(queue, graph[from], visitedNodes);

            while (queue.Any())
            {
                var node = queue.Dequeue();

                if (node == to) return true;
                if (graph.ContainsKey(node)) AddToQueue(queue, graph[node], visitedNodes);
            }

            return false;
        }

        private void AddToQueue(Queue<int> queue, HashSet<int> values, HashSet<int> visitedNodes)
        {
            foreach (var value in values)
            {
                if (visitedNodes.Contains(value)) continue;

                queue.Enqueue(value);
                visitedNodes.Add(value);
            }
        }
    }
}
