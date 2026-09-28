using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;
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
    private readonly Dispatcher dispatcher;
    private bool isLoading;

    public MatchStatsViewModel(Func<string, Task> openSearch)
    {
        this.openSearch = openSearch;
        dispatcher = Application.Current.Dispatcher;
        HomeViewModel.PhaseMonitor.PhaseChanged += OnPhaseChanged;
        if (HomeViewModel.PhaseMonitor.CurrentPhase is { } phase)
            _ = LoadForPhaseSafelyAsync(phase);
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
        await LoadForPhaseSafelyAsync(phase);
    }

    private async Task LoadForPhaseSafelyAsync(string phase)
    {
        try
        {
            await LoadForPhaseAsync(phase);
        }
        catch (Exception ex)
        {
            await dispatcher.InvokeAsync(() => StatusText = $"Unable to load match stats: {ex.Message}");
        }
    }

    private Task LoadForPhaseAsync(string phase)
    {
        if (dispatcher.CheckAccess())
            return LoadForPhaseOnUiAsync(phase);

        return dispatcher.InvokeAsync(() => LoadForPhaseOnUiAsync(phase)).Task.Unwrap();
    }

    private Task LoadForPhaseOnUiAsync(string phase) => phase switch
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