namespace MarkAndSweep;

/// <summary>Board size and mine count for a difficulty level.</summary>
public sealed record Difficulty(string Name, int Columns, int Rows, int Mines)
{
    public static readonly Difficulty Beginner = new("Beginner", 9, 9, 10);
    public static readonly Difficulty Intermediate = new("Intermediate", 16, 16, 40);
    public static readonly Difficulty Expert = new("Expert", 30, 16, 99);

    public static readonly Difficulty[] All = { Beginner, Intermediate, Expert };
}
