using System;
using System.Web.Configuration;

namespace RemoteSessionState
{
    public class SystemWebConfiguration
    {
        private readonly WebConfig _webConfig;
        private readonly Lazy<SessionStateSection> _sessionState;

        public SystemWebConfiguration(string path)
        {
            _webConfig = new WebConfig(path);
            _sessionState = new Lazy<SessionStateSection>(() => _webConfig.GetSection<SessionStateSection>("system.web/sessionState"));
        }

        public SessionStateSection SessionState { get { return _sessionState.Value; } }
    }
}