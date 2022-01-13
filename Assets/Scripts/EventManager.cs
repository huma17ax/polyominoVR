using System;
using System.Collections.Generic;

// オブザーバーパターンによるイベント管理
public class EventManager
{
    
    // Observer, Singleton
    static EventManager instance = new EventManager();
    private EventManager(){}
    public static EventManager Instance {
        get {
            return instance;
        }
    }

    public enum Event {
        RightHandShakeClockwise,
        RightHandShakeAnticlockwise,
        HandTouchEnterBlock,
        HandTouchLeaveBlock
    }

    private Dictionary<Event, EventHandler> eventHandler = new Dictionary<Event, EventHandler>();

    // 購読処理
    public void Subscribe(Event _event, EventHandler _handler) {
        if (!eventHandler.ContainsKey(_event)) eventHandler[_event] = delegate(object obj, EventArgs arg){};
        eventHandler[_event] += _handler;
    }

    // 購読の取り消し
    public void Unsubscribe(Event _event, EventHandler _handler) {
        if (!eventHandler.ContainsKey(_event)) return;
        eventHandler[_event] -= _handler;
    }

    // イベント通知
    public void NotifyEvent(Event _event, object _obj, EventArgs _args) {
        if (!eventHandler.ContainsKey(_event)) return;
        eventHandler[_event](_obj, _args);
    }

}
