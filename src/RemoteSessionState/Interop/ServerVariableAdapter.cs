namespace RemoteSessionState.Interop
{
    public class ServerVariableAdapter : IServerVariables
    {
        private readonly dynamic _serverVariables;

        public ServerVariableAdapter(dynamic serverVariables)
        {
            _serverVariables = serverVariables;
        }

        public object this[string name]
        {
            get { return _serverVariables.Item(name)(); }
        }
    }
}
