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
    
    private int baseNote;
    private int scaleType;
    private int numOctaves;

    private ScaleManager scaleManager;
    private Dictionary<string, int> noteNames;

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
    int baseNote;
    int scaleType;
    int numOctaves;

    private int[] allowedNotes;
    private int allowedNotesSize;

    private int[] majorScaleNotes;
    private string[] majorScaleChords;

    private int[] minorScaleNotes;
    private string[] minorScaleChords;

    private int[] scale;
    private string[] scaleChords;


    public int[] currentChord;
    private ChordManager chordManager;


    public ScaleManager(int baseNote, int scaleType, int numOctaves)
    {
        this.baseNote = baseNote;
        this.numOctaves = numOctaves;
        this.scaleType = scaleType;

        majorScaleNotes = new int[7];
        majorScaleChords = new string[7];

        minorScaleNotes = new int[7];
        minorScaleChords = new string[7];

        InitScales();

        scale = new int[7];
        SetScaleType();

        CalculateScale(this.baseNote, this.scaleType, this.numOctaves);

        chordManager = new ChordManager();
        GetRandomChordInScale();
        
    }

    public void CalculateScale(int baseNote, int scaleType, int numOctaves)
    {
        this.baseNote = baseNote;
        this.numOctaves = numOctaves;
        this.scaleType = scaleType;
        
        CalcAllowedNotes(false);
    }

    public void GetRandomChordInScale()
    {
        
        int index = Random.Range(0, 7);
        string[] notes = chordManager.GetChordNotes(scaleChords[index]);
        int[] result = new int[notes.Length];

        for (int i=0; i< notes.Length; i++)
        {
            if(!notes[i].Contains("b"))
            {
                result[i] = int.Parse(notes[i]);
            }
            else
            {
                result[i] = int.Parse(notes[i])-1;
            }
        }

        result.CopyTo(currentChord, 0);
    }

    private void CalcAllowedNotes(bool isChordMode)
    {
        int baseVal;
        if (!isChordMode)
            baseVal = 7;
        else
            baseVal = currentChord.Length;
     
        allowedNotesSize = baseVal * this.numOctaves;
        allowedNotes = new int[allowedNotesSize];

        int octaveCounter = 0;
        for (int i = 0; i < allowedNotesSize; i++)
        {
            allowedNotes[i] = baseNote + currentChord[i % baseVal] + 12 * octaveCounter;

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
        if (this.scaleType == 1)
        {
            minorScaleNotes.CopyTo(scale, 0);
            minorScaleChords.CopyTo(scaleChords, 0);
        }
        else if (this.scaleType == 2)
        {
            majorScaleNotes.CopyTo(scale, 0);
            majorScaleChords.CopyTo(scaleChords, 0);

        }
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

        majorScaleChords[0] = "M";
        majorScaleChords[1] = "m";
        majorScaleChords[2] = "m";
        majorScaleChords[3] = "M";
        majorScaleChords[4] = "M";
        majorScaleChords[5] = "m";
        majorScaleChords[6] = "dim";

        minorScaleChords[0] = "m";
        minorScaleChords[1] = "dim";
        minorScaleChords[2] = "M";
        minorScaleChords[3] = "m";
        minorScaleChords[4] = "m";
        minorScaleChords[5] = "M";
        minorScaleChords[6] = "M";
    }

}

class ChordManager
{
    private Dictionary<string, string[]> chordTypes;

    public ChordManager()
    {
        InitChordTypes();
    }

    public void InitChordTypes()
    { 
        chordTypes.Add("M", new string [3] { "0","2","4" });
        chordTypes.Add("m", new string[3] { "0", "2", "4" });
        chordTypes.Add("dim", new string[3] { "0", "2", "4b" });
    }

    public string[] GetChordNotes(string name)
    {
        return (0, 0, 0);
    }
}
