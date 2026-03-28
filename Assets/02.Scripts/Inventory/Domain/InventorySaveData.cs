using System;
using System.Collections.Generic;

[Serializable]
public class InventorySaveData
{
    public List<ItemInstanceSaveData> ItemInstances = new();
    public List<string> InventoryInstanceIds = new();
    public List<SceneItemSaveData> SceneItems = new();
    public string EquippedHandInstanceId;
    public int SelectedIndex = -1;
}

[Serializable]
public class ItemInstanceSaveData
{
    public string InstanceId;
    public int ItemId;
    public List<ItemStateValueSaveData> StateEntries = new();
}

[Serializable]
public class ItemStateValueSaveData
{
    public string Key;
    public bool BoolValue;
    public int IntValue;
    public string StringValue;
}

[Serializable]
public class SceneItemSaveData
{
    public string SceneObjectId;
    public string InstanceId;
    public bool IsCollected;
}
