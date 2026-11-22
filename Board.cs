namespace MarkAndSweep;

public enum GameStatus
{
    Ready,    // board created, no click yet (mines not placed)
    Playing,
    Won,
    Lost
}

/// <summary>Pure game logic: no rendering or input in here.</summary>
public sealed class Board
{
    private readonly Random _rng;
    private int _revealedSafeCells;

    public int Columns { get; }
    public int Rows { get; }
    public int MineCount { get; }
    public Cell[,] Cells { get; }
    public GameStatus Status { get; private set; } = GameStatus.Ready;
    public int FlagsPlaced { get; private set; }
    public int MinesRemaining => MineCount - FlagsPlaced;

    public Board(Difficulty difficulty, Random? rng = null)
    {
        Columns = difficulty.Columns;
        Rows = difficulty.Rows;
        MineCount = Math.Min(difficulty.Mines, Columns * Rows - 1);
        _rng = rng ?? new Random();

        Cells = new Cell[Columns, Rows];
        for (int x = 0; x < Columns; x++)
            for (int y = 0; y < Rows; y++)
                Cells[x, y] = new Cell();
    }

    public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < Columns && y < Rows;

    // ---------------------------------------------------------------- actions

    /// <summary>Reveal a hidden cell. The very first reveal is always safe.</summary>
    public void Reveal(int x, int y)
    {
        if (!InBounds(x, y) || Status is GameStatus.Won or GameStatus.Lost) return;
        if (Cells[x, y].State != CellState.Hidden) return;

        if (Status == GameStatus.Ready)
        {
            PlaceMines(x, y);
            Status = GameStatus.Playing;
        }

        RevealCell(x, y);
        CheckWin();
    }

    /// <summary>Toggle a flag on a hidden cell.</summary>
    public void ToggleFlag(int x, int y)
    {
        if (!InBounds(x, y) || Status is GameStatus.Won or GameStatus.Lost) return;

        Cell cell = Cells[x, y];
        switch (cell.State)
        {
            case CellState.Hidden:
                cell.State = CellState.Flagged;
                FlagsPlaced++;
                break;
            case CellState.Flagged:
                cell.State = CellState.Hidden;
                FlagsPlaced--;
                break;
        }
    }

    /// <summary>
    /// "Chording": on a revealed number whose adjacent flag count matches the number,
    /// reveal every remaining hidden neighbour at once.
    /// </summary>
    public void Chord(int x, int y)
    {
        if (!InBounds(x, y) || Status != GameStatus.Playing) return;

        Cell cell = Cells[x, y];
        if (cell.State != CellState.Revealed || cell.AdjacentMines == 0) return;

        int flags = 0;
        foreach (var (nx, ny) in Neighbors(x, y))
            if (Cells[nx, ny].State == CellState.Flagged) flags++;

        if (flags != cell.AdjacentMines) return;

        foreach (var (nx, ny) in Neighbors(x, y))
            if (Cells[nx, ny].State == CellState.Hidden)
                RevealCell(nx, ny);

        CheckWin();
    }

    // --------------------------------------------------------------- internals

    private void RevealCell(int x, int y)
    {
        if (Status != GameStatus.Playing) return;

        Cell cell = Cells[x, y];
        if (cell.State != CellState.Hidden) return;

        if (cell.IsMine)
        {
            cell.Exploded = true;
            Status = GameStatus.Lost;
            return;
        }

        FloodReveal(x, y);
    }

    /// <summary>Iterative flood fill (no recursion, so Expert boards can't overflow the stack).</summary>
    private void FloodReveal(int startX, int startY)
    {
        var stack = new Stack<(int X, int Y)>();
        stack.Push((startX, startY));

        while (stack.Count > 0)
        {
            var (x, y) = stack.Pop();
            Cell cell = Cells[x, y];
            if (cell.State != CellState.Hidden || cell.IsMine) continue;

            cell.State = CellState.Revealed;
            _revealedSafeCells++;

            if (cell.AdjacentMines != 0) continue;

            foreach (var (nx, ny) in Neighbors(x, y))
                if (Cells[nx, ny].State == CellState.Hidden)
                    stack.Push((nx, ny));
        }
    }

    private void CheckWin()
    {
        if (Status != GameStatus.Playing) return;
        if (_revealedSafeCells != Columns * Rows - MineCount) return;

        Status = GameStatus.Won;

        // Auto-flag all mines as the victory state.
        foreach (Cell cell in Cells)
        {
            if (cell.IsMine && cell.State != CellState.Flagged)
                cell.State = CellState.Flagged;
        }
        FlagsPlaced = MineCount;
    }

    private void PlaceMines(int safeX, int safeY)
    {
        // Keep the first click AND its neighbours mine-free (so it opens up),
        // unless the board is too crowded for that.
        bool protectNeighbourhood = Columns * Rows - 9 >= MineCount;

        var candidates = new List<int>(Columns * Rows);
        for (int y = 0; y < Rows; y++)
        {
            for (int x = 0; x < Columns; x++)
            {
                if (x == safeX && y == safeY) continue;
                if (protectNeighbourhood && Math.Abs(x - safeX) <= 1 && Math.Abs(y - safeY) <= 1) continue;
                candidates.Add(y * Columns + x);
            }
        }

        // Partial Fisher-Yates shuffle: pick MineCount distinct candidates.
        for (int i = 0; i < MineCount; i++)
        {
            int j = _rng.Next(i, candidates.Count);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);

            int index = candidates[i];
            Cells[index % Columns, index / Columns].IsMine = true;
        }

        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                int count = 0;
                foreach (var (nx, ny) in Neighbors(x, y))
                    if (Cells[nx, ny].IsMine) count++;
                Cells[x, y].AdjacentMines = count;
            }
        }
    }

    private IEnumerable<(int X, int Y)> Neighbors(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx, ny = y + dy;
                if (InBounds(nx, ny)) yield return (nx, ny);
            }
        }
    }
}
