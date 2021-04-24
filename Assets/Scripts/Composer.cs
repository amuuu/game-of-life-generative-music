using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


///define NUM_SCALE_NOTES = 7;

// Composer chooses sound samples based on user's settings such as scale type.

// File names: As4-pad-1.wav   (note name - sound type - file name)

public class Composer : MonoBehaviour
{
    public GameObject audioPrefab;
    
    public int scaleType; // 1:minor / 2:major
    public int baseNote; // c2: 36 c8: 88
    public int numOctaves; // 3

    private ComposerController controller;

    private List<int> loadedNotes;
    private Dictionary<GameObject, int> soundObjectsDict;


    private float timeToGo;


    void Start()
    {
        soundObjectsDict = new Dictionary<GameObject, int>();


        timeToGo = Time.fixedTime + 1;

        loadedNotes = new List<int>();

        controller = new ComposerController(baseNote, scaleType, numOctaves);
        
        ScanDirectory();

        //StartCoroutine(ChordChangeCoroutine());

    }

    private void FixedUpdate()
    {
        if (Time.fixedTime >= timeToGo)
        {
            ChordChangeCoroutine();
            timeToGo = Time.fixedTime + Random.Range(5,10);
        }
    }
    
    void ChordChangeCoroutine()
    {
        int[] allowedNotes = controller.GetNextChord();

        foreach (int n in allowedNotes)
        {
            if (!ItemAlreadyExists(n))
                LoadFilesWithNote(n);
        }

        int counter = 0;
        foreach(int loadedNote in loadedNotes)
        {
            Debug.Log("NOTE:" + loadedNote);

            foreach (int allowedNote in allowedNotes)
            {
                Debug.Log("ALLOWED NOTE:" + allowedNote);

                if (allowedNote != loadedNote)
                {
                    ToggleSoundObject(loadedNote, false);
                }
                else
                {
                    ToggleSoundObject(loadedNote, true);
                }
            }
        }
        Debug.Log("MATCHES LEN ->>>>>" + counter);

    }

    void ToggleSoundObject(int value, bool newState)
    {
        foreach (KeyValuePair<GameObject, int> pair in soundObjectsDict)
        {
            if (EqualityComparer<int>.Default.Equals(pair.Value, value))
            {
                //pair.Key.SetActive(newState);

                if (newState)
                    pair.Key.tag = "Sounds";
                else
                    pair.Key.tag = "Unusable";
            }
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

            int noteNum = controller.NoteNameToNumber(splittedName[0]);

            if ((noteNum != -1) && controller.IsInAllowedNotes(noteNum))
            {
                loadedNotes.Add(noteNum);

                GameObject tmp = Instantiate(audioPrefab);
                tmp.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(PATH + f.Name.Replace(".wav", ""));
                
                soundObjectsDict.Add(tmp, noteNum);

                print("LOADED: " + f.Name);
            }
        }
    }

    void LoadFilesWithNote(int noteNumber)
    {
        string PATH = "Samples/";

        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/" + PATH);

        FileInfo[] info = dir.GetFiles("*.wav");

        foreach (FileInfo f in info)
        {
            // print("Found: " + f.Name);

            string[] splittedName = f.Name.Split('-');

            int noteNum = controller.NoteNameToNumber(splittedName[0]);

            if (noteNum == noteNumber)
            {
                loadedNotes.Add(noteNum);

                GameObject tmp = Instantiate(audioPrefab);
                tmp.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(PATH + f.Name.Replace(".wav", ""));
                
                soundObjectsDict.Add(tmp, noteNum);


                //print("LOADED ON RUNTIME: " + f.Name);
            }
        }
    }

    private bool ItemAlreadyExists(int target)
    {
        foreach (int item in loadedNotes)
        {
            if (item.Equals(target))
                return true;
        }
        return false;
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

    public int[] GetNextChord()
    {
        scaleManager.SetRandomChordInScale();
        scaleManager.CalcAllowedNotes(true);
        return scaleManager.GetAllowedNotes();
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

    public int[] GetAllowedNotes()
    {
        return scaleManager.GetAllowedNotes();
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

    private int currentChordBaseNoteIndex;


    public ScaleManager(int baseNote, int scaleType, int numOctaves)
    {
        this.baseNote = baseNote;
        this.numOctaves = numOctaves;
        this.scaleType = scaleType;

        majorScaleNotes = new int[7];
        majorScaleChords = new string[7];

        minorScaleNotes = new int[7];
        minorScaleChords = new string[7];
        scaleChords = new string[7];

        InitScales();

        scale = new int[7];
        SetScaleType();

        CalculateScale(this.baseNote, this.scaleType, this.numOctaves);

        currentChord = new int[3];

        chordManager = new ChordManager();
    }

    public int[] GetAllowedNotes()
    {
        return allowedNotes;
    }

    public void CalculateScale(int baseNote, int scaleType, int numOctaves)
    {
        this.baseNote = baseNote;
        this.numOctaves = numOctaves;
        this.scaleType = scaleType;
        
        CalcAllowedNotes(false);
    }

    public void SetRandomChordInScale()
    {
        
        int index = Random.Range(0, 7);
        currentChordBaseNoteIndex = index;

        Debug.Log("NEW CHORD INDEX::::::" + index);
        Debug.Log("NEW CHORD TYPE::::::" + scaleChords[index]);
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
                result[i] = int.Parse(notes[i].Replace("b",""))-1;
            }
        }

        result.CopyTo(currentChord, 0);
    }

    public void CalcAllowedNotes(bool isChordMode)
    {
        int baseVal;
        int[] notes;
        if (!isChordMode)
        {
            baseVal = 7;
            notes = new int[7];
            scale.CopyTo(notes, 0);
        }
        else
        {
            baseVal = currentChord.Length;
            //Debug.Log("BASE VAL:::::" + baseVal);
            notes = new int[baseVal];
            currentChord.CopyTo(notes, 0);
        }
     
        allowedNotesSize = baseVal * this.numOctaves;
        allowedNotes = new int[allowedNotesSize];

        int octaveCounter = 0;
        for (int i = 0; i < allowedNotesSize; i++)
        {
            allowedNotes[i] = baseNote + notes[i % baseVal] + 12 * octaveCounter;
            if (isChordMode)
                allowedNotes[i] += scale[currentChordBaseNoteIndex];


            //Debug.Log("Allowed;;;;" + allowedNotes[i]);
            if (i % baseVal == baseVal-1) octaveCounter++;
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
        chordTypes = new Dictionary<string, string[]>();
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
        if (chordTypes.TryGetValue(name, out string[] result))
        {
            return result;
        }
        return null;
        
    }
}
