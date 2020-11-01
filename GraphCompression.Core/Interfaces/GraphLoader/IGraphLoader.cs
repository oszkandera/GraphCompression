using System.Collections.Generic;

namespace GraphProcessor.Core.Interfaces.GraphLoader
{
    public interface IGraphLoader
    {
        Dictionary<int, HashSet<int>> Load(string filePath);
    }
}
