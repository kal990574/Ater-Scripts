using System;

namespace _02.Scripts.UI.Domain
{
    public interface IUIManager
    {
        UIState CurrentState { get; }
        event Action<UIState> OnUIStateChanged;
    }
}