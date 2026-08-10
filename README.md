# SquadMasterWeb

Web version of [SquadMaster10](https://github.com/DigitalOxCanada/SquadMaster10) (Windows Store app **Squad Master HS**).

Build and manage a **HeroScape** army in the browser: pick units, track damage on each figure, reorder cards, mark game rounds, and keep score.

## Features (ported from SquadMaster10)

| Feature | Description |
|--------|-------------|
| **Army builder** | Browse 200+ units, filter by species and points, add up to 20 cards |
| **Game view** | Full card art + per-figure hit/heal with damage chits |
| **Map navigation** | Badge strip to jump between units and the game panel |
| **Scoring** | Alive units (any figure with remaining life) still award full points |
| **Rounds & glyphs** | Round markers and glyph reference on the game panel |
| **Settings** | Configure total rounds (1–20) and show/hide the round tracker |

## Run locally

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run
```

Then open the URL shown in the console (typically `https://localhost:7xxx`).

## Project layout

```
Models/           Hero, army, figure, glyph, settings
Services/         HeroDataService (XML catalog), GameStateService (session army)
Components/Pages  Home, Builder, Game
wwwroot/assets    Card images, thumbs, soldier art, glyphs (from SquadMaster10)
wwwroot/data      HeroScapeDB.xml
```

## Data & assets

Unit stats load from `wwwroot/data/HeroScapeDB.xml`. Images live under `wwwroot/assets/` and were taken from the original open-source SquadMaster10 project.

## Disclaimer

Card images and card text are copyright © Hasbro, Inc. **Heroscape** and its associated logos and character names are trademarks of Hasbro, Inc.

This project is a non-commercial fan recreation, matching the spirit of the original SquadMaster10 release (source shared because the app cannot be sold using Hasbro IP).

## Tech

- ASP.NET Core **Blazor Web App** (.NET 10)
- Interactive Server render mode
- Bootstrap 5 + custom tabletop theme
