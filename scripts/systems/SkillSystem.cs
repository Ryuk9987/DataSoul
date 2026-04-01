using Godot;
using System.Collections.Generic;

public partial class SkillSystem : Node
{
    [Signal] public delegate void SkillUsedEventHandler(string skillId);
    [Signal] public delegate void SlotChangedEventHandler(int slot, string skillId);

    public const int MAX_ACTIVE_SLOTS = 8;
    public const int MAX_PASSIVE_SLOTS = 8;

    private List<string> _activeSlots = new List<string> { "", "", "", "" }; // Start: 4 slots
    private List<string> _passiveSlots = new List<string> { "", "", "", "" };

    public int ActiveSlotCount => _activeSlots.Count;
    public int PassiveSlotCount => _passiveSlots.Count;

    public void EquipSkill(string skillId, int slot, bool isPassive = false)
    {
        var slots = isPassive ? _passiveSlots : _activeSlots;
        if (slot >= 0 && slot < slots.Count)
        {
            slots[slot] = skillId;
            EmitSignal(SignalName.SlotChanged, slot, skillId);
        }
    }

    public void UnequipSkill(int slot, bool isPassive = false)
    {
        var slots = isPassive ? _passiveSlots : _activeSlots;
        if (slot >= 0 && slot < slots.Count)
        {
            slots[slot] = "";
            EmitSignal(SignalName.SlotChanged, slot, "");
        }
    }

    public void UseSkill(int slot)
    {
        if (slot >= 0 && slot < _activeSlots.Count && _activeSlots[slot] != "")
        {
            EmitSignal(SignalName.SkillUsed, _activeSlots[slot]);
        }
    }

    public string GetActiveSkill(int slot)
    {
        if (slot >= 0 && slot < _activeSlots.Count) return _activeSlots[slot];
        return "";
    }

    public void ExpandActiveSlots(int count = 1)
    {
        for (int i = 0; i < count && _activeSlots.Count < MAX_ACTIVE_SLOTS; i++)
            _activeSlots.Add("");
    }

    public void ExpandPassiveSlots(int count = 1)
    {
        for (int i = 0; i < count && _passiveSlots.Count < MAX_PASSIVE_SLOTS; i++)
            _passiveSlots.Add("");
    }
}
