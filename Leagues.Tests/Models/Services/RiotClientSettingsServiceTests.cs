using System.IO;
using Leagues.Services;

namespace Leagues.Tests.Models.Services;

public class RiotClientSettingsServiceTests
{
    [Fact]
    public void SaveLocale_UpdatesInstallGlobalsLocale()
    {
        var settingsPath = Path.Combine(Path.GetTempPath(), $"RiotClientSettings-{Guid.NewGuid():N}.yaml");
        File.WriteAllText(settingsPath,
            "install:\n    localization:\n        locale: \"zh_CN\"\n        region: \"TENCENT\"\n");

        try
        {
            var riotClientSetting = new RiotClientSettings(settingsPath);

            Assert.Equal("zh_CN", riotClientSetting.LoadLocale());
            Assert.True(riotClientSetting.SaveLocale("en_US"));
            Assert.Equal("en_US", riotClientSetting.LoadLocale());

            Assert.Contains("locale: \"en_US\"", File.ReadAllText(settingsPath));
        }
        finally
        {
            File.Delete(settingsPath);
        }
    }
}