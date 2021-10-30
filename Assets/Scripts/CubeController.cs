using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    private int status = -1;
    private Color[] colors = new Color[11]{Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.white, Color.black, Color.gray, Color.magenta, new Color(0.7f, 0.3f, 1f), new Color(0.3f, 1f, 0.7f)};

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setStatus(int val) {
        status = val;
        GetComponent<Renderer>().material.color = colors[val];
    }
    public int getStatus() {
        return status;
    }
}
