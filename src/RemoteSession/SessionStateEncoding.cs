using System;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace RemoteSession
{
    public class SessionStateEncoding
    {
        public class InvalidSessionStateException : Exception { 
         public InvalidSessionStateException() : this(null) {}
         public InvalidSessionStateException(Exception e) : base("Invalid session state", e) {}}

        public byte[] Serialize(SessionStateStoreData sessionState)
        {
            var hasItems = sessionState.Items != null && sessionState.Items.Count > 0;
            var hasStaticObjects = sessionState.StaticObjects != null && !sessionState.StaticObjects.NeverAccessed;
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(sessionState.Timeout);
            writer.Write(hasItems);
            writer.Write(hasStaticObjects);
            if (hasItems) ((SessionStateItemCollection)sessionState.Items).Serialize(writer);
            if (hasStaticObjects) sessionState.StaticObjects.Serialize(writer);
            writer.Write(byte.MaxValue);
            return stream.ToArray();
        }

        public SessionStateStoreData Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0) throw new InvalidSessionStateException();
            try
            {
                var reader = new BinaryReader(new MemoryStream(data));
                var timeout = reader.ReadInt32();
                var hasItems = reader.ReadBoolean();
                var hasStaticObjects = reader.ReadBoolean();
                var stateItemCollection = !hasItems ? new SessionStateItemCollection() : SessionStateItemCollection.Deserialize(reader);
                var staticObjects = !hasStaticObjects ? new HttpStaticObjectsCollection() : HttpStaticObjectsCollection.Deserialize(reader);
                if (reader.ReadByte() != byte.MaxValue) throw new HttpException("Invalid session state");
                return new SessionStateStoreData(stateItemCollection, staticObjects, timeout);
            }
            catch (Exception e) { throw new InvalidSessionStateException(e); }
        }
    }
}