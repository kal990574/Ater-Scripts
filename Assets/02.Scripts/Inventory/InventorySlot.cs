using System;

[Serializable]
public class InventorySlot
{
    public object Item {  get; private set; }
    public bool IsEmpty => Item == null;

    public void Set(object item) {  Item = item; }

}
