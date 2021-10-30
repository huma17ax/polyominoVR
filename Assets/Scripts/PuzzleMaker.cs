using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PuzzleMaker : MonoBehaviour
{
    public static int size = 11;
    public GameObject cubePrefab;
    private GameObject[,] cubes = new GameObject[size,size];
    private float timeElapsed;
    private System.Random random = new System.Random();
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                cubes[i, j] = Instantiate(cubePrefab, new Vector3(i-5, j-5, 20), Quaternion.identity);
            }
        }
        
        int[] list = Enumerable.Range(0, size*size).OrderBy(i => Guid.NewGuid()).ToArray();
        for (int i = 0; i < size*size; i++) {
            
            int j = list[i];
            cubes[(int)Math.Floor((double)j/size), j%size].GetComponent<CubeController>().setStatus(i%size);
            // cubes[(int)Math.Floor((double)i/5), i%5].GetComponent<CubeController>().setStatus(i%5);
        }

        timeElapsed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > 0.00f) {
            updateCubes();
            timeElapsed = 0f;
        }

    }

    void updateCubes() {
        int rand;
        string dir;
        int offset;
        while(true) {
            rand = random.Next(0, size*size);
            int rand2 = random.Next(0, 4);
            dir = new string[4]{"UP", "DOWN", "RIGHT", "LEFT"}[rand2];
            offset = new int[4]{size, -size, 1, -1}[rand2];
            // Debug.Log(rand + " : " + rand2 + " : "  + dir);
            if (rand < size && dir == "DOWN") continue;
            if (rand >= size*(size-1) && dir == "UP") continue;
            if (rand%size == 0 && dir == "LEFT") continue;
            if (rand%size == size-1 && dir == "RIGHT") continue;

            float preGrav = calcGravity(cubes);

            int temp_status = cubes[rand%size, (int)Math.Floor((double)rand/size)].GetComponent<CubeController>().getStatus();
            cubes[rand%size, (int)Math.Floor((double)rand/size)].GetComponent<CubeController>().setStatus(cubes[(rand+offset)%size, (int)Math.Floor((double)(rand+offset)/size)].GetComponent<CubeController>().getStatus());
            cubes[(rand+offset)%size, (int)Math.Floor((double)(rand+offset)/size)].GetComponent<CubeController>().setStatus(temp_status);

            if (random.Next(0, 20) == 0) break;

            if (preGrav < calcGravity(cubes)) {

                cubes[(rand+offset)%size, (int)Math.Floor((double)(rand+offset)/size)].GetComponent<CubeController>().setStatus(cubes[rand%size, (int)Math.Floor((double)rand/size)].GetComponent<CubeController>().getStatus());
                cubes[rand%size, (int)Math.Floor((double)rand/size)].GetComponent<CubeController>().setStatus(temp_status);

                continue;
            }
            // Debug.Log("---------");
            break;
        }
    }

    Vector2[] getGravityCenter(GameObject[,] objs) {
        Vector2[] ret = new Vector2[size];
        for (int i = 0; i < size; i++) {
            ret[i] = new Vector2(0,0);
        }
        for (int i = 0;i < size*size; i++) {
            int temp = objs[i%size, (int)Math.Floor((double)i/size)].GetComponent<CubeController>().getStatus();
            ret[temp].x += i%size;
            ret[temp].y += (int)Math.Floor((double)i/size);
        }
        for (int i = 0; i < size; i++) {
            ret[i] /= size;
        }
        return ret;
    }

    float calcGravity(GameObject[,] objs) {
        float ret = 0;

        Vector2[] center = getGravityCenter(objs);

        for (int i = 0; i < size*size; i++) {
            int temp = objs[i%size, (int)Math.Floor((double)i/size)].GetComponent<CubeController>().getStatus();
            ret += Vector2.Distance(center[temp], new Vector2(i%size, (int)Math.Floor((double)i/size)));
        }

        return ret;
    }
}
