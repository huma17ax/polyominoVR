using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_reciever : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.Subscribe(EventManager.Event.RightHandShakeClockwise, log);
        EventManager.Instance.Subscribe(EventManager.Event.RightHandShakeAnticlockwise, log2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy(){
        EventManager.Instance.Unsubscribe(EventManager.Event.RightHandShakeClockwise, log);
        EventManager.Instance.Unsubscribe(EventManager.Event.RightHandShakeAnticlockwise, log2);
    }

    void log(object obj, EventArgs args) {
        Debug.Log("aaaa");
    }
    void log2(object obj, EventArgs args) {
        Debug.Log("bbbb");
    }
}
