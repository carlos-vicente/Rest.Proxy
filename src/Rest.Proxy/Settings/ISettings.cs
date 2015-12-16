namespace Rest.Proxy.Settings
{
    public interface ISettings
    {
        string GetBaseUrl(string settingName);
    }
}
