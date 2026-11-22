using System.Globalization;

namespace MarkAndSweep;

/// <summary>Stores the best completion time per difficulty in a small text file.</summary>
public sealed class HighScores
{
    private readonly string _path;
    private readonly Dictionary<string, float> _best = new();

    public HighScores()
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MarkAndSweep");
        _path = Path.Combine(folder, "highscores.txt");
        Load();
    }

    public float? GetBest(string difficultyName) =>
        _best.TryGetValue(difficultyName, out float value) ? value : (float?)null;

    /// <summary>Records the time if it beats the previous best. Returns true on a new record.</summary>
    public bool Submit(string difficultyName, float seconds)
    {
        if (_best.TryGetValue(difficultyName, out float current) && current <= seconds)
            return false;

        _best[difficultyName] = seconds;
        Save();
        return true;
    }

    private void Load()
    {
        try
        {
            if (!File.Exists(_path)) return;

            foreach (string line in File.ReadAllLines(_path))
            {
                string[] parts = line.Split('=');
                if (parts.Length != 2) continue;

                if (float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                    _best[parts[0]] = value;
            }
        }
        catch (Exception)
        {
            // Corrupt or unreadable file: just start with no records.
        }
    }

    private void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            var lines = _best.Select(kv => $"{kv.Key}={kv.Value.ToString("0.00", CultureInfo.InvariantCulture)}");
            File.WriteAllLines(_path, lines);
        }
        catch (Exception)
        {
            // Saving is best-effort; never crash the game over it.
        }
    }
}
