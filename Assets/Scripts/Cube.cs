using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public AudioSource audioSource;
    private Transform cameraTransform;
    public float baseVol; //0.12
    public float radius; // 300

    private void Awake()
    {
        cameraTransform = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
    }
    void Start()
    {
        float distance = Vector3.Distance(cameraTransform.position, transform.position);

        float vol= (float)(baseVol * distance / radius);
        audioSource.volume = vol;
        audioSource.pitch = Random.Range(0.7f, 1f);

        audioSource.PlayDelayed(distance/1000 + Random.Range(0f,0.5f));
    }

    void Update()
    {
        
    }
}
