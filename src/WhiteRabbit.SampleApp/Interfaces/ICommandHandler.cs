
namespace WhiteRabbit.SampleApp.Interfaces
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<in TCommand> : ICommandHandler
    {
        void Handle(TCommand cmd);
    }
}
