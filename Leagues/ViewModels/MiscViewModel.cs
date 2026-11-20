using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Leagues.Client;

namespace Leagues.ViewModels;

public partial class MiscViewModel : ObservableObject
{
    private readonly ClientSettings settings;

    [ObservableProperty] public partial string SelectedLanguage { get; set; }

    // FIXME: The Tencent Client only supports these two language by default.
    // Doesn't seem to be the case for the Riot Client.
    public ObservableCollection<string> AvailableLanguages { get; } =
    [
        "zh_CN",
        "en_US"
    ];

    public MiscViewModel()
    {
        settings = new();
        SelectedLanguage = settings.LoadLocale() ?? AvailableLanguages[0];
    }

    partial void OnSelectedLanguageChanged(string value)
    {
        settings.SaveLocale(value);
    }
}