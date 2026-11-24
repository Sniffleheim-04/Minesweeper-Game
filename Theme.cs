using Raylib_cs;

namespace MarkAndSweep;

// Pixel Art Assets: Minesweeper Pixel Kit by Infi76
// Source: itch.io – Minesweeper Pixel Kit
// Creator: Infi76
// Link: https://infi76.itch.io/minesweeper-pixel-kit

public static class Theme
{
    public static readonly Color Background = new(192, 192, 192, 255);
    public static readonly Color Toolbar = new(60, 63, 70, 255);
    public static readonly Color Button = new(88, 92, 102, 255);
    public static readonly Color ButtonHover = new(110, 115, 128, 255);
    public static readonly Color ButtonActive = new(52, 120, 200, 255);
    public static readonly Color Border = new(30, 32, 36, 255);

    public static readonly Color BevelLight = new(255, 255, 255, 255);
    public static readonly Color BevelDark = new(123, 123, 123, 255);

    public static readonly Color HiddenCell = new(192, 192, 192, 255);
    public static readonly Color HoverCell = new(210, 210, 210, 255);
    public static readonly Color RevealedCell = new(222, 222, 222, 255);
    public static readonly Color CellGrid = new(150, 150, 150, 255);
    public static readonly Color ExplodedCell = new(230, 60, 60, 255);

    public static readonly Color Black = new(0, 0, 0, 255);
    public static readonly Color Grey = new(160, 165, 180, 255);
    public static readonly Color White = new(255, 255, 255, 255);
    public static readonly Color Red = new(220, 30, 30, 255);
    public static readonly Color Yellow = new(255, 215, 0, 255);

    public static readonly Color DisplayBackground = new(20, 0, 0, 255);
    public static readonly Color DisplayText = new(255, 40, 40, 255);

    public static readonly Color Text = new(30, 30, 30, 255);
    public static readonly Color TextDim = new(90, 90, 90, 255);
    public static readonly Color Win = new(20, 130, 40, 255);
    public static readonly Color Lose = new(190, 30, 30, 255);

    /// <summary>Classic number colours; index = number of adjacent mines (0 unused).</summary>
    public static readonly Color[] NumberColors =
    {
        new(0, 0, 0, 255),
        new(0, 0, 255, 255),
        new(0, 128, 0, 255),
        new(255, 0, 0, 255),
        new(0, 0, 128, 255),
        new(128, 0, 0, 255),
        new(0, 128, 128, 255),
        new(0, 0, 0, 255),
        new(128, 128, 128, 255)
    };

    public static Texture2D explodedTexture;
    public static Texture2D faceCoolTexture;
    public static Texture2D faceDeadTexture;
    public static Texture2D faceHappyTexture;
    public static Texture2D faceWowTexture;
    public static Texture2D flagTexture;
    public static Texture2D hiddenTexture;
    public static Texture2D hoverTexture;
    public static Texture2D mineTexture;
    public static Texture2D num1Texture;
    public static Texture2D num2Texture;
    public static Texture2D num3Texture;
    public static Texture2D num4Texture;
    public static Texture2D num5Texture;
    public static Texture2D num6Texture;
    public static Texture2D num7Texture;
    public static Texture2D num8Texture;
    public static Texture2D openTexture;
    public static Texture2D questionTexture;
    public static Texture2D wrongTexture;

    public static void InitTextures()
    {
        explodedTexture = Raylib.LoadTexture("assets/images/exploded.png");
        faceCoolTexture = Raylib.LoadTexture("assets/images/face_cool.png");
        faceDeadTexture = Raylib.LoadTexture("assets/images/face_dead.png");
        faceHappyTexture = Raylib.LoadTexture("assets/images/face_happy.png");
        faceWowTexture = Raylib.LoadTexture("assets/images/face_wow.png");
        flagTexture = Raylib.LoadTexture("assets/images/flag.png");
        hiddenTexture = Raylib.LoadTexture("assets/images/hidden.png");
        hoverTexture = Raylib.LoadTexture("assets/images/hover.png");
        mineTexture = Raylib.LoadTexture("assets/images/mine.png");
        num1Texture = Raylib.LoadTexture("assets/images/num_1.png");
        num2Texture = Raylib.LoadTexture("assets/images/num_2.png");
        num3Texture = Raylib.LoadTexture("assets/images/num_3.png");
        num4Texture = Raylib.LoadTexture("assets/images/num_4.png");
        num5Texture = Raylib.LoadTexture("assets/images/num_5.png");
        num6Texture = Raylib.LoadTexture("assets/images/num_6.png");
        num7Texture = Raylib.LoadTexture("assets/images/num_7.png");
        num8Texture = Raylib.LoadTexture("assets/images/num_8.png");
        openTexture = Raylib.LoadTexture("assets/images/open.png");
        questionTexture = Raylib.LoadTexture("assets/images/question.png");
        wrongTexture = Raylib.LoadTexture("assets/images/wrong.png");
    }
}
