using System.Xml.Linq;
using SquadMasterWeb.Models;

namespace SquadMasterWeb.Services;

/// <summary>
/// Loads the HeroScape unit catalog from the packaged XML database.
/// </summary>
public sealed class HeroDataService
{
    private readonly HttpClient _http;
    private readonly ILogger<HeroDataService> _logger;
    private readonly SemaphoreSlim _loadLock = new(1, 1);

    private IReadOnlyList<HeroUnit>? _heroes;
    private IReadOnlyList<Glyph>? _glyphs;

    public HeroDataService(HttpClient http, ILogger<HeroDataService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public bool IsLoaded => _heroes is not null;

    public IReadOnlyList<HeroUnit> Heroes => _heroes ?? [];
    public IReadOnlyList<Glyph> Glyphs => _glyphs ?? [];

    public async Task EnsureLoadedAsync(CancellationToken cancellationToken = default)
    {
        if (_heroes is not null)
        {
            return;
        }

        await _loadLock.WaitAsync(cancellationToken);
        try
        {
            if (_heroes is not null)
            {
                return;
            }

            await using var stream = await _http.GetStreamAsync("data/HeroScapeDB.xml", cancellationToken);
            var doc = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

            var heroes = new List<HeroUnit>();
            foreach (var node in doc.Descendants("card"))
            {
                var hero = ParseHero(node);
                if (!string.IsNullOrWhiteSpace(hero.EntryId) && !string.IsNullOrWhiteSpace(hero.Name))
                {
                    heroes.Add(hero);
                }
            }

            var glyphs = new List<Glyph>();
            foreach (var node in doc.Descendants("glyph"))
            {
                var glyph = new Glyph
                {
                    EntryId = Attr(node, "id"),
                    Description = Attr(node, "description")
                };
                if (!string.IsNullOrWhiteSpace(glyph.EntryId))
                {
                    glyphs.Add(glyph);
                }
            }

            _heroes = heroes
                .OrderBy(h => h.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
            _glyphs = glyphs;
            _logger.LogInformation("Loaded {HeroCount} heroes and {GlyphCount} glyphs", _heroes.Count, _glyphs.Count);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HeroScape database could not be downloaded");
            _heroes = [];
            _glyphs = [];
        }
        finally
        {
            _loadLock.Release();
        }
    }

    public IEnumerable<HeroUnit> Filter(string? species, int minPoints, int maxPoints)
    {
        IEnumerable<HeroUnit> query = Heroes;

        if (!string.IsNullOrWhiteSpace(species) &&
            !species.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(h => h.Species.Equals(species, StringComparison.OrdinalIgnoreCase));
        }

        query = query.Where(h => h.Points >= minPoints && h.Points <= maxPoints);
        return query;
    }

    public IReadOnlyList<(string Species, int Count)> GetSpeciesOptions()
    {
        return Heroes
            .GroupBy(h => h.Species, StringComparer.OrdinalIgnoreCase)
            .Select(g => (Species: g.First().Species, Count: g.Count()))
            .OrderBy(x => x.Species, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static HeroUnit ParseHero(XElement node)
    {
        return new HeroUnit
        {
            EntryId = Attr(node, "id"),
            Name = Attr(node, "name"),
            Species = Attr(node, "species"),
            Size = Attr(node, "size"),
            World = Attr(node, "world"),
            Set = Attr(node, "set"),
            CollectorNumber = Attr(node, "collectornumber"),
            Valkyrie = Attr(node, "valkyrie"),
            HeroType = Attr(node, "herotype"),
            ClassType = Attr(node, "class"),
            Personality = Attr(node, "personality"),
            Life = IntAttr(node, "life"),
            Move = IntAttr(node, "move"),
            Range = IntAttr(node, "range"),
            Attack = IntAttr(node, "attack"),
            Defense = IntAttr(node, "defense"),
            Points = IntAttr(node, "points"),
            ArmySize = Math.Max(1, IntAttr(node, "armysize", 1)),
            BasicMove = IntAttr(node, "bmove"),
            BasicRange = IntAttr(node, "brange"),
            BasicAttack = IntAttr(node, "battack"),
            BasicDefense = IntAttr(node, "bdefense")
        };
    }

    private static string Attr(XElement node, string name) =>
        (string?)node.Attribute(name) ?? string.Empty;

    private static int IntAttr(XElement node, string name, int defaultValue = 0)
    {
        var raw = Attr(node, name);
        return int.TryParse(raw, out var value) ? value : defaultValue;
    }
}
