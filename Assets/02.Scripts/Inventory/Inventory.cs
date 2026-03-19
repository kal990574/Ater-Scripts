using System;
using System.Collections.Generic;

public class Inventory
{
    public event Action<InventorySlot> OnItemAdded;
    public event Action<InventorySlot> OnItemRemoved;

    private readonly List<InventorySlot> _slots = new();

    public IReadOnlyList<InventorySlot> Slots => _slots;

    public int Count => _slots.Count;


    public bool AddItem(ItemData item)
    {
        if(item == null) return false;

        var slot = new InventorySlot();
        slot.Set(item);
        _slots.Add(slot);

        OnItemAdded?.Invoke(slot);

        return true;
    }

    public bool RemoveItem(InventorySlot slot)
    {
        if (!_slots.Contains(slot)) return false;

        _slots.Remove(slot);

        OnItemRemoved?.Invoke(slot);

        return true;
    }

    public bool HasItem(string itemId)
    {
        return _slots.Exists(slot => slot.Item?.ItemId == itemId);
    }

    public void SwapItem(InventorySlot slotA, InventorySlot slotB)
    {
        ItemData temp = slotA.Item;
        slotA.Set(slotB.Item);
        slotB.Set(temp);
    }
}