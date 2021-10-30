using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool inFrame {get; private set;} = false;

    private List<Collider> triggeredColliders = new List<Collider>();

    void Start()
    {
        
    }

    void Update()
    {
        inFrame = false;
        foreach (Collider collider in triggeredColliders) {
            float distance = Vector3.Distance(collider.gameObject.transform.position, this.transform.position);
            if (distance < Mathf.Sqrt(3*Mathf.Pow(0.5f, 2))) {
                inFrame = true;
                break;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "frame") 
        {
            triggeredColliders.Add(other);
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "frame")
        {
            triggeredColliders.Remove(other);
        }
    }
}
