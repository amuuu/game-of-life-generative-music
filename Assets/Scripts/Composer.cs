using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Composer chooses sound samples based on user's settings such as scale type.

// File names: 36-pad-minilogue-1.wav

public class Composer : MonoBehaviour
{
    public int scaleType; // 0:None / 1:minor / 2:major
    public int baseNote;
    
    public int minNote; // 36
    public int maxNote; // 88
    public int numOctaves; // 3

    private int[] majorScaleNotes;
    private int[] minorScaleNotes;

    // Start is called before the first frame update
    void Start()
    {
        majorScaleNotes = new int[7];
        minorScaleNotes = new int[7];
        InitScales();
        
    }

    // Update is called once per frame
    void Update()
    {
        AudioSource a = new AudioSource();
        a.clip = Resources.Load<AudioClip>("t");
    }

    void InitScales()
    {
        majorScaleNotes[0] = 0;
        majorScaleNotes[1] = 2;
        majorScaleNotes[2] = 4;
        majorScaleNotes[3] = 5;
        majorScaleNotes[4] = 7;
        majorScaleNotes[5] = 9;
        majorScaleNotes[6] = 11;

        minorScaleNotes[0] = 0;
        minorScaleNotes[1] = 2;
        minorScaleNotes[2] = 3;
        minorScaleNotes[3] = 5;
        minorScaleNotes[4] = 7;
        minorScaleNotes[5] = 8;
        minorScaleNotes[6] = 10;
    }
}
