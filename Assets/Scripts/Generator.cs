using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject cubePrefab2;

    public Camera mainCamera;

    public float generationCycleDelay; // 0.5

    public int maxMapSize; // 50
    public int maxMapHeight; // 10

    public int padding; // 3
    public float cubeMargin; // 3

    public int randomParticlesNumber; //300
    public int presetStructuresNumber; // 50

    public int eyeSightRadius;

    private float scale = 5;

    private bool[,,] map;
    private bool[,,] newMap;
    private GameObject[,,] mapObjs;
    private bool[,,] objExistsOnMap;

    private float blockDistance;
    private PresetBank presetBank;
    private float timeToGo;
    private float tmpXCoord, tmpYCoord, tmpZCoord;

    void Start()
    {
        timeToGo = Time.fixedTime + generationCycleDelay;

        blockDistance = scale + cubeMargin;

        presetBank = new PresetBank();
        
        mapObjs = new GameObject[maxMapSize, maxMapHeight, maxMapSize];
        newMap = new bool[maxMapSize, maxMapHeight, maxMapSize];
        map = new bool[maxMapSize, maxMapHeight, maxMapSize];
        objExistsOnMap = new bool[maxMapSize, maxMapHeight, maxMapSize];


        GenerateRandomBlocks();

        newMap = (bool[,,]) map.Clone();

        InitDisplay();

    }

    void FixedUpdate()
    {
        if (Time.fixedTime >= timeToGo)
        {
            AdvanceGeneration();
            timeToGo = Time.fixedTime + generationCycleDelay;
        }
    }

    void AdvanceGeneration()
    {
        for (int i = padding; i < maxMapSize - padding; i++)
        {
            for (int j = 0; j < maxMapHeight; j++)
            {
                for (int k = padding; k < maxMapSize - padding; k++)
                {
                    newMap[i, j, k] = UpdateCellState(i, j, k);

                    UpdateDisplay(i, j, k);
                }
            }
        }

        map = (bool[,,])newMap.Clone();
    }

    private void UpdateDisplay(int x, int y, int z)
    {
        if (objExistsOnMap[x, y, z])
        {
            Destroy(mapObjs[x, y, z]);
            objExistsOnMap[x, y, z] = false;
        }

        if (newMap[x, y, z])
        {
            tmpXCoord = x * blockDistance;
            tmpYCoord = y * (blockDistance + 1);
            tmpZCoord = z * blockDistance;
            
            if (IsInFOV(tmpXCoord, tmpYCoord, tmpZCoord) && IsInRadius(eyeSightRadius, tmpXCoord, tmpYCoord, tmpZCoord))
            {
                if (y % 2 == 0)
                    mapObjs[x, y, z] = Instantiate(cubePrefab, new Vector3(tmpXCoord, tmpYCoord, tmpZCoord), Quaternion.identity);
                else
                    mapObjs[x, y, z] = Instantiate(cubePrefab2, new Vector3(tmpXCoord, tmpYCoord, tmpZCoord), Quaternion.identity);
                
                objExistsOnMap[x, y, z] = true;
            }
            //mapObjs[x, y, z].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

        }
    }

    private bool IsInFOV(float x, float y, float z)
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(new Vector3(x,y,z));
        if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
            return true;

        return false;
    }

    private bool IsInRadius(float radius, float x, float y, float z)
    {
        float distance = Vector3.Distance(mainCamera.transform.position, new Vector3(x, y, z));

        if (Math.Floor(distance) < radius)
            return true;
        
        return false;
    }

    private bool UpdateCellState(int x, int y, int z)
    {
        bool[] neighs = GetNeighbors(x, y, z);
        int neighCounter = 0;
        
        foreach (bool item in neighs)
        {
            if (item)
                neighCounter++;
        }

        if (map[x, y, z] && (neighCounter == 2 || neighCounter == 3))
        {
            return true;
        }
        else if(!map[x, y, z] && (neighCounter == 3))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool[] GetNeighbors(int x, int y, int z)
    {
        bool[] neighs = new bool[8];

        neighs[0] = map[     x,   y,   z + 1 ]; // u
        neighs[1] = map[ x + 1,   y,   z + 1 ]; // ur
        neighs[2] = map[ x + 1,   y,   z     ]; // r 
        neighs[3] = map[ x + 1,   y,   z - 1 ]; // dr 
        neighs[4] = map[     x,   y,   z - 1 ]; // d 
        neighs[5] = map[ x - 1,   y,   z - 1 ]; // dl
        neighs[6] = map[ x - 1,   y,   z     ]; // l 
        neighs[7] = map[ x - 1,   y,   z + 1 ]; // ul

        return neighs;
    }

    private void InitDisplay()
    {
        for (int i = padding; i < maxMapSize - padding; i++)
        {
            for (int j = 0; j < maxMapHeight; j++)
            {
                for (int k = padding; k < maxMapSize - padding; k++)
                {
                    UpdateDisplay(i, j, k);
                }
            }
        }
    }

    private void GenerateRandomBlocks()
    {
        GenerateBasedOnPresets();
        GenerateTotallyRandom();

    }

    private void GenerateTotallyRandom()
    {
        //int a = Random.Range(maxMapSize/3, maxMapSize/2);
        int size = randomParticlesNumber;
        List<(int, int, int)> already = new List<(int, int, int)>();

        int x, y, z;
        for (int i=0; i < size; i++)
        {
            do {
                x = UnityEngine.Random.Range(padding + 1, maxMapSize / 2);
                y = UnityEngine.Random.Range(0, maxMapHeight-1);
                z = UnityEngine.Random.Range(padding + 1, maxMapSize / 2);
            } while (_AlreadyExists(already, (x, y, z))) ;

            already.Add((x, y, z));

            //Debug.Log("X, Z: " + x + "  " + z);
            map[x, y, z] = true;
        }
        newMap = (bool[,,]) map.Clone();

    }

    private bool _AlreadyExists(List<(int, int, int)> list, (int, int, int) target)
    {
        foreach ((int, int, int) item in list)
        {
            if (item.Equals(target))
                return true;
        }
        return false;
    }

    private void GenerateBasedOnPresets()
    {
        float prob = 0f;
        
        for (int i = 0; i < presetStructuresNumber; i++)
        {
            prob = UnityEngine.Random.Range(0f, 1f);

            if (prob > presetBank.pulsarPreset.probability)
            {
                (int, int, int) coords = GenerateRandomCoordinate(presetBank.pulsarPreset.radius);
                InsertPreset((Preset) presetBank.pulsarPreset, coords.Item1, coords.Item2, coords.Item3);
            }

            else if (prob > presetBank.pentaPreset.probability)
            {
                (int, int, int) coords = GenerateRandomCoordinate(presetBank.pentaPreset.radius);
                InsertPreset((Preset) presetBank.pentaPreset, coords.Item1, coords.Item2, coords.Item3);
            }

            else if (prob > presetBank.blinkerPreset.probability)
            {
                (int, int, int) coords = GenerateRandomCoordinate(presetBank.blinkerPreset.radius);
                InsertPreset((Preset) presetBank.blinkerPreset, coords.Item1, coords.Item2, coords.Item3);
            }

            else if(prob > presetBank.toadPreset.probability)
            {
                (int, int, int) coords = GenerateRandomCoordinate(presetBank.toadPreset.radius);
                InsertPreset((Preset) presetBank.toadPreset, coords.Item1, coords.Item2, coords.Item3);
            }

            else if(prob > presetBank.beaconPreset.probability)
            {
                (int, int, int) coords = GenerateRandomCoordinate(presetBank.beaconPreset.radius);
                InsertPreset((Preset) presetBank.beaconPreset, coords.Item1, coords.Item2, coords.Item3);
            }

            
        }
    }

    private void InsertPreset(Preset presetObject, int rx, int ry, int rz)
    {
        
        for (int x = 0; x < presetObject.radius; x++)
        {
            for (int z = 0; z < presetObject.radius; z++)
            {
                map[rx + x, ry, rz + z] = presetObject.map[x, z];
            }
        }
    }

    private (int, int, int) GenerateRandomCoordinate(int radius)
    {
        int randX = UnityEngine.Random.Range(padding + radius, maxMapSize - padding - radius);
        int randY = UnityEngine.Random.Range(0, maxMapHeight - 1);
        int randZ = UnityEngine.Random.Range(padding + radius, maxMapSize - padding - radius);

        return (randX, randY, randZ);
    }
}

public class Preset
{
    public bool[,] map;
    public float probability;
    public int radius;
}

public class PresetBank
{
    public Blinker blinkerPreset;
    public Toad toadPreset;
    public Beacon beaconPreset;
    public Pulsar pulsarPreset;
    public Pentadecathlon pentaPreset;

    public PresetBank()
    {
        blinkerPreset = new Blinker();
        toadPreset = new Toad();
        beaconPreset = new Beacon();
        pulsarPreset = new Pulsar();
        pentaPreset = new Pentadecathlon();

    }
}
public class Blinker : Preset
{
    public Blinker()
    {
        probability = 0.4f;
        radius = 3;

        map = new bool[radius, radius];

        map[1, 0] = true;
        map[1, 1] = true;
        map[1, 2] = true;
    }
}

public class Toad : Preset
{
    public Toad()
    {
        probability = 0.5f;
        radius = 4;

        map = new bool[radius, radius];

        map[1, 0] = true;
        map[1, 1] = true;
        map[1, 2] = true;
        map[2, 1] = true;
        map[2, 2] = true;
        map[2, 3] = true;
    }
}

public class Beacon : Preset
{
    public Beacon()
    {
        probability = 0.5f;
        radius = 4;

        map = new bool[radius, radius];

        map[2, 0] = true;
        map[3, 0] = true;
        map[3, 1] = true;
        map[0, 2] = true;
        map[0, 3] = true;
        map[1, 3] = true;
    }
}

public class Pulsar : Preset
{
    public Pulsar()
    {
        probability = 0.7f;
        radius = 13;

        map = new bool[radius, radius];

        map[2, 0] = true;
        map[3, 0] = true;
        map[4, 0] = true;
        map[8, 0] = true;
        map[9, 0] = true;
        map[10, 0] = true;
        
        map[0, 2] = true;
        map[5, 2] = true;
        map[12, 2] = true;
        map[0, 3] = true;
        map[5, 3] = true;
        map[12, 3] = true;
        map[0, 4] = true;
        map[5, 4] = true;
        map[12, 4] = true;

        map[2, 5] = true;
        map[3, 5] = true;
        map[4, 5] = true;
        map[8, 5] = true;
        map[9, 5] = true;
        map[10, 5] = true;

        map[2, 7] = true;
        map[3, 7] = true;
        map[4, 7] = true;
        map[8, 7] = true;
        map[9, 7] = true;
        map[10, 7] = true;

        map[0, 8] = true;
        map[5, 8] = true;
        map[12, 8] = true;
        map[0, 9] = true;
        map[5, 9] = true;
        map[12, 9] = true;
        map[0, 10] = true;
        map[5, 10] = true;
        map[12, 10] = true;

        map[2, 12] = true;
        map[3, 12] = true;
        map[4, 12] = true;
        map[8, 12] = true;
        map[9, 12] = true;
        map[10, 12] = true;


    }
}

public class Pentadecathlon : Preset
{
    public Pentadecathlon()
    {
        probability = 0.6f;
        radius = 12;

        map = new bool[radius, radius];

        map[1, 0] = true;
        map[2, 0] = true;
        map[3, 0] = true;
        
        map[0, 1] = true;
        map[4, 1] = true;
        map[0, 2] = true;
        map[4, 2] = true;

        map[1, 3] = true;
        map[2, 3] = true;
        map[3, 3] = true;


        map[1, 8] = true;
        map[2, 8] = true;
        map[3, 8] = true;

        map[0, 9] = true;
        map[4, 9] = true;
        map[0, 10] = true;
        map[4, 10] = true;

        map[1, 11] = true;
        map[2, 11] = true;
        map[3, 11] = true;

    }
}