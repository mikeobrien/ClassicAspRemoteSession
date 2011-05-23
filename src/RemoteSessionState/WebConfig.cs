using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RemoteSessionState
{
    public class WebConfig
    {
        public const string Filename = "web.config";
        private readonly XDocument _document;

        public WebConfig(string path)
        {
            _document = XDocument.Load(Path.Combine(path, Filename));
        }

        public T GetValue<T>(string xpath)
        {
            var result = ((IEnumerable<Object>) _document.XPathEvaluate(xpath)).FirstOrDefault();
            if (result is XAttribute) return (T)Convert.ChangeType(((XAttribute) result).Value, typeof(T));
            if (result is XElement) return (T)Convert.ChangeType(((XElement)result).Value, typeof(T));
            return default(T);
        }
    }
}