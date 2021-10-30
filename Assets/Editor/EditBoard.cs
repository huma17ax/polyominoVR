using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.IO;
using System.Text;

public class EditBoard : EditorWindow
{

    private Vector2Int gridSize = new Vector2Int(1,1);

    private List<List<int>> boardStats = new List<List<int>>();

    private readonly Color[] bgColors = {Color.black, Color.red, Color.green, Color.blue, Color.white, Color.magenta, Color.yellow, Color.cyan, Color.gray};
    private enum blockType {
        None = 0, A, B, C, D, E, F, G, H
    }
    private blockType selectedBlockType;

    [MenuItem("Editor/EditBoard")]
    private static void Create() {
        GetWindow<EditBoard>("EditBoard");
    }

    private void OnGUI(){

        using (new GUILayout.VerticalScope()) {
            gridSize.x = EditorGUILayout.DelayedIntField("Board Size X", gridSize.x);
            gridSize.y = EditorGUILayout.DelayedIntField("Board Size Y", gridSize.y);
            clampGridsize();
            setBoardSize(gridSize);
            selectedBlockType = (blockType)EditorGUILayout.EnumPopup("Block Select", selectedBlockType);
            if (GUILayout.Button("Export")) Export();
        }

        using (new GUILayout.AreaScope(new Rect(20, 100, 30*gridSize.x, 30*gridSize.y))) {
            
            for (int x=0; x < gridSize.x; ++x) {
                for (int y=0; y < gridSize.y; ++y) {
                    GUI.backgroundColor = bgColors[boardStats[y][x]];
                    if (GUI.Button(new Rect(30*x,30*y,30,30), getBlockTypeName(boardStats[y][x]))) {
                        setBoardStat(new Vector2Int(x, y), (int)selectedBlockType);
                    }
                }
            }
        }

    }

    private void clampGridsize() {
        if (gridSize.x < 1) gridSize.x = 1;
        if (gridSize.y < 1) gridSize.y = 1;
    }

    private void setBoardSize(Vector2Int size) {
        if (boardStats.Count == size.y && boardStats[0].Count == size.x) return;
        boardStats.AddRange(Enumerable.Range(0,Mathf.Max(0, size.y - boardStats.Count)).Select(_ => new List<int>()));
        boardStats.RemoveRange(size.y, boardStats.Count - size.y);
        boardStats = boardStats.Select(row => {
            row.AddRange(Enumerable.Repeat(0,Mathf.Max(0, size.x - row.Count)));
            row.RemoveRange(size.x, row.Count - size.x);
            return row;
        }).ToList();
    }

    private void setBoardStat(Vector2Int pos, int stat) {
        boardStats[pos.y][pos.x] = stat;
    }

    private string getBlockTypeName(int value) {
        if (value == (int)blockType.None) return "";
        return Enum.GetName(typeof(blockType), value);
    }

    private void Export() {
        string filename = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
        StreamWriter file = new StreamWriter("./Assets/PazzleBoards/"+filename+".csv", true, Encoding.UTF8);
        foreach (List<int> row in boardStats) {
            file.WriteLine(string.Format(string.Join(",", row)));
        }
        file.Close();
        Debug.Log("save csv");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

}
