using GraphCompression.Core.Interfaces.Model;
using GraphCompression.Core.Models;

namespace GraphCompression.Core.Interfaces.Algorithms
{
    public interface IGraphCompressor
    {
        CompressedGraph Compress(IGraph originalGraph, CompressParameters graphCompresorParameters);
    }
}
