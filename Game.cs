using System.Numerics;
using Raylib_cs;

namespace MarkAndSweep;

/// <summary>Owns the window, the game loop, input handling and the timer.</summary>
public sealed class Game
{
    private readonly HighScores _scores = new();
    private readonly Renderer _renderer = new();

    private Difficulty _difficulty = Difficulty.Beginner;
    private Layout _layout;
    private Board _board;

    private float _elapsed;
    private bool _resultHandled;
    private bool _newBest;

    public Game()
    {
        _layout = new Layout(_difficulty);
        _board = new Board(_difficulty);
    }

    public void Run()
    {
        Raylib.InitWindow(_layout.WindowWidth, _layout.WindowHeight, "Mark-and-Sweep");
        Theme.InitTextures();
        Raylib.SetWindowIcon(Theme.icon);
        Raylib.UnloadImage(Theme.icon);
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            Update();
            Draw();
        }

        Raylib.CloseWindow();
    }

    

    // ----------------------------------------------------------------- update

    private void Update()
    {
        HandleKeyboard();
        HandleMouse();
        UpdateTimer();
        HandleResult();
    }

    private void HandleKeyboard()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.R) || Raylib.IsKeyPressed(KeyboardKey.F2))
            NewGame();
        else if (Raylib.IsKeyPressed(KeyboardKey.One))
            SetDifficulty(Difficulty.Beginner);
        else if (Raylib.IsKeyPressed(KeyboardKey.Two))
            SetDifficulty(Difficulty.Intermediate);
        else if (Raylib.IsKeyPressed(KeyboardKey.Three))
            SetDifficulty(Difficulty.Expert);
    }

    private void HandleMouse()
    {
        Vector2 mouse = Raylib.GetMousePosition();

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            // Smiley button
            if (Raylib.CheckCollisionPointRec(mouse, _layout.FaceButton))
            {
                NewGame();
                return;
            }

            // Difficulty buttons
            for (int i = 0; i < _layout.DifficultyButtons.Length; i++)
            {
                if (Raylib.CheckCollisionPointRec(mouse, _layout.DifficultyButtons[i]))
                {
                    SetDifficulty(Difficulty.All[i]);
                    return;
                }
            }

            // Instant Reset when clicking after winning/losing
            if (_board.Status == GameStatus.Lost || _board.Status == GameStatus.Won)
            {
                NewGame();
                return;
            }
        }

        if (!_layout.TryGetCell(mouse, out int x, out int y)) return;

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            // Clicking an already-revealed number chords; clicking a hidden cell reveals it.
            if (_board.Cells[x, y].State == CellState.Revealed)
                _board.Chord(x, y);
            else
                _board.Reveal(x, y);
        }
        else if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            _board.ToggleFlag(x, y);
        }
        else if (Raylib.IsMouseButtonPressed(MouseButton.Middle))
        {
            _board.Chord(x, y);
        }
    }

    private void UpdateTimer()
    {
        if (_board.Status == GameStatus.Playing)
            _elapsed += Raylib.GetFrameTime();
    }

    /// <summary>Runs once when a game ends: records the time on a win.</summary>
    private void HandleResult()
    {
        if (_resultHandled) return;

        if (_board.Status == GameStatus.Won)
        {
            _newBest = _scores.Submit(_difficulty.Name, _elapsed);
            _resultHandled = true;
        }
        else if (_board.Status == GameStatus.Lost)
        {
            _resultHandled = true;
        }
    }

    // ------------------------------------------------------------------- draw

    private void Draw()
    {
        Vector2 mouse = Raylib.GetMousePosition();

        (int X, int Y)? hover = null;
        bool overGrid = _layout.TryGetCell(mouse, out int hx, out int hy);
        if (overGrid) hover = (hx, hy);

        bool playable = _board.Status is GameStatus.Ready or GameStatus.Playing;
        bool mouseHeldOnGrid = overGrid && playable && Raylib.IsMouseButtonDown(MouseButton.Left);

        _renderer.Draw(
            _board,
            _layout,
            _difficulty,
            _elapsed,
            hover,
            mouseHeldOnGrid,
            _scores.GetBest(_difficulty.Name),
            _newBest);
    }

    // ------------------------------------------------------------ game control

    private void NewGame()
    {
        _board = new Board(_difficulty);
        _elapsed = 0f;
        _resultHandled = false;
        _newBest = false;
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        _difficulty = difficulty;
        _layout = new Layout(difficulty);
        Raylib.SetWindowSize(_layout.WindowWidth, _layout.WindowHeight);
        NewGame();
    }
}
