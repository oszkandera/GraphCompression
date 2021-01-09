using GraphCompression.Core.Model;
using System.Collections.Generic;

namespace GraphProcessor.Core.Interfaces.GraphLoader
{
    public interface IGraphLoader
    {
        Graph<int> Load(string filePath);
    }
}
