# Mark-and-Sweep (Minesweeper in C# + raylib-cs)

## Run it

Requires the .NET 8 SDK (or newer; change `TargetFramework` in the .csproj if needed).

```
cd MarkAndSweep
dotnet run
```

If the pinned Raylib-cs version can't be restored, run `dotnet add package Raylib-cs` to pull the latest.

## Controls

| Input                          | Action                                   |
|--------------------------------|------------------------------------------|
| Left click (hidden cell)       | Reveal (first click is always safe)      |
| Right click                    | Flag / unflag                            |
| Left click on a number, or middle click | Chord (open neighbours when flags match) |
| Smiley face, `R` or `F2`       | New game                                 |
| `1` / `2` / `3` or toolbar     | Beginner / Intermediate / Expert         |

## Files

| File             | Responsibility                                          |
|------------------|---------------------------------------------------------|
| `Program.cs`     | Entry point                                             |
| `Game.cs`        | Window, game loop, input, timer                         |
| `Board.cs`       | Game rules: mine placement, flood fill, flags, chording, win/lose |
| `Cell.cs`        | Cell data model                                         |
| `Difficulty.cs`  | Beginner / Intermediate / Expert presets                |
| `Layout.cs`      | Window geometry and mouse hit testing                   |
| `Renderer.cs`    | All drawing (grid, flags, mines, face, counters)        |
| `Theme.cs`       | Colour palette                                          |
| `HighScores.cs`  | Best times saved per difficulty                         |

Best times are stored in `highscores.txt` under your local app data folder.
