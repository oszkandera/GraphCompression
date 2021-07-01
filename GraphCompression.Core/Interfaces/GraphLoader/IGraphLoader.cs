using GraphCompression.Core.Interfaces.Model;
using System.Collections.Generic;

namespace GraphCompression.Core.Interfaces.GraphLoader
{
    public interface IGraphLoader
    {
        (IGraph, Dictionary<string, int>) Load(string filePath);
    }
}
