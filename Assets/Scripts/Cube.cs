using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ブロックを構成する1マス
public class Cube : MonoBehaviour
{
    public bool inFrame {get; private set;} = false;
    public bool isTouched {get; private set;} = false;

    private List<Collider> triggeredFrames = new List<Collider>();
    public Block affiliatedBlock {get; private set;}

    void Start()
    {
        
    }

    void Update()
    {
        // Boardに近いか判定
        inFrame = false;
        foreach (Collider frame in triggeredFrames) {
            float distance = Vector3.Distance(frame.gameObject.transform.position, this.transform.position);
            if (distance < Mathf.Sqrt(3*Mathf.Pow(0.5f, 2))) {
                inFrame = true;
                break;
            }
        }
    }

    public void SetAffiliatedBlock(Block block) {
        affiliatedBlock = block;
    }

    void OnTriggerEnter(Collider other) {
        // Boardに接触(侵入)
        if (other.gameObject.tag == "frame") {
            triggeredFrames.Add(other);
        }
        // 手に接触(侵入)
        if (other.gameObject.tag == "hand") {
            isTouched = true;
            EventManager.Instance.NotifyEvent(EventManager.Event.HandTouchEnterBlock, this, null);
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "frame") {
            triggeredFrames.Remove(other);
        }
        if (other.gameObject.tag == "hand") {
            isTouched = false;
            EventManager.Instance.NotifyEvent(EventManager.Event.HandTouchLeaveBlock, this, null);
        }
    }
}
