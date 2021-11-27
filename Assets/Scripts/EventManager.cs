using System;
using System.Collections.Generic;
public class EventManager
{
    
    //Singleton
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

    public void Subscribe(Event _event, EventHandler _handler) {
        if (!eventHandler.ContainsKey(_event)) eventHandler[_event] = delegate(object obj, EventArgs arg){};
        eventHandler[_event] += _handler;
    }
    public void Unsubscribe(Event _event, EventHandler _handler) {
        if (!eventHandler.ContainsKey(_event)) return;
        eventHandler[_event] -= _handler;
    }

    public void NotifyEvent(Event _event, object _obj, EventArgs _args) {
        if (!eventHandler.ContainsKey(_event)) return;
        eventHandler[_event](_obj, _args);
    }

}
