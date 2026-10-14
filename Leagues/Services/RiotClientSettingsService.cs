using System.IO;
using System.Text;
using Leagues.Utils;
using YamlDotNet.RepresentationModel;

namespace Leagues.Services;

public sealed class RiotClientSettingsService(string? settingPath = null)
{
    private static readonly string SettingFileName = "RiotClientSettings.yaml";

    private string? SettingPath { get; } = settingPath ?? FindSettingsPath();

    public string? LoadLocale()
    {
        if (string.IsNullOrWhiteSpace(SettingPath) || !File.Exists(SettingPath))
            return null;

        var yaml = LoadYaml();
        return GetLocaleNode(yaml)?.Value;
    }

    public bool SaveLocale(string locale)
    {
        if (string.IsNullOrWhiteSpace(SettingPath) ||
            SettingPath is null ||
            !File.Exists(SettingPath) || string.IsNullOrWhiteSpace(locale))
            return false;

        var yaml = LoadYaml();
        var localeNode = GetLocaleNode(yaml);
        if (localeNode is null)
            return false;

        localeNode.Value = locale;
        using var writer = new StringWriter();
        yaml.Save(writer, assignAnchors: false);
        File.WriteAllText(SettingPath, writer.ToString(), Encoding.UTF8);
        return true;
    }

    private YamlStream LoadYaml()
    {
        var yaml = new YamlStream();
        using var reader = File.OpenText(SettingPath!);
        yaml.Load(reader);
        return yaml;
    }

    private static YamlScalarNode? GetLocaleNode(YamlStream yaml)
    {
        if (yaml.Documents.Count == 0 ||
            yaml.Documents[0].RootNode is not YamlMappingNode root)
            return null;

        root.Children.TryGetValue("install", out var installNode);
        if (installNode is not YamlMappingNode install)
            return null;

        install.Children.TryGetValue("globals", out var globalsNode);
        if (globalsNode is not YamlMappingNode globals)
            return null;

        globals.Children.TryGetValue("locale", out var localeNode);
        return localeNode is not YamlScalarNode locale ? null : locale;
    }

    private static string? FindSettingsPath()
    {
        var clientPath = Registry.GetRiotClientPath();
        if (string.IsNullOrWhiteSpace(clientPath))
            return null;

        var directory = Directory.GetParent(clientPath);
        while (directory is not null)
        {
            var configDirectory = Path.Combine(directory.FullName, "Riot Client Data", "User Data", "Config");

            var path = Path.Combine(configDirectory, SettingFileName);
            if (File.Exists(path))
                return path;

            directory = directory.Parent;
        }

        return null;
    }
}