using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSelecter : MonoBehaviour
{

    private string[] filenames;
    public GameObject selectItemPrefab;
    public string decidedFilename;
    private List<BoardSelectItem> items = new List<BoardSelectItem>();

    private float timeout = 1.0f;
    private bool fin = false;

    void Start()
    {
        LoadFiles();
        decidedFilename = "";
    }

    void Update()
    {
        if (timeout > 0) timeout -= Time.deltaTime;
        if (timeout < 0 && !fin) {
            InstantiateItems();
            fin = true;
        }
    }

    void LoadFiles()
    {
        filenames = Directory.GetFiles("./Assets/PazzleBoards", "*.csv", System.IO.SearchOption.AllDirectories);
    }

    void InstantiateItems() {
        float radian = (float)Math.PI / (filenames.Length + 1);
        float radius = 0.5f;
        for (var i=0; i<filenames.Length; i++) {
            GameObject obj = Instantiate(
                selectItemPrefab,
                new Vector3(
                    this.transform.position.x + radius * (float)Math.Cos(radian*(i+1)),
                    this.transform.position.y + 0*this.transform.localScale.y,
                    this.transform.position.z + radius * (float)Math.Sin(radian*(i+1))
                ),
                new Quaternion());
            BoardSelectItem item = obj.GetComponent<BoardSelectItem>();
            item.SetUp(
                new Vector3(
                    this.transform.position.x + radius * (float)Math.Cos(radian*(i+1)),
                    this.transform.position.y + 0*this.transform.localScale.y,
                    this.transform.position.z + radius * (float)Math.Sin(radian*(i+1))
                ),
                this,
                filenames[i]
            );
            items.Add(item);
        }
    }

    public void DecideBoard(string filename) {
        if (decidedFilename == "") decidedFilename = filename;
    }

    public void Deactivate() {
        foreach (var item in items) {
            item.Disappear();
        }
        items = new List<BoardSelectItem>();
    }
}
