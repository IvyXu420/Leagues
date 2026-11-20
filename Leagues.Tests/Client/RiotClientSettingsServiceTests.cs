using System.IO;
using Leagues.Client;

namespace Leagues.Tests.Client;

public class ClientSettingsTests
{
    [Fact]
    public void SaveLocale_UpdatesInstallGlobalsLocale()
    {
        var settingsPath = Path.Combine(Path.GetTempPath(), $"RiotClientSettings-{Guid.NewGuid():N}.yaml");
        File.WriteAllText(settingsPath,
            "install:\n    localization:\n        locale: \"zh_CN\"\n        region: \"TENCENT\"\n");

        try
        {
            var settings = new ClientSettings(settingsPath);

            Assert.Equal("zh_CN", settings.LoadLocale());
            Assert.True(settings.SaveLocale("en_US"));
            Assert.Equal("en_US", settings.LoadLocale());

            Assert.Contains("locale: \"en_US\"", File.ReadAllText(settingsPath));
        }
        finally
        {
            File.Delete(settingsPath);
        }
    }
}