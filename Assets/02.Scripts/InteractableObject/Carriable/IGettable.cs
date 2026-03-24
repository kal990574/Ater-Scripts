using System;

public interface IGettable
{ 
    
    ItemData ItemData { get; }
    
    
    event Action OnGetEvent;

}