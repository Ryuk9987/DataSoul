using Godot;

public partial class LevelSystem : Node
{
    [Signal] public delegate void LevelUpEventHandler(int newLevel);
    [Signal] public delegate void XpGainedEventHandler(float xp, float total, float needed);

    public int Level { get; private set; } = 1;
    public float CurrentXp { get; private set; } = 0f;
    public const int MaxLevel = 100;

    public float XpForNextLevel => CalculateXpNeeded(Level);

    private static float CalculateXpNeeded(int level)
    {
        if (level <= 50)
            return 100f * Mathf.Pow(level, 1.5f);
        else
            return 100f * Mathf.Pow(50f, 1.5f) + 500f * Mathf.Pow(level - 50f, 2.2f);
    }

    public void AddXp(float amount)
    {
        if (Level >= MaxLevel) return;

        CurrentXp += amount;
        EmitSignal(SignalName.XpGained, CurrentXp, CurrentXp, XpForNextLevel);

        while (CurrentXp >= XpForNextLevel && Level < MaxLevel)
        {
            CurrentXp -= XpForNextLevel;
            Level++;
            EmitSignal(SignalName.LevelUp, Level);
            GD.Print($"[LevelSystem] Level Up! Now Lv{Level}");
        }
    }

    public static float GetXpForEnemy(string enemyType, int enemyLevel)
    {
        return enemyType switch
        {
            "normal" => enemyLevel * 10f,
            "elite" => enemyLevel * 25f,
            "miniboss" => enemyLevel * 100f,
            "boss" => enemyLevel * 500f,
            _ => enemyLevel * 10f
        };
    }
}
