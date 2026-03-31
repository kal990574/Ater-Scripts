using System;
using System.Collections.Generic;

public class GameEventBus : IGameEventBus
    {
        //이벤트 타입별 구독자를 저장. 해당 이벤트를 어떤 구독자들이 구독하고 있나?
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

        //타입을 찾아 구독자
        public IDisposable Subscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Type eventType = typeof(T);

            if (_subscribers.TryGetValue(eventType, out List<Delegate> handlers) == false)
            {
                handlers = new List<Delegate>();
                _subscribers.Add(eventType, handlers);
            }

            handlers.Add(handler);

            return new Subscription(() =>
            {
                if (_subscribers.TryGetValue(eventType, out List<Delegate> currentHandlers) == false)
                {
                    return;
                }

                currentHandlers.Remove(handler);

                if (currentHandlers.Count == 0)
                {
                    _subscribers.Remove(eventType);
                }
            });
        }

        public void Publish<T>(in T gameEvent) where T : struct, IGameEvent
        {
            Type eventType = typeof(T);

            if (_subscribers.TryGetValue(eventType, out List<Delegate> handlers) == false)
            {
                return;
            }

            Delegate[] invocationList = handlers.ToArray();

            for (int i = 0; i < invocationList.Length; i++)
            {
                Action<T> callback = (Action<T>)invocationList[i];
                callback.Invoke(gameEvent);
            }
        }

        public void Clear()
        {
            _subscribers.Clear();
        }

        private class Subscription : IDisposable
        {
            private Action _disposeAction;
            private bool _isDisposed;

            public Subscription(Action disposeAction)
            {
                _disposeAction = disposeAction;
            }

            public void Dispose()
            {
                if (_isDisposed == true)
                {
                    return;
                }

                _isDisposed = true;
                _disposeAction?.Invoke();
                _disposeAction = null;
            }
        }
    }