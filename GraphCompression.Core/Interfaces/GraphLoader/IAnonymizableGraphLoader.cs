using GraphCompression.Core.Interfaces.Model;
using System.Collections.Generic;

namespace GraphCompression.Core.Interfaces.GraphLoader
{
    public interface IAnonymizableGraphLoader : IGraphLoader
    {
        (IGraph, Dictionary<string, int>) LoadAnonymized(string filePath);
    }
}
