using System.Configuration;
using System.Web.Configuration;

namespace RemoteSessionState
{
    public class WebConfig
    {
        private const string VirtualDirectory = "/";
        private readonly string _path;
        private Configuration _configuration;

        public WebConfig(string path)
        {
            _path = path;
        }

        public T GetSection<T>(string section) where T : class
        {
            EnsureInitialized();
            return _configuration.GetSection(section) as T;
        }

        private void EnsureInitialized()
        {
            if (_configuration != null) return;
            var webConfigurationFileMap = new WebConfigurationFileMap();
            webConfigurationFileMap.VirtualDirectories.Add(VirtualDirectory, new VirtualDirectoryMapping(_path, true));
            _configuration = WebConfigurationManager.OpenMappedWebConfiguration(webConfigurationFileMap, VirtualDirectory);
        }
    }
}