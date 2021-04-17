using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Composer chooses sound samples based on user's settings such as scale type.

// File names: A4-pad-1.wav   (note number - sound type - file name)

public class Composer : MonoBehaviour
{
    public GameObject audioPrefab;
    
    public int scaleType; // 1:minor / 2:major
    public int baseNote;
    
    public int minNote; // 36
    public int maxNote; // 88
    public int numOctaves; // 3

    private int[] majorScaleNotes;
    private int[] minorScaleNotes;
    private int[] scale;

    private int[] allowedNotes;
    int allowedNotesSize;

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

        allowedNotesSize = 7 * numOctaves;
        allowedNotes = new int [allowedNotesSize];
        
        CalcAllowedNotes();
        

        ScanDirectory();
    }

    // Update is called once per frame
    void Update()
    {
       
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
        for (int i = 0; i < allowedNotesSize; i++)
        {
            allowedNotes[i] = baseNote + scale[i%7] + 12 * (i % 7);
        }
    }

    void ScanDirectory()
    {
        string PATH = "Samples/";
        
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/"+PATH);

        FileInfo[] info = dir.GetFiles("*.wav");

        foreach (FileInfo f in info)
        {
            print("Found: " + f.Name);
            string[] splittedName = f.Name.Split('-');

            if (IsInAllowedNotes(NoteNameToNumber(splittedName[0])))
            {
                GameObject tmp = Instantiate(audioPrefab);
                tmp.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(f.Name);
            }
        }

    }

    int NoteNameToNumber(string noteName)
    {


        return -1;
    }

    bool IsInAllowedNotes(int num)
    {
        for (int i = 0; i < allowedNotesSize; i++)
        {
            if (allowedNotes[i] == num)
                return true;
        }
        return false;
    }
}
