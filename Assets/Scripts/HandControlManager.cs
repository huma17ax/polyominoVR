using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// 手の握る操作
public class HandControlManager : MonoBehaviour
{
    private List<Cube> touchingCubes = new List<Cube>();
    private Block? graspingBlock;

    void Start()
    {
        // イベントの購読
        EventManager.Instance.Subscribe(EventManager.Event.HandTouchEnterBlock, AddTouchingCube);
        EventManager.Instance.Subscribe(EventManager.Event.HandTouchLeaveBlock, RemoveTouchingCube);
    }

    void Update()
    {
        // コントローラーを握ったとき
        if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)) {
            float minDist = 100000f;
            Cube? nearestCube = null;
            foreach (Cube cube in touchingCubes) {
                float dist = Vector3.Distance(cube.transform.position, this.transform.position);
                if (minDist > dist) {
                    minDist = dist;
                    nearestCube = cube;
                }
            }
            if (nearestCube) {
                // graspingBlock.transform.SetParent(this.transform);
                graspingBlock = nearestCube.affiliatedBlock;
                graspingBlock.Grasp(this);
            }
        }

        // コントローラーを離したとき
        if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger) && graspingBlock) {
            graspingBlock.Grasp(null);
            graspingBlock = null;
        }

    }

    private void AddTouchingCube(object obj, EventArgs args) {
        touchingCubes.Add((Cube)obj);
    }
    private void RemoveTouchingCube(object obj, EventArgs args) {
        touchingCubes.Remove((Cube)obj);
    }
}
