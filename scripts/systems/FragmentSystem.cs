using Godot;
using System;
using System.Collections.Generic;

public partial class FragmentSystem : Node
{
    [Signal] public delegate void SkillAbsorbedEventHandler(string skillId, int newLevel);

    public Dictionary<string, int> SkillLevels { get; private set; } = new Dictionary<string, int>();

    public override void _Ready()
    {
        // Initialization if needed
    }

    public void AbsorbSkill(string skillId)
    {
        if (SkillLevels.ContainsKey(skillId))
        {
            SkillLevels[skillId] = Mathf.Min(SkillLevels[skillId] + 1, 10);