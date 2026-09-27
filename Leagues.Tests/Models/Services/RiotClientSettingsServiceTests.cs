using System.IO;
using Leagues.Models.Services;
using Leagues.ViewModels;

namespace Leagues.Tests.Models.Services;

public class RiotClientSettingsServiceTests
{
    [Fact]
    public void SaveLocale_UpdatesInstallGlobalsLocale()
    {
        var settingsPath = Path.Combine(Path.GetTempPath(), $"RiotClientSettings-{Guid.NewGuid():N}.yaml");
        File.WriteAllText(settingsPath,
            "install:\n    globals:\n        locale: \"zh_CN\"\n        region: \"TENCENT\"\n");

        try
        {
            var service = new RiotClientSettingsService(settingsPath);

            Assert.Equal("zh_CN", service.LoadLocale());
            Assert.True(service.SaveLocale("en_US"));
            Assert.Equal("en_US", service.LoadLocale());

            Assert.Contains("locale: \"en_US\"", File.ReadAllText(settingsPath));
        }
        finally
        {
            File.Delete(settingsPath);
        }
    }
}