using Godot;
using System.Collections.Generic;

public partial class FragmentSystem : Node
{
    [Signal] public delegate void SkillAbsorbedEventHandler(string skillId, int newLevel);

    public Dictionary<string, int> SkillLevels { get; private set; } = new Dictionary<string, int>();

    public override void _Ready()
    {
    }

    public void AbsorbSkill(string skillId)
    {
        if (SkillLevels.ContainsKey(skillId))
        {
            SkillLevels[skillId] = Mathf.Min(SkillLevels[skillId] + 1, 10);
        }
        else
        {
            SkillLevels[skillId] = 1;
        }

        int newLevel = SkillLevels[skillId];
        EmitSignal(SignalName.SkillAbsorbed, skillId, newLevel);
    }

    public int GetSkillLevel(string skillId)
    {
        return SkillLevels.TryGetValue(skillId, out int level) ? level : 0;
    }

    public bool HasSkill(string skillId)
    {
        return SkillLevels.ContainsKey(skillId) && SkillLevels[skillId] > 0;
    }
}
