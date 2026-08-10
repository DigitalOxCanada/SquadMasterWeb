namespace SquadMasterWeb.Models;

/// <summary>
/// One figure in a card's squad (tracks damage / life).
/// DamageTaken is the number of damage chits on the figure.
/// </summary>
public sealed class ArmyFigure
{
    public int Index { get; set; }
    public int MaxLife { get; set; }
    public int DamageTaken { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayHeight { get; set; } = 100;

    public bool IsAlive => DamageTaken < MaxLife;
    public int LifeRemaining => Math.Max(0, MaxLife - DamageTaken);

    public void Hit()
    {
        if (DamageTaken < MaxLife)
        {
            DamageTaken++;
        }
    }

    public void Heal()
    {
        if (DamageTaken > 0)
        {
            DamageTaken--;
        }
    }
}
