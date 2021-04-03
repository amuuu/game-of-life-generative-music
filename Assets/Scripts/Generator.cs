using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject cubePrefab;
    public int maxMapSize; // 50
    public int maxMapHeight; // 10

    public int padding; // 3
    public float cubeMargin; // 3
    public int numberOfParticles; //300


    private bool[,,] map;
    private bool[,,] newMap;

    private GameObject[,,] mapObjs;


    private float scale = 5;

    private float blockDistance;

    private PresetBank presetBank;

    void Start()
    {
        blockDistance = scale + cubeMargin;

        presetBank = new PresetBank();
        
        mapObjs = new GameObject[maxMapSize, maxMapHeight, maxMapSize];
        newMap = new bool[maxMapSize, maxMapHeight, maxMapSize];
        map = new bool[maxMapSize, maxMapHeight, maxMapSize];

        GenerateRandomBlocks();

        newMap = (bool[,,]) map.Clone();

        InitDisplay();

    }

    void Update()
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
        
        map = (bool[,,]) newMap.Clone();
    }

    void UpdateDisplay(int x, int y, int z)
    {
        Debug.Log("COORD: " + x + ", " + y + ", " + z);
        Destroy(mapObjs[x, y, z]);

        if (newMap[x, y, z])
        {

            mapObjs[x, y, z] = Instantiate(cubePrefab, new Vector3(x * blockDistance, y * blockDistance, z * blockDistance), Quaternion.identity);
            
            //mapObjs[x, y, z].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

        }
    }

    bool UpdateCellState(int x, int y, int z)
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

    bool[] GetNeighbors(int x, int y, int z)
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

    void InitDisplay()
    {
        for (int i = padding; i < maxMapSize - padding; i++)
        {
            for (int j = 0; i < maxMapHeight; j++)
            {
                for (int k = padding; k < maxMapSize - padding; k++)
                {
                    UpdateDisplay(i, j, k);
                }
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
        List<(int, int, int)> already = new List<(int, int, int)>();

        int x, y, z;
        for (int i=0; i< size; i++)
        {
            do {
                x = Random.Range(padding + 1, maxMapSize / 2);
                y = Random.Range(0, maxMapHeight-1);
                z = Random.Range(padding + 1, maxMapSize / 2);
            } while (_AlreadyExists(already, (x, y, z))) ;

            already.Add((x, y, z));

            //Debug.Log("X, Z: " + x + "  " + z);
            map[x, y, z] = true;
        }
        newMap = (bool[,,]) map.Clone();

    }

    bool _AlreadyExists(List<(int, int, int)> list, (int, int, int) target)
    {
        foreach ((int, int, int) item in list)
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
                (int, int, int) coords = GenerateRandomCoordinate();
                InsertPreset((Preset) presetBank.blinkerPreset, coords.Item1, coords.Item2, coords.Item3);
            }

            if (prob > presetBank.toadPreset.probability)
            {
                (int, int, int) coords = GenerateRandomCoordinate();
                InsertPreset((Preset) presetBank.toadPreset, coords.Item1, coords.Item2, coords.Item3);

            }
                
        }
    }

    void InsertPreset(Preset presetObject, int rx, int ry, int rz)
    {
        
        for (int x = 0; x < presetObject.radius; x++)
        {
            for (int z = 0; z < presetObject.radius; z++)
            {
                map[rx + x, ry, rz + z] = presetObject.map[x, z];
            }
        }
    }

    (int, int, int) GenerateRandomCoordinate()
    {
        int randX = Random.Range(padding + presetBank.blinkerPreset.radius, maxMapSize - padding - presetBank.blinkerPreset.radius);
        int randY = Random.Range(0, maxMapHeight - 1);
        int randZ = Random.Range(padding + presetBank.blinkerPreset.radius, maxMapSize - padding - presetBank.blinkerPreset.radius);

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