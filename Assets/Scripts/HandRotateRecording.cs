using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class HandRotateRecording : MonoBehaviour
{
    private List<Vector3> records = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.localEulerAngles.x != 0.0) {
            records.Add(this.transform.localEulerAngles);
        }

        if (records.Count == 1000) {
            saveRecordToCSV();
        }
    }

    void saveRecordToCSV()
    {
        StreamWriter file = new StreamWriter("./record.csv", true, Encoding.UTF8);
        foreach (Vector3 vec in records) {
            file.WriteLine(string.Format("{0},{1},{2}", vec.x, vec.y, vec.z));
        }
        file.Close();

        records = new List<Vector3>();
        Debug.Log("save csv");
    }

    void OnDestroy()
    {
        saveRecordToCSV();
    }
}