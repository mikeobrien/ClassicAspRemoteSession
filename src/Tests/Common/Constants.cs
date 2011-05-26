using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Common
{
    public class Constants
    {
        // Session 

        public const int SessionTimeout = 20;
        public const string MetabasePath = "/LM/W3SVC/1/ROOT";
        public const int ApplicationId = 538231025;
        public const string ApplicationIdEncoded = "2014c0f1";
        public const string SessionId = "1sc3aoog5u5zyko2tl3ghnvq";
        public const string FullSessionId = SessionId + ApplicationIdEncoded;
        public const string ConnectionString = "server=localhost;database=ASPNetSessionState;Integrated Security=SSPI";

        // Session data 

        public const string SessionStateEmptySerializedHex = "140000000000FF";

        public static readonly byte[] SessionStateEmptySerializedBytes = Enumerable.Range(0, SessionStateEmptySerializedHex.Length).
                                                                          Where(x => x % 2 == 0).
                                                                          Select(x => Convert.ToByte(SessionStateEmptySerializedHex.Substring(x, 2), 16)).
                                                                          ToArray();

        public const int SessionStateItemCount = 3;

        public const string SessionStateKey1 = "name", SessionStateValue1 = "Ed";
        public const string SessionStateKey2 = "age"; public const int SessionStateValue2 = 44;
        public const string SessionStateKey3 = "price"; public const double SessionStateValue3 = 34.5;

        public static readonly IDictionary<string, object> SessionState = new Dictionary<string, object> 
                { { SessionStateKey1, SessionStateValue1 }, { SessionStateKey2, SessionStateValue2 }, 
                { SessionStateKey3, SessionStateValue3 } };

        public const string SessionStateSerializedHex = "14000000010003000000FFFFFFFF046E616D650361676505707269636504" + 
                                                        "000000090000001200000001024564022C000000090000000000404140FF";

        public static readonly byte[] SessionStateSerializedBytes = Enumerable.Range(0, SessionStateSerializedHex.Length).
                                                                          Where(x => x % 2 == 0).
                                                                          Select(x => Convert.ToByte(SessionStateSerializedHex.Substring(x, 2), 16)).
                                                                          ToArray();

        // Multi data type session data

        public const int SessionStateMultiDataTypeValidItemCount = 8;

        public const string SessionStateStringKey = "string", SessionStateStringValue = "string";
        public const string SessionStateByteKey = "byte"; public const byte SessionStateByteValue = 25;
        public const string SessionStateBoolKey = "bool"; public const bool SessionStateBoolValue = true;
        public const string SessionStateShortKey = "short"; public const short SessionStateShortValue = 500;
        public const string SessionStateIntKey = "int"; public const int SessionStateIntValue = 700;
        public const string SessionStateDoubleKey = "double"; public const double SessionStateDoubleValue = 44.5;
        public const string SessionStateFloatKey = "float"; public const float SessionStateFloatValue = 36.7F;
        public const string SessionStateDateTimeKey = "datetime"; public static readonly DateTime SessionStateDateTimeValue = DateTime.MinValue;
        public const string SessionStateGuidKey = "guid"; public static readonly Guid SessionStateGuidValue = Guid.Empty;

        public static readonly IDictionary<string, object> SessionStateMultiDataType = new Dictionary<string, object> {
                    { SessionStateStringKey, SessionStateStringValue }, { SessionStateByteKey, SessionStateByteValue }, 
                    { SessionStateBoolKey, SessionStateBoolValue }, { SessionStateShortKey, SessionStateShortValue }, 
                    { SessionStateIntKey, SessionStateIntValue }, { SessionStateDoubleKey, SessionStateDoubleValue }, 
                    { SessionStateFloatKey, SessionStateFloatValue }, { SessionStateDateTimeKey, SessionStateDateTimeValue }, 
                    { SessionStateGuidKey, SessionStateGuidValue } };

        public const string SessionStateMultiDataTypeSerializedHex = "14000000010009000000FFFFFFFF06737472696E67046279746504626F6F" +
                                                                     "6C0573686F727403696E7406646F75626C6505666C6F6174086461746574" +
                                                                     "696D650467756964080000000A0000000C0000000F000000140000001D00" +
                                                                     "0000220000002B0000003C0000000106737472696E67061903010BF40102" +
                                                                     "BC02000009000000000040464008CDCC1242040000000000000000110000" +
                                                                     "0000000000000000000000000000FF";

        public static readonly byte[] SessionStateMultiDataTypeSerializedBytes = Enumerable.Range(0, SessionStateMultiDataTypeSerializedHex.Length).
                                                                          Where(x => x % 2 == 0).
                                                                          Select(x => Convert.ToByte(SessionStateMultiDataTypeSerializedHex.Substring(x, 2), 16)).
                                                                          ToArray();
    }
}