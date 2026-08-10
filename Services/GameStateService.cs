using SquadMasterWeb.Models;

namespace SquadMasterWeb.Services;

/// <summary>
/// Per-circuit game state: selected army, active card, rounds, and scoring.
/// Scoped so each browser session has its own army.
/// </summary>
public sealed class GameStateService
{
    public GameSettings Settings { get; } = new();

    /// <summary>Units picked on the builder page (before "Build Squad").</summary>
    public List<HeroUnit> DraftArmy { get; } = [];

    /// <summary>Active army cards during a game session.</summary>
    public List<ArmyCard> Army { get; private set; } = [];

    public int SelectedIndex { get; private set; }
    public bool HasArmy => Army.Count > 0;

    public event Action? Changed;

    public int TotalPoints => Army.Sum(c => c.Hero.Points);

    /// <summary>
    /// Points still in play — full card points while any figure is alive.
    /// </summary>
    public int CurrentPoints => Army.Where(c => c.IsAlive).Sum(c => c.Hero.Points);

    public ArmyCard? SelectedCard =>
        SelectedIndex >= 0 && SelectedIndex < Army.Count ? Army[SelectedIndex] : null;

    public bool ShowingGamePanel => SelectedIndex == Army.Count && Army.Count > 0;

    public void AddToDraft(HeroUnit hero)
    {
        if (DraftArmy.Count >= Settings.MaxArmyUnits)
        {
            return;
        }

        DraftArmy.Add(hero);
        Notify();
    }

    public void RemoveFromDraft(int index)
    {
        if (index < 0 || index >= DraftArmy.Count)
        {
            return;
        }

        DraftArmy.RemoveAt(index);
        Notify();
    }

    public void ClearDraft()
    {
        DraftArmy.Clear();
        Notify();
    }

    public int DraftPoints => DraftArmy.Sum(h => h.Points);

    /// <summary>
    /// Convert the draft into playable army cards and start a game.
    /// </summary>
    public bool BuildArmyFromDraft()
    {
        if (DraftArmy.Count == 0)
        {
            return false;
        }

        Army = DraftArmy.Select(ArmyCard.FromHero).ToList();
        DraftArmy.Clear();
        SelectedIndex = 0;
        Settings.CurrentRound = 1;
        Notify();
        return true;
    }

    public void SelectIndex(int index)
    {
        // Valid range: 0..Army.Count (Army.Count = game panel)
        if (index < 0 || index > Army.Count)
        {
            return;
        }

        SelectedIndex = index;
        Notify();
    }

    public void MoveSelectedLeft()
    {
        if (SelectedIndex <= 0 || SelectedIndex >= Army.Count)
        {
            return;
        }

        var card = Army[SelectedIndex];
        Army.RemoveAt(SelectedIndex);
        SelectedIndex--;
        Army.Insert(SelectedIndex, card);
        Notify();
    }

    public void MoveSelectedRight()
    {
        if (SelectedIndex < 0 || SelectedIndex >= Army.Count - 1)
        {
            return;
        }

        var card = Army[SelectedIndex];
        Army.RemoveAt(SelectedIndex);
        SelectedIndex++;
        Army.Insert(SelectedIndex, card);
        Notify();
    }

    public void DeleteSelected()
    {
        if (SelectedIndex < 0 || SelectedIndex >= Army.Count || Army.Count <= 1)
        {
            return;
        }

        Army.RemoveAt(SelectedIndex);
        if (SelectedIndex >= Army.Count)
        {
            SelectedIndex = Math.Max(0, Army.Count - 1);
        }

        Notify();
    }

    public void HitFigure(Guid cardId, int figureIndex)
    {
        var figure = FindFigure(cardId, figureIndex);
        figure?.Hit();
        Notify();
    }

    public void HealFigure(Guid cardId, int figureIndex)
    {
        var figure = FindFigure(cardId, figureIndex);
        figure?.Heal();
        Notify();
    }

    public void SetRound(int round)
    {
        if (round < 1 || round > Settings.TotalRounds)
        {
            return;
        }

        Settings.CurrentRound = round;
        Notify();
    }

    public void SetTotalRounds(int rounds)
    {
        Settings.TotalRounds = Math.Clamp(rounds, 1, 20);
        if (Settings.CurrentRound > Settings.TotalRounds)
        {
            Settings.CurrentRound = Settings.TotalRounds;
        }

        Notify();
    }

    public void StartNewGame()
    {
        Army = [];
        DraftArmy.Clear();
        SelectedIndex = 0;
        Settings.CurrentRound = 1;
        Notify();
    }

    public void SetShowRounds(bool show)
    {
        Settings.ShowRounds = show;
        Notify();
    }

    /// <summary>Force UI listeners to refresh after an external mutation.</summary>
    public void Refresh() => Notify();

    private ArmyFigure? FindFigure(Guid cardId, int figureIndex)
    {
        var card = Army.FirstOrDefault(c => c.InstanceId == cardId);
        return card?.Figures.FirstOrDefault(f => f.Index == figureIndex);
    }

    private void Notify() => Changed?.Invoke();
}
