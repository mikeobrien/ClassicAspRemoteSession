namespace RemoteSession.Interop
{
    public interface ICookies
    {
        string this[string name] { get; set; }
    }
}