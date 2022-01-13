using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム管理
public class GameManager : MonoBehaviour
{

    public GameObject boardPrefab;

    void Start()
    {
        // 盤面の生成，ゲームの開始
        GameObject board = Instantiate(boardPrefab, new Vector3(-0.7f,-0.7f,0.5f), new Quaternion());
        board.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        board.GetComponent<Board>().Load("./Assets/PazzleBoards/test.csv");
    }

    void Update()
    {
        
    }
}
