namespace MarkAndSweep;

public enum CellState
{
    Hidden,
    Revealed,
    Flagged
}

/// <summary>A single square on the minefield.</summary>
public sealed class Cell
{
    public bool IsMine { get; set; }
    public CellState State { get; set; } = CellState.Hidden;
    public int AdjacentMines { get; set; }

    /// <summary>True for the mine the player stepped on (drawn with a red background).</summary>
    public bool Exploded { get; set; }
}
