using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private AudioSource audioSource;
    private Transform cameraTransform;
    public float baseVol; //0.12
    public float radius; // 300

    private void Awake()
    {
        cameraTransform = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
    }
    
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.mass = Random.Range(0.1f, 2f);

        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Sounds");
        int index =  Random.Range(0, tmp.Length);
        
        audioSource = tmp[index].GetComponent<AudioSource>();

        float distance = Vector3.Distance(cameraTransform.position, transform.position);

        float vol = (float)(baseVol * distance / radius);
        audioSource.volume = vol;
        audioSource.pitch = Random.Range(0.2f, 1f);

        audioSource.PlayDelayed(Random.Range(0.05f,0.10f)*1.2f);//distance / 1000 + Random.Range(.2f,0.5f));
        //audioSource.Play();


      
    }
    
    void Update()
    {
        
    }

}
