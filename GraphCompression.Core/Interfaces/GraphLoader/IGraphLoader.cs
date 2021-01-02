using System.Collections.Generic;

namespace GraphProcessor.Core.Interfaces.GraphLoader
{
    public interface IGraphLoader
    {
        Dictionary<int, List<int>> Load(string filePath);
    }
}
