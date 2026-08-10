namespace SquadMasterWeb.Models;

/// <summary>
/// A HeroScape unit card definition (ported from SquadMaster10 / Platoon10).
/// </summary>
public sealed class HeroUnit
{
    public string EntryId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string World { get; set; } = string.Empty;
    public string Set { get; set; } = string.Empty;
    public string CollectorNumber { get; set; } = string.Empty;
    public string Valkyrie { get; set; } = string.Empty;
    public string HeroType { get; set; } = string.Empty;
    public string ClassType { get; set; } = string.Empty;
    public string Personality { get; set; } = string.Empty;

    public int Life { get; set; }
    public int Move { get; set; }
    public int Range { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Points { get; set; }
    public int ArmySize { get; set; } = 1;

    public int BasicMove { get; set; }
    public int BasicRange { get; set; }
    public int BasicAttack { get; set; }
    public int BasicDefense { get; set; }

    public string Image => $"/assets/Cards/{EntryId}.png";
    public string Thumb => $"/assets/Cards/_t/{EntryId}.png";
    public string Badge => $"/assets/Cards/_b/{EntryId}.png";

    public string StatsLine => $"M:{Move} R:{Range} A:{Attack} D:{Defense}";

    public string SoldierImage(int index) =>
        $"/assets/Cards/_s/{EntryId}-{index}.png";
}
