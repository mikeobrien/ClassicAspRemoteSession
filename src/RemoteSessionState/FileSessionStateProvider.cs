using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.SessionState;

namespace RemoteSessionState
{
    public class FileSessionStateProvider : ISessionStateProvider
    {
        private readonly string _path;

        public FileSessionStateProvider() : 
            this(Path.GetTempPath()) { }

        public FileSessionStateProvider(string sessionPath)
        {
            _path = sessionPath;
        }

        public string GetSessionFilePath(ISessionStateContext context)
        {
            return Path.Combine(_path, string.Format("{0}.session", context.SessionId));
        }

        public IDictionary<string, object> Load(ISessionStateContext context)
        {
            var path = GetSessionFilePath(context);
            if (!File.Exists(path)) return null;
            SessionStateItemCollection collection;
            using (var file = File.OpenRead(path))
            using (var reader = new BinaryReader(file))
                collection = SessionStateItemCollection.Deserialize(reader);
            return collection.Cast<object>().ToDictionary(item => (string) item, item => collection[(string) item]);
        }
         
        public void Save(ISessionStateContext context, IDictionary<string, object> values)
        {
            var collection = new SessionStateItemCollection();
            foreach (var value in values) collection[value.Key] = value.Value;
            using (var file = File.OpenWrite(GetSessionFilePath(context)))
            using (var writer = new BinaryWriter(file)) collection.Serialize(writer);
        }

        public void Abandon(ISessionStateContext context)
        {
            File.Delete(GetSessionFilePath(context));
        }
    }
}