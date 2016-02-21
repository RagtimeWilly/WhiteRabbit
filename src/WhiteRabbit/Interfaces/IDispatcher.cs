
namespace WhiteRabbit
{
    public interface IDispatcher
    {
        void Dispatch<TMsg>(TMsg cmd);
    }
}
