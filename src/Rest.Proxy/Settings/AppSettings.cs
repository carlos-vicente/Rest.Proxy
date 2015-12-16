using System;
using System.Configuration;

namespace Rest.Proxy.Settings
{
    public class AppSettings : ISettings
    {
        private const string SettingPrefix = "rest.proxy";

        public string GetBaseUrl(string settingName)
        {
            var completeSettingName = $"{SettingPrefix}:{settingName}";

            var settingValue = ConfigurationManager
                .AppSettings[completeSettingName];

            if(string.IsNullOrWhiteSpace(settingValue))
                throw new InvalidOperationException(
                    $"No entry found for setting {completeSettingName}");

            return settingValue;
        }
    }
}
