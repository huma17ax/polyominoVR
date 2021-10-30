using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject boardPrefab;

    void Start()
    {
        GameObject board = Instantiate(boardPrefab);
        board.GetComponent<Board>().Load("./Assets/PazzleBoards/test.csv");
    }

    void Update()
    {
        
    }
}
