using System.IO;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Leagues.Client;

public partial class MatchStatItemViewModel(ParticipantStats stats, Func<string, Task> openSearch) : ObservableObject
{
    public ParticipantStats Stats { get; } = stats;

    [ObservableProperty] public partial BitmapImage? Avatar { get; private set; }

    [RelayCommand]
    private Task OpenSearch() => openSearch(Stats.PlayerName);

    /// <summary>
    /// Load champion avatar during the in-game phase;
    /// load summoner avatar during the champion-select phase.
    /// </summary>
    /// <returns></returns>
    public async Task LoadAvatarAsync(bool ingame)
    {
        var endpoint = ingame ? LcuEndPoint.ChampionAvatar(Stats.AvatarIconId) : LcuEndPoint.ProfileIcon(Stats.AvatarIconId);
        var bytes = await LcuNet.Client.GetByteArrayAsync(endpoint);
        using var stream = new MemoryStream(bytes);
        var avatar = new BitmapImage();
        avatar.BeginInit();
        avatar.CacheOption = BitmapCacheOption.OnLoad;
        avatar.StreamSource = stream;
        avatar.EndInit();
        avatar.Freeze();
        Avatar = avatar;
    }
}