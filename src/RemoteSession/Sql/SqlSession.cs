namespace RemoteSession.Sql
{
    public class SqlSession
    {
        public string SessionId { get; set; }
        public byte[] Data { get; set; }
        public bool Locked { get; set; }
        public int LockAge { get; set; }
        public int LockCookie { get; set; }
        public int ActionFlags { get; set; }
        public bool Empty { get { return ActionFlags == 0; } }
    }
}