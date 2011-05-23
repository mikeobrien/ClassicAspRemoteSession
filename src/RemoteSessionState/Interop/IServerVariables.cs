namespace RemoteSessionState.Interop
{
    public interface IServerVariables
    {
        object this[string name] { get; }
    }
}