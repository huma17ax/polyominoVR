using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム管理
public class GameManager : MonoBehaviour
{

    public GameObject boardPrefab;
    private BoardSelecter selecter;

    void Start()
    {
        // 盤面の生成，ゲームの開始
        selecter = this.GetComponent<BoardSelecter>();
    }

    void Update()
    {
        if (selecter.decidedFilename != "") {
            selecter.Deactivate();
        }
    }
}
