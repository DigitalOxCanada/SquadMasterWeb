namespace SquadMasterWeb.Models;

public sealed class GameSettings
{
    public bool ShowRounds { get; set; } = true;
    public int TotalRounds { get; set; } = 10;
    public int CurrentRound { get; set; } = 1;
    public int MaxArmyUnits { get; set; } = 20;
}
