using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Composer chooses sound samples based on user's settings such as scale type.

// File names: As4-pad-1.wav   (note name - sound type - file name)

public class Composer : MonoBehaviour
{
    public GameObject audioPrefab;
    
    public int scaleType; // 1:minor / 2:major
    public int baseNote; // c2: 36 c8: 88
    public int numOctaves; // 3

    private Dictionary<string, int> noteNames;
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

        noteNames = new Dictionary<string, int>();
        InitNoteNamesDict();
    }

    void Start()
    {
        
        switch (scaleType)
        {
            case 1: minorScaleNotes.CopyTo(scale,0); break;
            case 2: majorScaleNotes.CopyTo(scale, 0); break;
        }

        allowedNotesSize = 7 * numOctaves;
        allowedNotes = new int [allowedNotesSize];
        
        CalcAllowedNotes();
        
        ScanDirectory();
    }

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

    void InitNoteNamesDict()
    {
        int minNote = 0;
        for (int i = 0; i < 8; i++)
        {
            noteNames.Add("c" + i.ToString(), minNote + 12 * i);
            noteNames.Add("cs" + i.ToString(), minNote + 1 + 12 * i);
            noteNames.Add("d" + i.ToString(), minNote + 2 + 12 * i);
            noteNames.Add("ds" + i.ToString(), minNote + 3 + 12 * i);
            noteNames.Add("e" + i.ToString(), minNote + 4 + 12 * i);
            noteNames.Add("f" + i.ToString(), minNote + 5 + 12 * i);
            noteNames.Add("fs" + i.ToString(), minNote + 6 + 12 * i);
            noteNames.Add("g" + i.ToString(), minNote + 7 + 12 * i);
            noteNames.Add("gs" + i.ToString(), minNote + 8 + 12 * i);
            noteNames.Add("a" + i.ToString(), minNote + 9 + 12 * i);
            noteNames.Add("as" + i.ToString(), minNote + 10 + 12 * i);
            noteNames.Add("b" + i.ToString(), minNote + 11 + 12 * i);
        }
    }

    void CalcAllowedNotes()
    {
        int octaveCounter = 0;
        for (int i = 0; i < allowedNotesSize; i++)
        {
            allowedNotes[i] = baseNote + scale[i%7] + 12 * octaveCounter;
            Debug.Log("ALLOWED------->" + allowedNotes[i]);
            if (i % 7 == 6) octaveCounter++;
        }
    }

    void ScanDirectory()
    {
        string PATH = "Samples/";
        
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/"+PATH);

        FileInfo[] info = dir.GetFiles("*.wav");

        foreach (FileInfo f in info)
        {
            // print("Found: " + f.Name);
            
            string[] splittedName = f.Name.Split('-');

            int noteNum = NoteNameToNumber(splittedName[0]);

            if ((noteNum != -1) && IsInAllowedNotes(noteNum))
            {
                GameObject tmp = Instantiate(audioPrefab);
                tmp.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(PATH + f.Name.Replace(".wav", ""));
                
                print("LOADED: " + f.Name);
            }
        }
    }

    int NoteNameToNumber(string noteName)
    {
        Debug.Log("NAME----->" + noteName);
        if (noteNames.TryGetValue(noteName.ToLower(), out int number))
        {
            Debug.Log("DICT------->" + number);
            return number;
        }
        return -1;
    }

    bool IsInAllowedNotes(int num)
    {
        Debug.Log("num------->" + num);
        for (int i = 0; i < allowedNotesSize; i++)
        {
            if (allowedNotes[i] == num)
            {
                return true;
            }
        }
        return false;
    }

}
