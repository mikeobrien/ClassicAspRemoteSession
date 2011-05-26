namespace RemoteSessionState
{
    public class SqlSession
    {
        public SqlSessionId Id { get; set; }
        public byte[] Data { get; set; }
        public bool Locked { get; set; }
        public int LockAge { get; set; }
        public int LockCookie { get; set; }
        public int ActionFlags { get; set; }
        public bool Empty { get { return Data == null || Data.Length == 0; } }

        public static bool IsShortData(byte[] data)
        {
            return data.Length <= 7000;
        }
    }
}