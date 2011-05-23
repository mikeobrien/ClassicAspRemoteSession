namespace RemoteSessionState.Interop
{
    public interface IHttpContext
    {
        IServerVariables ServerVariables { get; }
        ICookies RequestCookies { get; }
        ICookies ResponseCookies { get; }
        ISessionState SessionState { get; }
    }
}