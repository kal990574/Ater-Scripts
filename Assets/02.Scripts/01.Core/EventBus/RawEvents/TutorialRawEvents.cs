// 플레이어 이동 (WASD 입력)
public readonly struct PlayerMovedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public PlayerMovedRawEvent(GameEventContext context)
    {
        Context = context;
    }
}

// 오브젝트 E키 상호작용
public readonly struct InteractedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public InteractedRawEvent(GameEventContext context)
    {
        Context = context;
    }
}

// 아이템 인벤토리에 추가됨
public readonly struct ItemAddedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public ItemAddedRawEvent(GameEventContext context)
    {
        Context = context;
    }
}

// 1~5로 아이템 장착
public readonly struct ItemEquippedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public ItemEquippedRawEvent(GameEventContext context)
    {
        Context = context;
    }
}

// TAB 인벤토리 토글
public readonly struct InventoryToggledRawEvent : IGameEvent
{
    public GameEventContext Context { get; }
    public bool IsOpen { get; }

    public InventoryToggledRawEvent(GameEventContext context, bool isOpen)
    {
        Context = context;
        IsOpen = isOpen;
    }
}

// 아이템 드래그
public readonly struct ItemDraggedRawEvent : IGameEvent
{
    public GameEventContext Context { get; }             
                  
    public ItemDraggedRawEvent(GameEventContext context)
    {
        Context = context;
    }
}

// 아이템 스크롤
public readonly struct ItemScrolledRawEvent : IGameEvent
{
    public GameEventContext Context { get; }

    public ItemScrolledRawEvent(GameEventContext context)
    {
        Context = context;                               
    }           
}
