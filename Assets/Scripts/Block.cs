using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private List<List<bool>> shape;
    private GameObject cubeWrapper;
    private GameObject boardObj;
    public GameObject cubePrefab;
    public GameObject cubeWrapperPrefab;

    public bool test;
    private const float rotateTime = 0.1f;
    private float leftTime;
    private int rotateWay = 0;

    private List<Cube> cubes = new List<Cube>();

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
                    Cube cube = obj.GetComponent<Cube>();
                    cubes.Add(cube);
                }
            }
        }
    }

    public void SetBoard(GameObject _board) {
        boardObj = _board;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (test) {
            test = false;
            leftTime = rotateTime;
            rotateWay = -1;
        }
        if (leftTime > 0f) {
            if (leftTime > Time.deltaTime) {
                leftTime -= Time.deltaTime;
                this.cubeWrapper.transform.Rotate(new Vector3(0f,0f,rotateWay*90f*Time.deltaTime/rotateTime));
            }
            else {
                this.cubeWrapper.transform.Rotate(new Vector3(0f,0f,rotateWay*90f*leftTime/rotateTime));
                leftTime = 0.0f;
            }
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            this.transform.Translate(Vector3.left * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            this.transform.Translate(Vector3.right * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
            this.transform.Translate(Vector3.up * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            this.transform.Translate(Vector3.down * Time.deltaTime);
        }
        if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) {
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
    }

}
