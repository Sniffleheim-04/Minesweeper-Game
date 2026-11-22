using System.Numerics;
using Raylib_cs;

namespace MarkAndSweep;

/// <summary>
/// Computes where everything goes for a given difficulty and handles mouse hit testing.
/// Deliberately makes no Raylib font calls, so it can be built before the window exists.
/// </summary>
public sealed class Layout
{
    public const int CellSize = 48;
    public const int Padding = 12;
    public const int ToolbarHeight = 36;
    public const int HeaderHeight = 56;
    public const int FooterHeight = 44;
    public const int MinWindowWidth = 360;

    private const int ButtonWidth = 104;
    private const int ButtonHeight = 26;
    private const int ButtonGap = 6;
    private const int FaceSize = 40;
    private const int DisplayWidth = 84;
    private const int DisplayHeight = 40;

    public int WindowWidth { get; }
    public int WindowHeight { get; }

    public Rectangle ToolbarRect { get; }
    public Rectangle HeaderRect { get; }
    public Rectangle FaceButton { get; }
    public Rectangle MineCounter { get; }
    public Rectangle TimerRect { get; }
    public Rectangle GridRect { get; }
    public Rectangle FooterRect { get; }
    public Rectangle[] DifficultyButtons { get; }

    public Layout(Difficulty difficulty)
    {
        int gridW = difficulty.Columns * CellSize;
        int gridH = difficulty.Rows * CellSize;

        WindowWidth = Math.Max(MinWindowWidth, gridW + Padding * 2);

        ToolbarRect = new Rectangle(0, 0, WindowWidth, ToolbarHeight);

        HeaderRect = new Rectangle(Padding, ToolbarHeight + Padding, WindowWidth - Padding * 2, HeaderHeight);

        int gridX = (WindowWidth - gridW) / 2;
        int gridY = (int)(HeaderRect.Y + HeaderRect.Height) + Padding;
        GridRect = new Rectangle(gridX, gridY, gridW, gridH);

        FooterRect = new Rectangle(Padding, gridY + gridH + Padding / 2, WindowWidth - Padding * 2, FooterHeight);
        WindowHeight = (int)(FooterRect.Y + FooterRect.Height) + Padding / 2;

        FaceButton = new Rectangle(
            HeaderRect.X + HeaderRect.Width / 2 - FaceSize / 2,
            HeaderRect.Y + (HeaderHeight - FaceSize) / 2,
            FaceSize, FaceSize);

        MineCounter = new Rectangle(
            HeaderRect.X + 8,
            HeaderRect.Y + (HeaderHeight - DisplayHeight) / 2,
            DisplayWidth, DisplayHeight);

        TimerRect = new Rectangle(
            HeaderRect.X + HeaderRect.Width - 8 - DisplayWidth,
            HeaderRect.Y + (HeaderHeight - DisplayHeight) / 2,
            DisplayWidth, DisplayHeight);

        int count = Difficulty.All.Length;
        int totalW = count * ButtonWidth + (count - 1) * ButtonGap;
        int startX = (WindowWidth - totalW) / 2;
        int btnY = (ToolbarHeight - ButtonHeight) / 2;

        DifficultyButtons = new Rectangle[count];
        for (int i = 0; i < count; i++)
            DifficultyButtons[i] = new Rectangle(startX + i * (ButtonWidth + ButtonGap), btnY, ButtonWidth, ButtonHeight);

        UpdateWindow(this);
    }

    public Rectangle CellRect(int x, int y) =>
        new(GridRect.X + x * CellSize, GridRect.Y + y * CellSize, CellSize, CellSize);

    // Convert mouse pos into grid coordinates
    public bool TryGetCell(Vector2 mouse, out int x, out int y)
    {
        x = y = -1;
        if (!Raylib.CheckCollisionPointRec(mouse, GridRect)) return false;

        x = (int)((mouse.X - GridRect.X) / CellSize);
        y = (int)((mouse.Y - GridRect.Y) / CellSize);

        return true;
    }

    // Centers the window when the layout changes
    private static void UpdateWindow(Layout layout)
    {
        Raylib.SetWindowSize(layout.WindowWidth, layout.WindowHeight);

        int monitor = Raylib.GetCurrentMonitor();
        int screenWidth = Raylib.GetMonitorWidth(monitor);
        int screenHeight = Raylib.GetMonitorHeight(monitor);

        int x = (screenWidth - layout.WindowWidth) / 2;
        int y = (screenHeight - layout.WindowHeight) / 2;

        Raylib.SetWindowPosition(x, y);
    }
}
