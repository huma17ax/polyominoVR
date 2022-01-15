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
    private TextMeshPro textMesh;

    private bool isWaiting;
    private bool isSelecting;

    void Start()
    {
        // 盤面の生成，ゲームの開始
        selecter = this.GetComponent<BoardSelecter>();
        textMesh = this.transform.Find("WaitingText").GetComponent<TextMeshPro>();
        textMesh.enabled = true;
        isWaiting = true;
        isSelecting = false;
    }

    void Update()
    {
        if (isWaiting && OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)) {
            isWaiting = false;
            textMesh.enabled = false;
            selecter.Activate();
            isSelecting = true;
        }
        if (isSelecting && selecter.decidedFilename != "") {
            selecter.Deactivate();
            isSelecting = false;
            StartGame();
        }
    }

    async void StartGame() {
        await Task.Delay(200);
        GameObject board = Instantiate(
            boardPrefab,
            new Vector3(
                this.transform.position.x,
                this.transform.position.y,
                this.transform.position.z + 0.5f),
            new Quaternion());
        board.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        board.GetComponent<Board>().Load(selecter.decidedFilename);
    }
}
