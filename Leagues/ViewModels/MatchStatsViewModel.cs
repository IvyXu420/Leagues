using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Leagues.Services;

namespace Leagues.ViewModels;

public partial class MatchStatsViewModel : ObservableObject
{
    private readonly Func<string, Task> openSearch;
    private readonly Dispatcher dispatcher;
    private readonly DispatcherTimer statsPollTimer = new() { Interval = TimeSpan.FromSeconds(2) };
    private bool isLoading;
    private bool? loaded;
    public MatchStatsViewModel(Func<string, Task> openSearch)
    {
        this.openSearch = openSearch;
        dispatcher = Application.Current.Dispatcher;
        statsPollTimer.Tick += StatsPollTimer_Tick;
        statsPollTimer.Start();
    }

    public ObservableCollection<MatchStatsItemViewModel> Players { get; } = [];

    [ObservableProperty] public partial string StatusText { get; private set; } = "Not in a match.";

    private async Task LoadSummonerStatsAsync(bool ingame)
    {
        if (isLoading || loaded == ingame)
            return;

        isLoading = true;
        try
        {
            var stats = await ParticipantStatsService.LoadAsync(ingame);
            Players.Clear();
            foreach (var stat in stats)
            {
                var item = new MatchStatsItemViewModel(stat, openSearch);
                Players.Add(item);
                _ = item.LoadAvatarAsync(ingame);
            }

            if (Players.Count == 0)
                StatusText = "No players found for this match.";
            else
            {
                StatusText = string.Empty;
                loaded = ingame;
            }
        }
        finally
        {
            isLoading = false;
        }
    }

    private async void StatsPollTimer_Tick(object sender, EventArgs e)
    {
        await LoadCurrentPhaseSafelyAsync();
    }

    private async Task LoadCurrentPhaseSafelyAsync()
    {
        try
        {
            var phase = HomeViewModel.PhaseMonitor.CurrentPhase;
            if (phase is null)
            {
                await ClearStatsAsync();
                return;
            }

            await LoadForPhaseAsync(phase);
        }
        catch (Exception ex)
        {
            await dispatcher.InvokeAsync(() => StatusText = $"Unable to load match stats: {ex.Message}");
        }
    }

    private Task LoadForPhaseAsync(string phase)
    {
        return dispatcher.CheckAccess() ? LoadForPhaseOnUiAsync(phase) : dispatcher.InvokeAsync(() => LoadForPhaseOnUiAsync(phase)).Task.Unwrap();
    }

    private Task LoadForPhaseOnUiAsync(string phase) => phase switch
    {
        "ChampSelect" => LoadSummonerStatsAsync(ingame: false),
        "GameStart" or "InProgress" => LoadSummonerStatsAsync(ingame: true),
        _ => ClearStatsAsync()
    };

    private Task ClearStatsAsync()
    {
        Players.Clear();
        StatusText = "Not in a match.";
        loaded = null;
        return Task.CompletedTask;
    }
}