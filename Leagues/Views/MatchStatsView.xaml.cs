using System.Windows.Controls;
using Leagues.ViewModels;

namespace Leagues;

public partial class MatchStatsView : UserControl
{
    public MatchStatsView(MatchStatsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}