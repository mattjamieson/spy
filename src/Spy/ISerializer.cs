namespace Spy
{
    using System.IO;

    public interface ISerializer
    {
        void Serialize<T>(T obj, Stream outputStream);
    }
}