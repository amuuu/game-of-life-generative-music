using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    public int rate; // 10
    void Start()
    {
        
    }

    void Update()
    {

        if(Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector3(transform.position.x + rate, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x - rate, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + rate);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - rate);
        }
        if (Input.GetKey(KeyCode.G))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - rate, transform.position.z);
        }
        if (Input.GetKey(KeyCode.H))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + rate, transform.position.z);
        }
    }
}
