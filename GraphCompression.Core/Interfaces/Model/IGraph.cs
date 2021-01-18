using GraphCompression.Core.Models;
using System.Collections.Generic;

namespace GraphCompression.Core.Interfaces.Model
{
    public interface IGraph : IEnumerable<Node>
    {
        int Size { get; }

        void AddNode(int node);
        void AddEdge(int nodeX, int nodeY);
        void AddBidirectionEdge(int nodeX, int nodeY);
        Node GetNode(int nodeIdentifier);
    }
}
