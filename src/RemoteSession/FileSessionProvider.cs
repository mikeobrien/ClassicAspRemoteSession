using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.SessionState;

namespace RemoteSession
{
    public class FileSessionProvider : ISessionProvider
    {
        private readonly string _path;

        public FileSessionProvider() : 
            this(Path.GetTempPath()) { }

        public FileSessionProvider(string sessionPath)
        {
            _path = sessionPath;
        }

        public string GetSessionFilePath(Context context)
        {
            return Path.Combine(_path, string.Format("{0}.session", context.SessionId));
        }

        public IDictionary<string, object> Load(Context context)
        {
            var path = GetSessionFilePath(context);
            if (!File.Exists(path)) return new Dictionary<string, object>();
            SessionStateItemCollection collection;
            using (var file = File.OpenRead(path))
            using (var reader = new BinaryReader(file))
                collection = SessionStateItemCollection.Deserialize(reader);
            return collection.Cast<object>().ToDictionary(item => (string) item, item => collection[(string) item]);
        }
         
        public void Save(Context context, IDictionary<string, object> values)
        {
            var collection = new SessionStateItemCollection();
            foreach (var value in values) collection[value.Key] = value.Value;
            using (var file = File.OpenWrite(GetSessionFilePath(context)))
            using (var writer = new BinaryWriter(file)) collection.Serialize(writer);
        }

        public void Abandon(Context context)
        {
            File.Delete(GetSessionFilePath(context));
        }
    }
}