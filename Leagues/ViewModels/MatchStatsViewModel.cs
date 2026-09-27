using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Leagues.Models.Client;
using Leagues.Models.Services;
using static Leagues.Models.Client.LcuConnection;

namespace Leagues.ViewModels;

public partial class MatchStatsViewModel : ObservableObject
{
    private readonly Func<string, Task> openSearch;
    private bool isLoading;

    public MatchStatsViewModel(Func<string, Task> openSearch)
    {
        this.openSearch = openSearch;
        HomeViewModel.PhaseMonitor.PhaseChanged += OnPhaseChanged;
        if (HomeViewModel.PhaseMonitor.CurrentPhase is { } phase)
            _ = LoadForPhaseAsync(phase);
    }

    public ObservableCollection<MatchStatItemViewModel> Players { get; } = [];

    [ObservableProperty] public partial string StatusText { get; private set; } = "Not in a match.";

    private async Task LoadSummonerStatsAsync(bool friend)
    {
        if (isLoading)
            return;

        isLoading = true;
        StatusText = friend ? "Loading teammates..." : "Loading enemies...";
        Players.Clear();
        var stats = await MatchStats.LoadAsync(friend);
        foreach (var stat in stats)
        {
            var item = new MatchStatItemViewModel(stat, openSearch);
            Players.Add(item);
            _ = item.LoadAvatarAsync();
        }

        StatusText = Players.Count == 0 ? "No players found for this match." : "";
        isLoading = false;
    }

    private async void OnPhaseChanged(object? sender, string phase)
    {
        await LoadForPhaseAsync(phase);
    }

    private Task LoadForPhaseAsync(string phase) => phase switch
    {
        "ChampSelect" => LoadSummonerStatsAsync(friend: true),
        "GameStart" or "InProgress" => LoadSummonerStatsAsync(friend: false),
        _ => Task.CompletedTask
    };
}

public partial class MatchStatItemViewModel(SummonerStats stats, Func<string, Task> openSearch) : ObservableObject
{
    public SummonerStats Stats { get; } = stats;

    [ObservableProperty] public partial BitmapImage? Avatar { get; private set; }

    [RelayCommand]
    private Task OpenSearch() => openSearch(Stats.PlayerName);

    public async Task LoadAvatarAsync()
    {
        try
        {
            var bytes = await LcuHttpClient.GetByteArrayAsync(LcuEndPoint.ProfileIcon(Stats.ProfileIconId));
            using var stream = new MemoryStream(bytes);
            var avatar = new BitmapImage();
            avatar.BeginInit();
            avatar.CacheOption = BitmapCacheOption.OnLoad;
            avatar.StreamSource = stream;
            avatar.EndInit();
            avatar.Freeze();
            Avatar = avatar;
        }
        catch
        {
            Avatar = null;
        }
    }
}