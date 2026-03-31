using System;
using System.Collections.Generic;

//구독자를 위한 스크립트.
//해당 구독자가 가진 IDisposable을 한꺼번에 관리한다.
//씬 변경 및 객체 파괴시 한번에 구독을 해지하기 위함
public sealed class CompositeSubscription : IDisposable
{
    private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
    private bool _isDisposed;

    public void Add(IDisposable subscription)
    {
        if (_isDisposed == true)
        {
            subscription?.Dispose();
            return;
        }

        if (subscription == null)
        {
            return;
        }

        _subscriptions.Add(subscription);
    }

    public void Dispose()
    {
        if (_isDisposed == true)
        {
            return;
        }

        _isDisposed = true;

        for (int i = 0; i < _subscriptions.Count; i++)
        {
            _subscriptions[i]?.Dispose();
        }

        _subscriptions.Clear();
    }
}