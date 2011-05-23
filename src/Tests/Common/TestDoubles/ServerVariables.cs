using System;
using System.Collections.Generic;
using System.Reflection;
using RemoteSessionState;
using RemoteSessionState.Interop;

namespace Tests.Common.TestDoubles
{
    public class ServerVariables : IServerVariables
    {
        private readonly IDictionary<string, string> _items = new Dictionary<string, string>();

        public ServerVariables(string metabasePath)
        {
            _items[SessionStateContext.MetadataPathServerVariable] = metabasePath;
            _items[SessionStateContext.VirtualDirectoryPathServerVariable] = Environment.CurrentDirectory;
        }

        public object this[string name] 
        { get { return _items.ContainsKey(name) ? _items[name] : null; } }
    }
}