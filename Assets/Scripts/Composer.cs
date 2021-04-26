using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct Settings
{
    public int scaleType; // 1:minor / 2:major
    public int baseNote; // c2: 36 c8: 88
    public int numOctaves; // 3

}

public struct SoundObject
{
    public GameObject obj;
    public int noteNumber;
    public bool isEnabled;

    public void SetActive(bool b)
    {
        isEnabled = b;
    }
}

public class Composer : MonoBehaviour
{

    public GameObject audioPrefab;

    public int scaleType; // 1:minor / 2:major
    public int baseNote; // c2: 36 c8: 88
    public int numOctaves; // 3
    public bool isChordMode = false;
    public bool fileLoadStats = false;

    public bool randomTimeMode = false;
    [Range(5.0f, 9.0f)] public float minDelay = 5;
    [Range(10.0f, 15.0f)] public float maxDelay = 10;
    private float delay;

    private float time;

    private string samplesRootPath = "Samples/";

    private List<SoundObject> sounds;

    ///////////////
    private ComposerController controller;
    private DirectoryInfo dir;
    FileInfo[] info;
    private Note[] nextChord;

    void Start()
    {

        dir = new DirectoryInfo("Assets/Resources/" + samplesRootPath);
        info = dir.GetFiles("*.wav");

        time = Time.fixedTime;
        if (!randomTimeMode) delay = minDelay;
        

        controller = new ComposerController(scaleType, baseNote, numOctaves);
        sounds = new List<SoundObject>();

        // scan the directory and load all the sounds in the scale
        ScanDirectory();
    }

    void FixedUpdate()
    {
        // change the chord every couple of seconds
        if (isChordMode)
        {
            if (Time.fixedTime >= time)
            //if(Input.GetKeyDown(KeyCode.L))
            {
                nextChord = controller.scale.GetRandomChordInScale();
                ManageSamplesBasedOnNextChord();

                if(randomTimeMode) delay = UnityEngine.Random.Range(minDelay, maxDelay);
                time = Time.fixedTime + delay;
            }
        }
    }


    void ManageSamplesBasedOnNextChord()
    {
        // if the note that's inside the chord isn't loaded before, load the corresponding samples
        // if a note is loaded but it's not in the chord, diactive the corresponding samples
        bool exists;

        foreach (Note n in nextChord)
        {
            exists = false;
            foreach (SoundObject s in sounds)
            {
                if (s.noteNumber == n.number)
                { 
                    exists = true;
                    if (!s.obj.activeSelf)
                    {
                        s.obj.SetActive(true);
                    }
                }
            }

            if (!exists)
            {
                // if note is in the chord but not on the loaded sounds
                LoadFilesWithNote(n.number);
            }
        }

        // if note isn't in the chord but has been loaded before
        int counter = 0;
        foreach (SoundObject s in sounds)
        {
            if (!Utility.ChordContainsNote(nextChord, s.noteNumber))
            {
                s.obj.SetActive(false);
            }
            else counter++;
        }
    }

    void ScanDirectory()
    {
        foreach (FileInfo f in info)
        {
            // print("Found: " + f.Name);

            string[] splittedName = f.Name.Split('-');

            int noteNum = Utility.NoteNameToNumber(splittedName[0].ToLower());

            if ((noteNum != -1) && controller.scale.IsInScaleNotes(noteNum))
            {
                GameObject tmp = Instantiate(audioPrefab);
                tmp.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(samplesRootPath + f.Name.Replace(".wav", ""));
                
                sounds.Add(new SoundObject { isEnabled = true, noteNumber = noteNum, obj = tmp });

                if (fileLoadStats)
                    print("LOADED: " + f.Name);
            }
        }
    }

    void LoadFilesWithNote(int noteNumber)
    {
        dir = new DirectoryInfo("Assets/Resources/" + samplesRootPath);
        info = dir.GetFiles("*.wav");
        foreach (FileInfo f in info)
        {
            string[] splittedName = f.Name.Split('-');

            int noteNum = Utility.NoteNameToNumber(splittedName[0].ToLower());
            if (noteNum == noteNumber)
            {

                GameObject tmp = Instantiate(audioPrefab);
                tmp.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(samplesRootPath + f.Name.Replace(".wav", ""));

                sounds.Add(new SoundObject { isEnabled = true, noteNumber = noteNum, obj = tmp });
                
                if (fileLoadStats)
                    print("LOADED ON RUNTIME: " + f.Name);
            }
        }
    }
}

class ComposerController
{

    public Scale scale;
    private Settings settings;

    public ComposerController(int scaleType, int baseNote, int numOctaves)
    {
        settings.scaleType = scaleType;
        settings.baseNote = baseNote;
        settings.numOctaves = numOctaves;

        scale = new Scale(scaleType, baseNote, numOctaves);
    }
}

class Scale
{
    Note[] notes;
    Chord[] chords;
    Settings settings;

    public Scale(int type, int baseNote, int numOctaves)
    {
        settings.scaleType = type;
        settings.baseNote = baseNote;
        settings.numOctaves = numOctaves;

        CalculateScaleNotes();
        CalculateScaleChords();
    }

    public bool IsInScaleNotes(int noteNumber)
    {
        foreach (Note n in notes)
        {
            if ((noteNumber % 12) == n.number)
                return true;
        }

        return false;
    }

    public Note[] GetAllowedNotesInScale()
    {
        return Utility.ExpandNotesIntoOctaves(notes, settings.baseNote, settings.numOctaves);
    }

    public Note[] GetRandomChordInScale()
    {
        int index = 0;
        float f = UnityEngine.Random.Range(0f, 1f);
        if (f > 0.75f) index = 0;
        else if (f < 0.75 && f > 0.50) index = 3;
        else if (f < 0.50 && f > 0.35) index = 4;
        else
        {
            index = UnityEngine.Random.Range(3, notes.Length);
            if (index == 3) index = 1;
            if (index == 4) index = 2;

        }

        return chords[index].GetChordNotes(settings.baseNote + index, settings.numOctaves);
    }

    public void CalculateScaleNotes()
    {
        notes = Utility.GetScaleNotes(settings.scaleType);
    }

    public void CalculateScaleChords()
    {
        chords = Utility.GetScaleChords(settings.scaleType, notes);
    }
}

class Chord
{
    Note[] notes;

    public Chord(string type, int baseNoteIndex, Note[] scale)
    {
        switch(type)
        {
            case "M": notes = new Note[3] { scale[(baseNoteIndex) % 7], scale[(baseNoteIndex + 2) % 7], scale[(baseNoteIndex + 4 ) % 7] }; break;
            case "m": notes = new Note[3] { scale[(baseNoteIndex) % 7], new Note((scale[(baseNoteIndex + 2) % 7].number - 1) % 12), scale[(baseNoteIndex + 4) % 7] }; break;
            case "dim": notes = new Note[3] { scale[(baseNoteIndex) % 7], new Note((scale[(baseNoteIndex + 2) % 7].number - 1) % 12), new Note((scale[(baseNoteIndex + 4) % 7].number - 1) % 12) }; break;
            default: break;
        }
    }

    public Note[] GetNotes()
    {
        return notes;
    }

    public bool IsInChordNotes(int noteIndex)
    {
        foreach (Note n in notes)
        {
            if (noteIndex == n.number)
                return true;
        }
         
        return false;
    }

    public Note[] GetChordNotes(int baseNote, int numOctaves)
    {
        return Utility.ExpandNotesIntoOctaves(notes, baseNote, numOctaves);
    }
}

class Note
{
    public int number;

    public Note(int n)
    {
        number = n;
    }

}

static class Utility
{
    public static Note[] ExpandNotesIntoOctaves(Note[] notes, int baseNote, int numOctaves)
    {
        int size = notes.Length * numOctaves;
        Note[] result = new Note[size];
        for (int i = 0; i < size; i++)
        {
            result[i] = new Note(baseNote + notes[i % notes.Length].number + 12 * ((i / numOctaves)));
        }

        return result;
    }

    public static int NoteNameToNumber(string name)
    {
        int result = 0;
        
        int length = name.Length;
        int octave = int.Parse(name[length - 1].ToString());

        if (length == 3) // aka it's a sharp note (Because the name will be like As4 with the length of 3 chars.)
        {
            result = +1;
        }

        switch (name[0])
        {
            case 'c': result += 0; break;
            case 'd': result += 2; break;
            case 'e': result += 4; break;
            case 'f': result += 5; break;
            case 'g': result += 7; break;
            case 'a': result += 9; break;
            case 'b': result += 11; break;
        }

        result += 12 * octave;

        return result;
    }

    public static Note[] GetScaleNotes(int type)
    {
        Note[] notes = new Note[7];
        if (type == 1)
        {
            notes[0] = new Note(0);
            notes[1] = new Note(2);
            notes[2] = new Note(3);
            notes[3] = new Note(5);
            notes[4] = new Note(7);
            notes[5] = new Note(8);
            notes[6] = new Note(10);
        }
        else if (type == 2)
        {
            notes[0] = new Note(0);
            notes[1] = new Note(2);
            notes[2] = new Note(4);
            notes[3] = new Note(5);
            notes[4] = new Note(7);
            notes[5] = new Note(9);
            notes[6] = new Note(11);
        }
        return notes;
    }

    public static Chord[] GetScaleChords(int type, Note[] notes)
    {
        Chord[] chords = new Chord[7];
        if (type == 1)
        {
            chords[0] = new Chord("m", 0, notes);
            chords[1] = new Chord("dim", 1, notes);
            chords[2] = new Chord("M", 2, notes);
            chords[3] = new Chord("m", 3, notes);
            chords[4] = new Chord("m", 4, notes);
            chords[5] = new Chord("M", 5, notes);
            chords[6] = new Chord("M", 6, notes);
        }
        else if (type == 2)
        {
            chords[0] = new Chord("M", 0, notes);
            chords[1] = new Chord("m", 1, notes);
            chords[2] = new Chord("m", 2, notes);
            chords[3] = new Chord("M", 3, notes);
            chords[4] = new Chord("M", 4, notes);
            chords[5] = new Chord("m", 5, notes);
            chords[6] = new Chord("dim", 6, notes);
        }
        
        return chords;
    }

    public static bool ChordContainsNote(Note[]chord, int number)
    {
        foreach(Note n in chord)
        {
            if (n.number == number)
            {
                return true;
            }
        }
        return false;
    }
}