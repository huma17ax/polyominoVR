using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// パズル盤面
public class Board : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject framePrefab;
    private Color[] colors = {
        new Color(1f,0.3f,0.3f,1f),
        new Color(0.3f,1f,0.3f,1f),
        new Color(0.3f,0.3f,1f,1f),
        new Color(1f,1f,0.3f,1f),
        new Color(1f,0.3f,1f,1f),
        new Color(0.3f,1f,1f,1f)
    };
    private List<List<bool>> shape;
    private List<List<int>> fillingStatus;
    public bool finishGame {get; private set; } = false;
    
    void Start()
    {
    }

    void Update()
    {   
    }

    // パズル形状ファイルを読み込み，Blockの生成
    public void Load(string path) {
        List<List<int>> board = new List<List<int>>();

        if (path == "Generater") {
            BoardGenerator generator = new BoardGenerator("./Assets/PazzleBoards/test.csv", 5);
            board = generator.Generate();
        }
        else {
            // TODO: *ライブラリ使う
            StreamReader reader = new StreamReader(path);

            while (!reader.EndOfStream) {
                string row = reader.ReadLine();
                board.Add(new List<int>(row.Split(',').Select(str => int.Parse(str))));
            }

            if (!isValidBoard(board)) return;
        }

        shape = board
            .Select(row =>
                row.Select(val => val!=0).ToList()
            ).ToList();
        fillingStatus = board
            .Select(row =>
                row.Select(val => 0).ToList()
            ).ToList();

        this.transform.position = new Vector3(
            this.transform.position.x - board[0].Count * this.transform.localScale.x / 2,
            this.transform.position.y - board.Count * this.transform.localScale.y / 2,
            this.transform.position.z
        );

        for (int y=0; y < board.Count; ++y) {
            for (int x=0; x < board[y].Count; ++x) {
                Instantiate(
                    framePrefab,
                    new Vector3(
                        this.transform.position.x + x*this.transform.localScale.x,
                        this.transform.position.y + y*this.transform.localScale.y,
                        this.transform.position.z + 0*this.transform.localScale.z),
                    new Quaternion(),
                    this.transform);
            }
        }

        // TODO: *charに変える，非有効マスの処理(#)
        List<int> ids = board.SelectMany(row => row).Distinct().ToList();

        Dictionary<int, (Vector2Int min, Vector2Int max)> blockRanges = new Dictionary<int, (Vector2Int, Vector2Int)>();
        foreach (int id in ids) {
            blockRanges[id] = (
                new Vector2Int(board.Select(row => row.Count).Max(), board.Count),
                new Vector2Int(0,0)
            );
        }

        for (int y=0; y < board.Count; ++y) { 
            for (int x=0; x < board[y].Count; ++x) {
                int id = board[y][x];
                blockRanges[id] = (
                    new Vector2Int(Math.Min(blockRanges[id].min.x, x), Math.Min(blockRanges[id].min.y, y)),
                    new Vector2Int(Math.Max(blockRanges[id].max.x, x), Math.Max(blockRanges[id].max.y, y))
                );
            }
        }

        List<Block> blocks = new List<Block>();
        int pos = 0;
        int num = 0;
        foreach (int id in ids) {
            List<List<bool>> blockShape = board
                .GetRange(blockRanges[id].min.y, blockRanges[id].max.y - blockRanges[id].min.y + 1)
                .Select(row => row.GetRange(blockRanges[id].min.x, blockRanges[id].max.x - blockRanges[id].min.x + 1))
                .Select(row => row.Select(val => val == id).ToList())
                .ToList();
            
            GameObject blockInstance = Instantiate(
                blockPrefab,
                new Vector3(
                    this.transform.position.x + pos * this.transform.localScale.x,
                    this.transform.position.y,
                    this.transform.position.z - 0.15f
                ),
                new Quaternion()
            );
            blockInstance.transform.localScale = this.transform.localScale;
            Block _temp_block = blockInstance.GetComponent<Block>();
            _temp_block.SetShape(blockShape);
            _temp_block.SetColor(colors[num++]);
            _temp_block.SetBoard(this);
            blocks.Add(_temp_block);
            pos += blockShape[0].Count + 1;
        }

        foreach (var block in blocks) {
            block.gameObject.transform.position = new Vector3(
                block.gameObject.transform.position.x + (board[0].Count-pos)/2*this.transform.localScale.x,
                block.gameObject.transform.position.y,
                block.gameObject.transform.position.z
            );
        }
    }

    bool isValidBoard(List<List<int>> board) {
        // TODO: *盤面正当性チェック
        Debug.Log((board.Count, board[0].Count));
        return true;
    }

    public void Fill(List<Vector2Int> positions) {
        foreach (var pos in positions) {
            if (pos.y < 0 || fillingStatus.Count <= pos.y) continue;
            if (pos.x < 0 || fillingStatus[pos.y].Count <= pos.x) continue;
            fillingStatus[pos.y][pos.x] += 1;
            Debug.Log("(" + pos.x+","+pos.y + ")");
        }
        if (isFull()) {
            Debug.Log("clear");
            finishGame = true;
        }
    }

    public void Empty(List<Vector2Int> positions) {
        foreach (var pos in positions) {
            if (pos.y < 0 || fillingStatus.Count <= pos.y) continue;
            if (pos.x < 0 || fillingStatus[pos.y].Count <= pos.x) continue;
            fillingStatus[pos.y][pos.x] -= 1;
        }
    }

    private bool isFull() {
        Debug.Log(string.Join(",", fillingStatus.Select(row => "(" + string.Join(",", row) + ")").ToList()));
        bool flg = true;
        for (int y=0; y < fillingStatus.Count; ++y) { 
            for (int x=0; x < fillingStatus[y].Count; ++x) {
                if (shape[y][x] && fillingStatus[y][x]!=1) {
                    flg = false;
                    break;
                } 
            }
        }
        return flg;
    }
}
