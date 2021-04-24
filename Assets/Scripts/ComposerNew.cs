using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ComposerNew : MonoBehaviour
{

    public GameObject audioPrefab;

    public int scaleType; // 1:minor / 2:major
    public int baseNote; // c2: 36 c8: 88
    public int numOctaves; // 3
    public bool isChordMode;

    private string samplesRootPath = "Samples/";

    
    private ComposerController controller;


    void Start()
    {
        // scan the directory and load all the sounds in the scale
        ScanDirectory();
    }

    void FixedUpdate()
    {
        // if isChordMode
        // change the chord every couple of seconds
        // if the note that's inside the chord isn't loaded before, load the corresponding samples
        // if a note is loaded but it's not in the chord, diactive the corresponding samples
    }

    void ScanDirectory()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/" + samplesRootPath);

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
}

