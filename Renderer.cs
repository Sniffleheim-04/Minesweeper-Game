using System.Numerics;
using Raylib_cs;

namespace MarkAndSweep;

public sealed class Renderer
{
    

    public void Draw(
        Board board,
        Layout layout,
        Difficulty difficulty,
        float elapsed,
        (int X, int Y)? hover,
        bool mouseHeldOnGrid,
        float? best,
        bool newBest)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Theme.Grey);

        DrawToolbar(layout, difficulty);
        DrawHeader(board, layout, elapsed, mouseHeldOnGrid);
        DrawSkillTilesRect(board, layout, elapsed, mouseHeldOnGrid);
        DrawGrid(board, layout, hover);
        DrawFooter(board, layout, difficulty, elapsed, best, newBest);

        Raylib.EndDrawing();
    }

    #region Toolbar

    private static void DrawToolbar(Layout layout, Difficulty current)
    {
        Raylib.DrawRectangleRec(layout.ToolbarRect, Theme.Toolbar);
        Vector2 mouse = Raylib.GetMousePosition();

        for (int i = 0; i < Difficulty.All.Length; i++)
        {
            Rectangle r = layout.DifficultyButtons[i];
            Difficulty d = Difficulty.All[i];

            bool active = d == current;
            bool hot = Raylib.CheckCollisionPointRec(mouse, r);
            Color fill = active ? Theme.ButtonActive : hot ? Theme.ButtonHover : Theme.Button;

            Raylib.DrawRectangleRec(r, fill);
            Raylib.DrawRectangleLinesEx(r, 1, Theme.Border);
            DrawCenteredText(d.Name, r, 14, Theme.White);
        }
    }

    #endregion

    #region Header

    private static void DrawHeader(Board board, Layout layout, float elapsed, bool grabbing)
    {
        // DrawBevel(layout.HeaderRect, raised: false, thickness: 3, fill: Theme.Background);

        DrawCounter(layout.MineCounter, board.MinesRemaining);
        DrawCounter(layout.TimerRect, Math.Min(999, (int)elapsed));

        DrawFace(layout.FaceButton, board.Status, grabbing);
    }

    private static void DrawCounter(Rectangle r, int value)
    {
        value = Math.Clamp(value, -99, 999);
        string text = value < 0 ? $"-{-value:00}" : $"{value:000}";

        Raylib.DrawRectangleRec(r, Theme.DisplayBackground);
        Raylib.DrawRectangleLinesEx(r, 2, Theme.BevelDark);
        DrawCenteredText(text, r, 32, Theme.DisplayText);
    }

    private static void DrawFace(Rectangle r, GameStatus status, bool grabbing)
    {
        Vector2 mouse = Raylib.GetMousePosition();
        bool pressed = Raylib.CheckCollisionPointRec(mouse, r) && Raylib.IsMouseButtonDown(MouseButton.Left);
        DrawBevel(r, raised: !pressed, thickness: 3, fill: Theme.Background);

        float cx = r.X + r.Width / 2;
        float cy = r.Y + r.Height / 2;
        Vector2 center = new(cx, cy);

        Raylib.DrawCircleV(center, 14, Theme.Yellow);
        Raylib.DrawCircleLines((int)cx, (int)cy, 14, Theme.Black);

        switch (status)
        {
            case GameStatus.Lost:
                // X eyes + frown
                Raylib.DrawTexturePro(
                    Theme.faceDeadTexture,
                    new Rectangle(0, 0, Theme.faceDeadTexture.Width, Theme.faceDeadTexture.Height),
                    r,
                    Vector2.Zero,
                    0f,
                    Color.White
                );
                break;

            case GameStatus.Won:
                Raylib.DrawTexturePro(
                    Theme.faceCoolTexture,
                    new Rectangle(0, 0, Theme.faceCoolTexture.Width, Theme.faceCoolTexture.Height),
                    r,
                    Vector2.Zero,
                    0f,
                    Color.White
                );
                break;

            default:
                if (grabbing)
                    Raylib.DrawTexturePro(
                        Theme.faceWowTexture,
                        new Rectangle(0, 0, Theme.faceWowTexture.Width, Theme.faceWowTexture.Height),
                        r,
                        Vector2.Zero,
                        0f,
                        Color.White
                    );
                else
                    Raylib.DrawTexturePro(
                        Theme.faceHappyTexture,
                        new Rectangle(0, 0, Theme.faceHappyTexture.Width, Theme.faceHappyTexture.Height),
                        r,
                        Vector2.Zero,
                        0f,
                        Color.White
                    );
                break;
        }
    }

    #endregion

    #region Skill Tiles

    private static void DrawSkillTilesRect(Board board, Layout layout, float elapsed, bool grabbing)
    {
        DrawBevel(layout.SkillTilesRect, raised: false, thickness: 3, fill: Theme.Background);

        DrawSkillTile(layout.AbilityShield, board.Status, grabbing);
        DrawSkillTile(layout.AbilityDestroyTile, board.Status, grabbing);
        DrawSkillTile(layout.AbilityRevealRandomTile, board.Status, grabbing);
    }

    private static void DrawSkillTile(Rectangle r, GameStatus status, bool grabbing)
    {
        Vector2 mouse = Raylib.GetMousePosition();
        bool pressed = Raylib.CheckCollisionPointRec(mouse, r) && Raylib.IsMouseButtonDown(MouseButton.Left);
        DrawBevel(r, raised: !pressed, thickness: 3, fill: Theme.Background);

        
    }

    #endregion

    #region Main Grid

    private static void DrawGrid(Board board, Layout layout, (int X, int Y)? hover)
    {
        bool lost = board.Status == GameStatus.Lost;

        for (int x = 0; x < board.Columns; x++)
        {
            for (int y = 0; y < board.Rows; y++)
            {
                Cell cell = board.Cells[x, y];
                Rectangle r = layout.CellRect(x, y);

                if (cell.State == CellState.Revealed)
                {
                    DrawRevealedBackground(r, Theme.RevealedCell);
                    if (cell.AdjacentMines > 0) DrawNumber(r, cell.AdjacentMines);
                }
                else if (lost && cell.IsMine && cell.State != CellState.Flagged)
                {
                    // Show all remaining mines after a loss.
                    DrawRevealedBackground(r, cell.Exploded ? Theme.ExplodedCell : Theme.RevealedCell);
                    DrawMine(r);
                }
                else if (lost && cell.State == CellState.Flagged && !cell.IsMine)
                {
                    // Wrong flag: mine drawn with a red cross.
                    DrawRevealedBackground(r, Theme.RevealedCell);
                    DrawMine(r);
                    DrawCross(r);
                }
                else
                {
                    bool hot = hover.HasValue && hover.Value.X == x && hover.Value.Y == y
                               && board.Status is GameStatus.Ready or GameStatus.Playing;

                    DrawCell(r, raised: hot);
                    if (cell.State == CellState.Flagged) DrawFlag(r);
                }
            }
        }
    }

    private static void DrawCell(Rectangle r, bool raised)
    {
        Texture2D texture = raised
            ? Theme.hoverTexture
            : Theme.hiddenTexture;

        Raylib.DrawTexturePro(
            texture,
            new Rectangle(0, 0, texture.Width, texture.Height),
            r,
            Vector2.Zero,
            0f,
            Color.White
        );
    }

    private static void DrawRevealedBackground(Rectangle r, Color fill)
    {
        Raylib.DrawTexturePro(
            Theme.openTexture,
            new Rectangle(0, 0, Theme.openTexture.Width, Theme.openTexture.Height),
            r,
            Vector2.Zero,
            0f,
            Color.White
        );
    }

    private static void DrawNumber(Rectangle r, int n)
    {
        Texture2D texture = n switch
        {
            1 => Theme.num1Texture,
            2 => Theme.num2Texture,
            3 => Theme.num3Texture,
            4 => Theme.num4Texture,
            5 => Theme.num5Texture,
            6 => Theme.num6Texture,
            7 => Theme.num7Texture,
            8 => Theme.num8Texture,
            _ => default
        };

        if (texture.Id == 0) return;

        Raylib.DrawTexturePro(
            texture,
            new Rectangle(0, 0, texture.Width, texture.Height),
            r,
            Vector2.Zero,
            0f,
            Color.White
        );
    }

    private static void DrawFlag(Rectangle r)
    {
        Raylib.DrawTexturePro(
        Theme.flagTexture,
        new Rectangle(0, 0, Theme.flagTexture.Width, Theme.flagTexture.Height),
        r,
        Vector2.Zero,
        0f,
        Color.White
    );
    }

    private static void DrawMine(Rectangle r)
    {
        Raylib.DrawTexturePro(
            Theme.mineTexture,
            new Rectangle(0, 0, Theme.mineTexture.Width, Theme.mineTexture.Height),
            r,
            Vector2.Zero,
            0f,
            Color.White
        );
    }

    private static void DrawCross(Rectangle r)
    {
        Raylib.DrawTexturePro(
            Theme.wrongTexture,
            new Rectangle(0, 0, Theme.wrongTexture.Width, Theme.wrongTexture.Height),
            r,
            Vector2.Zero,
            0f,
            Color.White
        );
    }

    #endregion

    #region Footer

    private static void DrawFooter(Board board, Layout layout, Difficulty difficulty, float elapsed, float? best, bool newBest)
    {
        Rectangle f = layout.FooterRect;

        string status;
        Color color = Theme.Text;

        switch (board.Status)
        {
            case GameStatus.Won:
                status = $"You win! Cleared in {elapsed:0.0}s" + (newBest ? " - new best!" : "");
                color = Theme.Win;
                break;
            case GameStatus.Lost:
                status = "Boom! Press R or click the face to retry.";
                color = Theme.Lose;
                break;
            case GameStatus.Playing:
                status = "Right-click: flag  |  Click a number: chord";
                break;
            default:
                status = "Click any square to begin. First click is safe.";
                break;
        }

        DrawCenteredText(status, new Rectangle(f.X, f.Y, f.Width, 22), 14, color);

        string bestText = best.HasValue
            ? $"Best ({difficulty.Name}): {best.Value:0.0}s"
            : $"Best ({difficulty.Name}): --";
        DrawCenteredText(bestText, new Rectangle(f.X, f.Y + 22, f.Width, 18), 14, Theme.TextDim);
    }

    #endregion

    #region Helpers

    private static void DrawBevel(Rectangle r, bool raised, int thickness, Color fill)
    {
        Raylib.DrawRectangleRec(r, fill);

        Color hi = raised ? Theme.BevelLight : Theme.BevelDark;
        Color lo = raised ? Theme.BevelDark : Theme.BevelLight;
        int x = (int)r.X, y = (int)r.Y, w = (int)r.Width, h = (int)r.Height, t = thickness;

        Raylib.DrawRectangle(x, y, w, t, hi);           // top
        Raylib.DrawRectangle(x, y, t, h, hi);           // left
        Raylib.DrawRectangle(x, y + h - t, w, t, lo);   // bottom
        Raylib.DrawRectangle(x + w - t, y, t, h, lo);   // right
    }

    private static void DrawCenteredText(string text, Rectangle r, int size, Color color)
    {
        int w = Raylib.MeasureText(text, size);
        Raylib.DrawText(text, (int)(r.X + (r.Width - w) / 2), (int)(r.Y + (r.Height - size) / 2), size, color);
    }

    /// <summary>Raylib culls triangles with the wrong winding, so draw both windings.</summary>
    private static void FillTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        Raylib.DrawTriangle(a, b, c, color);
        Raylib.DrawTriangle(a, c, b, color);
    }

    private static void DrawX(Vector2 c, float size)
    {
        Raylib.DrawLineEx(new Vector2(c.X - size, c.Y - size), new Vector2(c.X + size, c.Y + size), 2, Theme.Black);
        Raylib.DrawLineEx(new Vector2(c.X - size, c.Y + size), new Vector2(c.X + size, c.Y - size), 2, Theme.Black);
    }

    /// <summary>Polyline arc; angles in degrees, 0 = right, 90 = down (screen coordinates).</summary>
    private static void DrawArc(Vector2 center, float radius, float startDeg, float endDeg, float thickness, Color color)
    {
        const int segments = 12;
        Vector2 prev = ArcPoint(center, radius, startDeg);

        for (int i = 1; i <= segments; i++)
        {
            float angle = startDeg + (endDeg - startDeg) * i / segments;
            Vector2 next = ArcPoint(center, radius, angle);
            Raylib.DrawLineEx(prev, next, thickness, color);
            prev = next;
        }
    }

    private static Vector2 ArcPoint(Vector2 center, float radius, float degrees)
    {
        float rad = degrees * MathF.PI / 180f;
        return new Vector2(center.X + MathF.Cos(rad) * radius, center.Y + MathF.Sin(rad) * radius);
    }

    #endregion
}
