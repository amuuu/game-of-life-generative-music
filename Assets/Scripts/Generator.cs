using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject cubePrefab;
    public int maxMapSize; // 50
    public int padding; // 3
    public float cubeMargin; // 3
    public int numberOfParticles; //300


    private bool[,] map;
    private bool[,] newMap;

    private GameObject[,] mapObjs;


    private float scale = 5;

    private float blockDistance;

    private PresetBank presetBank;

    void Start()
    {
        blockDistance = scale + cubeMargin;

        presetBank = new PresetBank();
        
        mapObjs = new GameObject[maxMapSize, maxMapSize];
        newMap = new bool[maxMapSize, maxMapSize];
        map = new bool[maxMapSize, maxMapSize];

        GenerateRandomBlocks();

        newMap = (bool[,]) map.Clone();

        InitDisplay();

    }

    void Update()
    {
        for (int i = padding; i < maxMapSize - padding; i++)
        {
            for (int k = padding; k < maxMapSize - padding; k++)
            {
                newMap[i, k] = UpdateCellState(i, k);
                
                UpdateDisplay(i, k);
            }
        }
        
        map = (bool[,]) newMap.Clone();
    }

    void UpdateDisplay(int x, int z)
    {
        Destroy(mapObjs[x, z]);

        if (newMap[x, z])
        {

            mapObjs[x, z] = Instantiate(cubePrefab, new Vector3(x * blockDistance, 0, z * blockDistance), Quaternion.identity);
            
            //mapObjs[x, z].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

        }
    }

    bool UpdateCellState(int x, int z)
    {
        bool[] neighs = GetNeighbors(x, z);
        int neighCounter = 0;
        
        foreach (bool item in neighs)
        {
            if (item)
                neighCounter++;
        }

        if (map[x, z] && (neighCounter == 2 || neighCounter == 3))
        {
            return true;
        }
        else if(!map[x, z] && (neighCounter == 3))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool[] GetNeighbors(int x, int z)
    {
        bool[] neighs = new bool[8];

        neighs[0] = map[x, z + 1]; // u
        neighs[1] = map[x + 1, z + 1]; // ur
        neighs[2] = map[x + 1, z]; // r 
        neighs[3] = map[x + 1, z - 1]; // dr 
        neighs[4] = map[x, z - 1]; // d 
        neighs[5] = map[x - 1, z - 1]; // dl
        neighs[6] = map[x - 1, z]; // l 
        neighs[7] = map[x - 1, z + 1]; // ul

        return neighs;
    }

    void InitDisplay()
    {
        for (int i = padding; i < maxMapSize - padding; i++)
        {
            for (int k = padding; k < maxMapSize - padding; k++)
            {
                UpdateDisplay(i, k);
            }
        }
    }
    void GenerateRandomBlocks()
    {
        Method3();
        //Method2();

    }

    void Method2()
    {
        //int a = Random.Range(maxMapSize/3, maxMapSize/2);
        int size = numberOfParticles;
        List<(int,int)> already = new List<(int, int)>();
        

        for (int i=0; i< size; i++)
        {
            int x = Random.Range(padding + 1, maxMapSize/2);
            int z = Random.Range(padding + 1, maxMapSize/2);
            while (_AlreadyExists(already, (x,z)))
            {
                x = Random.Range(padding + 1, maxMapSize / 2);
                z = Random.Range(padding + 1, maxMapSize / 2);
            }
            
            already.Add((x, z));

            //Debug.Log("X, Z: " + x + "  " + z);
            map[x, z] = true;
        }
        newMap = (bool[,]) map.Clone();

    }

    bool _AlreadyExists(List<(int,int)> list, (int,int)target)
    {
        foreach ((int,int) item in list)
        {
            if (item.Equals(target))
                return true;
        }
        return false;
    }

    void Method3()
    {
        float prob = 0f;

        for (int i = 0; i < 20; i++)
        {
            prob = Random.Range(0f, 1f);

            if (prob > presetBank.blinkerPreset.probability)
            {
                (int, int) coords = GenerateRandomCoordinate();
                InsertPreset((Preset) presetBank.blinkerPreset, coords.Item1, coords.Item2);
            }

            if (prob > presetBank.toadPreset.probability)
            {
                (int, int) coords = GenerateRandomCoordinate();
                InsertPreset((Preset) presetBank.toadPreset, coords.Item1, coords.Item2);

            }
                
        }
    }

    void InsertPreset(Preset presetObject, int rx, int rz)
    {
        
        for (int x = 0; x < presetObject.radius; x++)
        {
            for (int z = 0; z < presetObject.radius; z++)
            {
                map[rx + x, rz + z] = presetObject.map[x, z];
            }
        }
    }

    (int, int) GenerateRandomCoordinate()
    {
        int randX = Random.Range(padding + presetBank.blinkerPreset.radius, maxMapSize - padding - presetBank.blinkerPreset.radius);
        int randZ = Random.Range(padding + presetBank.blinkerPreset.radius, maxMapSize - padding - presetBank.blinkerPreset.radius);

        return (randX, randZ);
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


    public PresetBank()
    {
        blinkerPreset = new Blinker();
        toadPreset = new Toad();
    }
}
public class Blinker : Preset
{
    public Blinker()
    {
        probability = 0.3f;
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