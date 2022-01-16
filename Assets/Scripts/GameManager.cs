using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

// ゲーム管理
public class GameManager : MonoBehaviour
{

    public GameObject boardPrefab;
    private BoardSelecter selecter;
    private TextMeshPro waitingTextMesh;
    private TextMeshPro finishTextMesh;
    private Board board;

    private bool isWaiting;
    private bool isSelecting;
    private bool isGaming;

    void Start()
    {
        // 盤面の生成，ゲームの開始
        selecter = this.GetComponent<BoardSelecter>();
        waitingTextMesh = this.transform.Find("WaitingText").GetComponent<TextMeshPro>();
        waitingTextMesh.enabled = true;
        finishTextMesh = this.transform.Find("FinishText").GetComponent<TextMeshPro>();
        finishTextMesh.enabled = false;
        isWaiting = true;
        isSelecting = false;
    }

    void Update()
    {
        if (isWaiting && OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)) {
            isWaiting = false;
            waitingTextMesh.enabled = false;
            selecter.Activate();
            isSelecting = true;
        }
        if (isSelecting && selecter.decidedFilename != "") {
            selecter.Deactivate();
            isSelecting = false;
            StartGame();
        }
        if (isGaming && board.finishGame) {
            finishTextMesh.enabled = true;
        }
    }

    async void StartGame() {
        await Task.Delay(200);
        GameObject boardObj = Instantiate(
            boardPrefab,
            new Vector3(
                this.transform.position.x,
                this.transform.position.y,
                this.transform.position.z + 0.5f),
            new Quaternion());
        boardObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        board = boardObj.GetComponent<Board>();
        board.Load(selecter.decidedFilename);
        isGaming = true;
    }
}
