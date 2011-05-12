namespace RemoteSession.Asp
{
    public interface IServerVariables
    {
        object this[string name] { get; }
    }
}