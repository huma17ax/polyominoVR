using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardSelectItem : MonoBehaviour
{

    private Vector3 initPos;
    private BoardSelecter selecter;
    private string filename;

    private const float disappearTime = 0.15f;
    private float leftTime;
    private Vector3 initScale;

    void Start()
    {
    }

    void Update()
    {
        if (transform.position != initPos && selecter) {
            selecter.DecideBoard(filename);
        }
        if (leftTime > 0) {
            this.transform.localScale = initScale * (leftTime / disappearTime);
            leftTime -= Time.deltaTime;
            if (leftTime <= 0.0f) {
                Destroy(this.gameObject);
            }
        }
    }

    public void SetSelecter(BoardSelecter _selecter) {
        selecter = _selecter;
    }

    public void SetUp(Vector3 _pos, BoardSelecter _selecter, string _text) {
        initPos = _pos;
        selecter = _selecter;
        filename = _text;
        this.transform.position = initPos;
        Block block = this.GetComponent<Block>();
        block.SetShape(new List<List<bool>>(){new List<bool>(){true}});
        block.SetColor(new Color(1f,1f,1f,1f));
        this.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        this.transform.LookAt(selecter.transform);
        this.transform.Find("Text").GetComponent<TextMeshPro>().text = filename.Split('\\').Last().Replace(".csv", "");
    }

    public void Disappear() {
        leftTime = disappearTime;
        initScale = this.transform.localScale;
    }

}
