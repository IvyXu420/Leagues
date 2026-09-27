using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Leagues.Views;

namespace Leagues.ViewModels;

public partial class MatchViewModel : ObservableObject
{
    private readonly MatchSearchView searchView;
    private readonly MatchStatsView statsView;

    public MatchViewModel()
    {
        statsView = new MatchStatsView(new MatchStatsViewModel(OpenSearchAsync));
        searchView = new MatchSearchView();
        CurrentViewModel = searchView;
    }


    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ShowSearchViewCommand))]
    [NotifyCanExecuteChangedFor(nameof(ShowStatsViewCommand))]
    public partial object CurrentViewModel { get; set; }

    [RelayCommand(CanExecute = nameof(CanShowSearchView))]
    private void ShowSearchView()
    {
        CurrentViewModel = searchView;
    }

    private bool CanShowSearchView() => CurrentViewModel.GetType() != typeof(MatchSearchView);

    [RelayCommand(CanExecute = nameof(CanShowStatsView))]
    private void ShowStatsView()
    {
        CurrentViewModel = statsView;
    }

    private bool CanShowStatsView() => CurrentViewModel.GetType() != typeof(MatchStatsView);

    internal async Task OpenSearchAsync(string playerName)
    {
        CurrentViewModel = searchView;
        await searchView.SearchPlayerAsync(playerName);
    }
}