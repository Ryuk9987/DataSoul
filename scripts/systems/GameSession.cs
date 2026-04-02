/// <summary>
/// Statische Klasse zum Übertragen von Charakterdaten zwischen Szenen.
/// Wird von CharacterCreationController befüllt und von PlayerStats in _Ready() gelesen.
/// </summary>
public static class GameSession
{
    public static string PlayerName { get; set; } = "Held";
    public static PlayerStats.Background PlayerBackground { get; set; } = PlayerStats.Background.Gamer;
    public static bool IsInitialized { get; set; } = false;
}
