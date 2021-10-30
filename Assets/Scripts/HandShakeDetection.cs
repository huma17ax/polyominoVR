using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandShakeDetection : MonoBehaviour
{
    private float interval = 0.0f;
    private float prevRotZ = 0.0f;
    private const float rotThreshPerSec = 1300.0f;

    void Start()
    {

    }

    void Update()
    {
        if (interval == 0.0f) {
            float spRotZ = convertRot(this.transform.localEulerAngles.z) - prevRotZ;
            if (spRotZ > rotThreshPerSec * Time.deltaTime) {
                interval = 0.5f;
                EventManager.Instance.NotifyEvent(
                    EventManager.Event.RightHandShakeClockwise,
                    this,
                    null
                );
            }
            if (spRotZ < -rotThreshPerSec * Time.deltaTime) {
                interval = 0.5f;
                EventManager.Instance.NotifyEvent(
                    EventManager.Event.RightHandShakeAnticlockwise,
                    this,
                    null
                );
            }
        }
        else if (interval > 0.0f) {
            interval -= Time.deltaTime;
            if (interval < 0.0f) interval = 0.0f;
        }
        prevRotZ = convertRot(this.transform.localEulerAngles.z);
    }

    float convertRot(float rot)
    {
        return rot < 180f ? rot : rot-360f;
    }
}
