using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public AudioSource audioSource;
    public Transform cameraTransform;
    public float baseVol; //0.12
    public float radius; // 300

    private void Awake()
    {
        cameraTransform = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
    }
    void Start()
    {
        float distance = Vector3.Distance(cameraTransform.position, transform.position);
        //Debug.Log("DISTANCE " + distance);

        audioSource.volume = (float)(baseVol * distance / radius);
        audioSource.PlayDelayed(distance / 1000);
        //audioSource.Play();
    }

    void Update()
    {
        
    }
}
