using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 手首の回転検出
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
            // 時計回りの回転
            if (spRotZ > rotThreshPerSec * Time.deltaTime) {
                interval = 0.5f;
                EventManager.Instance.NotifyEvent(
                    EventManager.Event.RightHandShakeClockwise,
                    this,
                    null
                );
            }
            // 反時計回りの回転
            if (spRotZ < -rotThreshPerSec * Time.deltaTime) {
                interval = 0.5f;
                EventManager.Instance.NotifyEvent(
                    EventManager.Event.RightHandShakeAnticlockwise,
                    this,
                    null
                );
            }
        }

        // 回転操作のインターバル
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
