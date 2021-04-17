using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Composer chooses sound samples based on user's settings such as scale type.

// File names: 36-pad-minilogue-1.wav   (note number - sound type - sound pack name - file name)

public class Composer : MonoBehaviour
{
    public int scaleType; // 1:minor / 2:major
    public int baseNote;
    
    public int minNote; // 36
    public int maxNote; // 88
    public int numOctaves; // 3

    private int[] majorScaleNotes;
    private int[] minorScaleNotes;
    private int[] scale;

    private int[] allowedNotes;

    private void Awake()
    {
        majorScaleNotes = new int[7];
        minorScaleNotes = new int[7];
        InitScales();

        scale = new int[7];
    }

    void Start()
    {
        switch (scaleType)
        {
            case 1: scale = minorScaleNotes; break;
            case 2: scale = majorScaleNotes; break;
        }

        allowedNotes = new int [7];
        CalcAllowedNotes();
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

    void CalcAllowedNotes()
    {
        for (int i = 0; i < 7; i++)
        {
            allowedNotes[i] = baseNote + scale[i];
        }
    }


}
