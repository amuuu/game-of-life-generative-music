using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    public int movementRate; // 10
    public int rotationRate; // 4

    private Vector3 rotation;

    Vector3 lastAngles;
    Vector3 lastPosition;


    void Start()
    {
        
    }

    void Update()
    {

        if(Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector3(transform.position.x + movementRate, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x - movementRate, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + movementRate);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - movementRate);
        }
        if (Input.GetKey(KeyCode.G))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - movementRate, transform.position.z);
        }
        if (Input.GetKey(KeyCode.H))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + movementRate, transform.position.z);
        }
        if (Input.GetKey(KeyCode.T))
        {
            rotation = transform.eulerAngles;
            rotation.x += rotationRate;
            transform.eulerAngles = rotation;
        }
        if (Input.GetKey(KeyCode.Y))
        {
            rotation = transform.eulerAngles;
            rotation.x -= rotationRate;
            transform.eulerAngles = rotation;
        }


        // quick bird's eye view
        if(Input.GetKeyDown(KeyCode.M))
        {

            lastPosition = transform.position;
            lastAngles = transform.eulerAngles;
            
            rotation = transform.eulerAngles;
            rotation.x = 90;
            transform.eulerAngles = rotation;

            transform.position = new Vector3(transform.position.x, 800, transform.position.z);
        }

        // quick bird's eye view REVERT
        if (Input.GetKeyDown(KeyCode.N))
        {
            rotation = lastAngles;
            transform.eulerAngles = rotation;
            transform.position = lastPosition;
        }
    }
}
