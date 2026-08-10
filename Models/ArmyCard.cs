namespace SquadMasterWeb.Models;

/// <summary>
/// A unit placed in the current army for a game session.
/// </summary>
public sealed class ArmyCard
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public required HeroUnit Hero { get; init; }
    public List<ArmyFigure> Figures { get; } = [];

    /// <summary>
    /// Full points are awarded while any figure still has remaining life
    /// (matches SquadMaster10 scoring rules).
    /// </summary>
    public bool IsAlive => Figures.Any(f => f.IsAlive);

    public static ArmyCard FromHero(HeroUnit hero)
    {
        var card = new ArmyCard { Hero = hero };
        var height = FigureHeightForSize(hero.Size);

        for (var i = 0; i < Math.Max(1, hero.ArmySize); i++)
        {
            card.Figures.Add(new ArmyFigure
            {
                Index = i + 1,
                MaxLife = Math.Max(1, hero.Life),
                DamageTaken = 0,
                ImageUrl = hero.SoldierImage(i + 1),
                DisplayHeight = height
            });
        }

        return card;
    }

    private static int FigureHeightForSize(string size)
    {
        // Original used the numeric part of "Medium 5", "Huge 13", etc.
        var parts = size.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !int.TryParse(parts[^1], out var sizeValue))
        {
            return 100;
        }

        return sizeValue switch
        {
            >= 10 => 200,
            >= 6 => 150,
            _ => 100
        };
    }
}
