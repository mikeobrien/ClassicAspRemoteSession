namespace RemoteSession.Asp
{
    public interface ICookies
    {
        string this[string name] { get; set; }
    }
}