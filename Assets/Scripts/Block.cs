using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ブロック(パズルの1ピース)
public class Block : MonoBehaviour
{
    private List<List<bool>> shape;
    private GameObject cubeWrapper;
    private GameObject boardObj;
    public GameObject cubePrefab;
    public GameObject cubeWrapperPrefab;
    private Color color;

    private const float rotateTime = 0.1f;
    private float leftTime;
    private int rotateWay = 0;
    private bool isTouched = false;
    private HandControlManager? graspedHand = null;
    private Vector3? prevHandPos = null;

    private List<Cube> cubes = new List<Cube>();

    //ブロック形状の設定(Cubeの生成)
    public void SetShape(List<List<bool>> _shape) {
        if (cubeWrapper) Destroy(cubeWrapper);
        cubeWrapper = Instantiate(cubeWrapperPrefab, this.transform.position, new Quaternion(), this.transform);
        shape = _shape;
        for (int y=0; y < shape.Count; ++y) {
            for (int x=0; x < shape[y].Count; ++x) {
                if (shape[y][x]) {
                    GameObject obj = Instantiate(
                        cubePrefab,
                        new Vector3(
                            this.cubeWrapper.transform.position.x + x*this.transform.localScale.x,
                            this.cubeWrapper.transform.position.y + y*this.transform.localScale.y,
                            this.cubeWrapper.transform.position.z + 0*this.transform.localScale.z),
                        new Quaternion(),
                        this.cubeWrapper.transform);
                    obj.GetComponent<Renderer>().material.color = color;
                    Cube cube = obj.GetComponent<Cube>();
                    cube.SetAffiliatedBlock(this);
                    cubes.Add(cube);
                }
            }
        }
    }

    public void SetColor(Color _color) {
        color = _color;
        foreach (Cube cube in cubes) {
            cube.gameObject.GetComponent<Renderer>().material.color = _color;
        }
    }

    public void SetBoard(GameObject _board) {
        boardObj = _board;
    }

    // 掴まれた際に呼び出し
    public void Grasp(HandControlManager? hand) {
        graspedHand = hand;
    }

    // 回転操作の際に呼び出し(回転開始)
    private void Rotate(bool clockwise) {
        if (graspedHand && leftTime == 0.0f) {
            leftTime = rotateTime;
            rotateWay = (clockwise ? 1 : -1);
        }
    }
    void RotateClockwise(object obj, EventArgs args) { Rotate(true); }
    void RotateAnticlockwise(object obj, EventArgs args) { Rotate(false); }

    void Start()
    {
        // イベントの購読
        EventManager.Instance.Subscribe(EventManager.Event.RightHandShakeClockwise, RotateClockwise);
        EventManager.Instance.Subscribe(EventManager.Event.RightHandShakeAnticlockwise, RotateAnticlockwise);
    }

    void Update()
    {
        // rotateTimeかけて90度回転させる
        if (leftTime > 0f) {
            if (leftTime > Time.deltaTime && graspedHand) {
                leftTime -= Time.deltaTime;
                this.cubeWrapper.transform.RotateAround((Vector3)prevHandPos, Vector3.forward, rotateWay*90f*Time.deltaTime/rotateTime);
            }
            else {
                this.cubeWrapper.transform.RotateAround((Vector3)prevHandPos, Vector3.forward, rotateWay*90f*leftTime/rotateTime);
                leftTime = 0.0f;
            }
        }

        // 掴まれていない & Boardに近い場合，Boardのマス目に合わせて位置を補正
        if (!graspedHand) {
            bool inFrame = false;
            foreach (Cube cube in cubes) {
                if (cube.inFrame) {inFrame = true; break;}
            }
            if (inFrame) {
                Vector3 temp = this.cubeWrapper.transform.position - boardObj.transform.position;
                temp = new Vector3(temp.x / this.transform.localScale.x, temp.y / this.transform.localScale.y, temp.z / this.transform.localScale.z);
                temp = new Vector3(Mathf.Round(temp.x), Mathf.Round(temp.y), Mathf.Round(temp.z));
                this.cubeWrapper.transform.position = boardObj.transform.position + Vector3.Scale(temp, this.transform.localScale);
            }
        }

        // 掴まれているとき，手の位置に追従
        if (graspedHand) {
            if (prevHandPos.HasValue) {
                this.transform.position = this.transform.position + graspedHand.gameObject.transform.position - (Vector3)prevHandPos;
            }
            prevHandPos = graspedHand.gameObject.transform.position;
        } else if (leftTime == 0.0f) {
            prevHandPos = null;
        }

    }

}
