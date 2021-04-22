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

    private ComposerController controller;

    void Start()
    {

        controller = new ComposerController(baseNote, scaleType, numOctaves);
        
        ScanDirectory();
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

            int noteNum = controller.NoteNameToNumber(splittedName[0]);

            if ((noteNum != -1) && controller.IsInAllowedNotes(noteNum))
            {
                GameObject tmp = Instantiate(audioPrefab);
                tmp.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(PATH + f.Name.Replace(".wav", ""));
                
                print("LOADED: " + f.Name);
            }
        }
    }
}

class ComposerController
{
    private Dictionary<string, int> noteNames;


    private int baseNote;
    private int scaleType;
    private int numOctaves;

    private ScaleManager scaleManager;

    public ComposerController(int baseNote, int scaleType, int numOctaves)
    {
        this.baseNote = baseNote;
        this.scaleType = scaleType;
        this.numOctaves = numOctaves;

        noteNames = new Dictionary<string, int>();
        InitNoteNamesDict();

        scaleManager = new ScaleManager(this.baseNote, this.scaleType, this.numOctaves);
    }

    public int NoteNameToNumber(string noteName)
    {
        if (noteNames.TryGetValue(noteName.ToLower(), out int number))
        {
            return number;
        }
        return -1;
    }

    public bool IsInAllowedNotes(int num)
    {
        return scaleManager.IsInAllowedScaleNotes(num);
    }

    private void InitNoteNamesDict()
    {
        for (int i = 0; i < 8; i++)
        {
            noteNames.Add("c" + i.ToString(), 12 * i);
            noteNames.Add("cs" + i.ToString(), 1 + 12 * i);
            noteNames.Add("d" + i.ToString(), 2 + 12 * i);
            noteNames.Add("ds" + i.ToString(), 3 + 12 * i);
            noteNames.Add("e" + i.ToString(), 4 + 12 * i);
            noteNames.Add("f" + i.ToString(), 5 + 12 * i);
            noteNames.Add("fs" + i.ToString(), 6 + 12 * i);
            noteNames.Add("g" + i.ToString(), 7 + 12 * i);
            noteNames.Add("gs" + i.ToString(), 8 + 12 * i);
            noteNames.Add("a" + i.ToString(), 9 + 12 * i);
            noteNames.Add("as" + i.ToString(), 10 + 12 * i);
            noteNames.Add("b" + i.ToString(), 11 + 12 * i);
        }
    }
}

class ScaleManager
{
    private int[] majorScaleNotes;
    private int[] minorScaleNotes;
    private int[] scale;

    private int[] allowedNotes;
    private int allowedNotesSize;

    int baseNote;
    int scaleType;
    int numOctaves;

    public ScaleManager(int baseNote, int scaleType, int numOctaves)
    {
        this.baseNote = baseNote;
        this.numOctaves = numOctaves;
        this.scaleType = scaleType;

        majorScaleNotes = new int[7];
        minorScaleNotes = new int[7];
        InitScales();

        scale = new int[7];
        SetScaleType();

        CalculateScale(this.baseNote, this.scaleType, this.numOctaves);
    }

    public void CalculateScale(int baseNote, int scaleType, int numOctaves)
    {
        this.baseNote = baseNote;
        this.numOctaves = numOctaves;
        this.scaleType = scaleType;

        allowedNotesSize = 7 * this.numOctaves;
        allowedNotes = new int[allowedNotesSize];
        
        CalcAllowedNotes();
    }

    private void CalcAllowedNotes()
    {
        int octaveCounter = 0;
        for (int i = 0; i < allowedNotesSize; i++)
        {
            allowedNotes[i] = baseNote + scale[i % 7] + 12 * octaveCounter;

            if (i % 7 == 6) octaveCounter++;
        }
    }
    
    public bool IsInAllowedScaleNotes(int num)
    {
        for (int i = 0; i < allowedNotesSize; i++)
        {
            if (allowedNotes[i] == num)
            {
                return true;
            }
        }
        return false;
    }

    private void SetScaleType()
    {
        if (this.scaleType== 1)
            minorScaleNotes.CopyTo(scale, 0);
        else if (this.scaleType == 2)
            majorScaleNotes.CopyTo(scale, 0);
    }

    private void InitScales()
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

class ChordManager
{
    


}
