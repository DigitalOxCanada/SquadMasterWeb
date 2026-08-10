namespace SquadMasterWeb.Models;

public sealed class Glyph
{
    public string EntryId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Image => $"/assets/Glyphs/{EntryId}.png";
}
