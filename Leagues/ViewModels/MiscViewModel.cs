using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Leagues.Models.Services;

namespace Leagues.ViewModels;

public partial class MiscViewModel : ObservableObject
{
    private readonly RiotClientSettingsService settingsService;

    [ObservableProperty] public partial string SelectedLanguage { get; private set; }

    public ObservableCollection<string> AvailableLanguages { get; } =
    [
        "zh_CN",
        "en_US"
    ];

    public MiscViewModel()
    {
        settingsService = new RiotClientSettingsService();
        SelectedLanguage = settingsService.LoadLocale() ?? AvailableLanguages[0];
    }

    partial void OnSelectedLanguageChanged(string value)
    {
        settingsService.SaveLocale(value);
    }
}