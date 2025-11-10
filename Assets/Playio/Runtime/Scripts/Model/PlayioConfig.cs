using System;

namespace PlayioSDK
{
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERROR,
        NONE
    }

    public class PlayioConfig
    {
        public string clientId { get; private set; }
        public LogLevel logLevel { get; private set; }

        private PlayioConfig() { }

        public class Builder
        {
            private string clientId;
            private LogLevel logLevel = LogLevel.INFO;
            public Builder SetClientId(string clientId)
            {
                this.clientId = clientId;
                return this;
            }

            public Builder SetLogLevel(LogLevel logLevel)
            {
                this.logLevel = logLevel;
                return this;
            }

            public PlayioConfig Build()
            {
                if (string.IsNullOrEmpty(this.clientId))
                {
                    throw new System.Exception("PlayioConfig: Client ID must be set.");
                }
                return new PlayioConfig
                {
                    clientId = this.clientId,
                    logLevel = this.logLevel
                };
            }
        }
    }
}