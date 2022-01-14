using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator
{
    
    /// <summary>
    /// 盤面の形状
    /// True: マス目　False: 穴
    /// </summary>
    private List<List<bool>> frame;

    private int block_num;
    private int board_size;
    private static System.Random rand = new System.Random();

    public BoardGenerator(string path, int block_num){
        loadFrame(path);
        setBlockNum(block_num);
    }
    public BoardGenerator(List<List<int>> board, int block_num) {
        setFrame(board);
        setBlockNum(block_num);
    }

    /// <summary>
    /// ファイルから盤面形状を読み込み
    /// </summary>
    /// <param name="path">対象のファイルパス</param>
    public void loadFrame(string path) {
        StreamReader reader = new StreamReader(path);
        List<List<int>> board = new List<List<int>>();

        while (!reader.EndOfStream) {
            string row = reader.ReadLine();
            board.Add(new List<int>(row.Split(',').Select(str => int.Parse(str))));
        }
        setFrame(board);
    }

    /// <summary>
    /// 盤面データから盤面形状を読み込み
    /// </summary>
    /// <param name="board">盤面データ</param>
    public void setFrame(List<List<int>> board) {
        frame = board
            .Select(row =>
                row.Select(val => val!=0).ToList()
            ).ToList();
        board_size = frame
            .Sum(row => row.Count(flg => flg));
    }

    /// <summary>
    /// 盤面を構成するブロック数を設定
    /// </summary>
    /// <param name="num">ブロック数</param>
    public void setBlockNum(int num) {
        block_num = num;
    }

    /// <summary>
    /// 盤面を生成
    /// </summary>
    /// <returns>生成した盤面データ</returns>
    public List<List<int>> generate() {
        int id = 1;
        List<List<int>> board = frame
            .Select(row =>
                row.Select(flg => (flg ? id++ : 0)).ToList()
            ).ToList();
        
        var block_size = new Dictionary<int, int>();
        for (var i=1; i<=board_size; i++) {
            block_size[i] = 1;
        }
        
        for (var i=0; i<board_size-block_num; i++) {
            Tuple<int, int> pair = selectBlockPair(board, block_size);
            mergeBlock(board, block_size, pair.Item1, pair.Item2);
        }

        return board;
    }

    /// <summary>
    /// 結合するブロックペアを選択
    /// </summary>
    /// <param name="board">盤面データ</param>
    /// <param name="block_size">各ブロックの大きさ</param>
    /// <returns>選択したブロックペア</returns>
    private Tuple<int, int> selectBlockPair(List<List<int>> board, Dictionary<int, int> block_size) {

        // 隣接するブロックペアを列挙
        var neighbor = new HashSet<Tuple<int, int>>();
        for (var i=0; i<board.Count; i++) {
            for (var j=0; j<board[i].Count; j++) {
                if (board[i][j] == 0) continue;
                if (j != board[i].Count - 1 && board[i][j+1] != 0 && board[i][j] != board[i][j+1]) {
                    neighbor.Add(new Tuple<int,int>(
                        Math.Min(board[i][j], board[i][j+1]),
                        Math.Max(board[i][j], board[i][j+1])
                    ));
                }
                if (i != board.Count - 1 && j < board[i+1].Count && board[i+1][j] != 0 && board[i][j] != board[i+1][j]) {
                    neighbor.Add(new Tuple<int,int>(
                        Math.Min(board[i][j], board[i+1][j]),
                        Math.Max(board[i][j], board[i+1][j])
                    ));
                }
            }
        }

        // 各ブロックに隣接する他ブロックの数を算出
        var neighbor_num = new Dictionary<int, int>();
        foreach (var pair in neighbor) {
            if (neighbor_num.ContainsKey(pair.Item1)) neighbor_num[pair.Item1] += 1;
            else neighbor_num[pair.Item1] = 1;
            if (neighbor_num.ContainsKey(pair.Item2)) neighbor_num[pair.Item2] += 1;
            else neighbor_num[pair.Item2] = 1;
        }

        // 各ペアに対して，そのペアを結合する確率を算出
        double sum = 0;
        var probabilities = new List<Tuple<Tuple<int,int>, double>>();
        foreach (var pair in neighbor) {
            double prob = calcProbability(block_size, neighbor_num, pair.Item1, pair.Item2);
            probabilities.Add(new Tuple<Tuple<int, int>, double>(pair, prob));
            sum += prob;
        }

        // 乱数をもとに，結合するペアを決定
        double res = rand.NextDouble() * sum;
        foreach (var prob in probabilities) {
            if (res <= prob.Item2) {
                return prob.Item1;
            } else {
                res -= prob.Item2;
            }
        }
        return probabilities.Last().Item1;
    }

    /// <summary>
    /// ブロックのペアに対して，そのペアを結合する確率を計算する
    /// </summary>
    /// <param name="block_size">各ブロックの大きさ</param>
    /// <param name="neighbor_num">隣接する他ブロックの数</param>
    /// <param name="block_id1">ブロックID1</param>
    /// <param name="block_id2">ブロックID2</param>
    /// <returns>ペアに対する確率</returns>
    private double calcProbability(Dictionary<int, int> block_size, Dictionary<int, int> neighbor_num, int block_id1, int block_id2) {
        double prob = 
            (board_size / block_size[block_id1]) / neighbor_num[block_id1]
            * (board_size / block_size[block_id2]) / neighbor_num[block_id2];
        prob = prob * prob;
        return prob;
    }

    /// <summary>
    /// 盤面上の2つのブロックを結合する
    /// </summary>
    /// <param name="board">盤面データ</param>
    /// <param name="block_size">各ブロックの大きさ</param>
    /// <param name="block_id1">結合するブロックID1</param>
    /// <param name="block_id2">結合するブロックID2</param>
    private void mergeBlock(List<List<int>> board, Dictionary<int, int> block_size, int block_id1, int block_id2) {

        block_size[block_id1] += block_size[block_id2];
        block_size[block_id2] = 0;

        for (var i=0; i<board.Count; i++) {
            for (var j=0; j<board[i].Count; j++) {
                if (board[i][j] == block_id2) board[i][j] = block_id1;
            }
        }

    }

}
