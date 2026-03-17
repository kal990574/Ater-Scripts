using System;

[Serializable]
public class InventorySlot
{
    public ItemData Item {  get; private set; }
    public bool IsEmpty => Item == null;

    public void Set(ItemData item) {  Item = item; }

}
