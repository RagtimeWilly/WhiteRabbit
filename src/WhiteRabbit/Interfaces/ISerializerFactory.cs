
namespace WhiteRabbit
{
    public interface ISerializerFactory
    {
        ISerializer For(string contentType);
    }
}
