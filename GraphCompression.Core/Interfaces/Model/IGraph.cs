using GraphCompression.Core.Model;

namespace GraphCompression.Core.Interfaces.Model
{
    public interface IGraph<T>
    {
        void AddNode(T node);
        void AddEdge(T nodeX, T nodeY);
        Node<T> GetNode(T node);
    }
}
