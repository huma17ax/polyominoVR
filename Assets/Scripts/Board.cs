using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject framePrefab;
    
    void Start()
    {
    }

    void Update()
    {   
    }

    public void Load(string path) {
        // *ライブラリ使う
        StreamReader reader = new StreamReader(path);
        List<List<int>> board = new List<List<int>>();

        while (!reader.EndOfStream) {
            string row = reader.ReadLine();
            board.Add(new List<int>(row.Split(',').Select(str => int.Parse(str))));
        }

        if (!isValidBoard(board)) return;

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

        // *charに変える，非有効マスの処理(#)
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
        foreach (int id in ids) {
            List<List<bool>> blockShape = board
                .GetRange(blockRanges[id].min.y, blockRanges[id].max.y - blockRanges[id].min.y + 1)
                .Select(row => row.GetRange(blockRanges[id].min.x, blockRanges[id].max.x - blockRanges[id].min.x + 1))
                .Select(row => row.Select(val => val == id).ToList())
                .ToList();
            
            GameObject blockInstance = Instantiate(blockPrefab, new Vector3(this.transform.position.x - 1+0.4f*(pos++),this.transform.position.y,this.transform.position.z-0.15f), new Quaternion());
            blockInstance.transform.localScale = this.transform.localScale;
            Block _temp_block = blockInstance.GetComponent<Block>();
            _temp_block.SetShape(blockShape);
            _temp_block.SetBoard(this.gameObject);
            blocks.Add(_temp_block);
        }
    }

    bool isValidBoard(List<List<int>> board) {
        // *盤面正当性チェック
        Debug.Log((board.Count, board[0].Count));
        return true;
    }
}
