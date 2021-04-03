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

    private bool canCont;

    private PresetBank presetBank;

    void Start()
    {
        canCont = false;

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
        /*
        if (Input.GetKeyDown(KeyCode.Space))
            canCont = !canCont;
        */
        
        //if (canCont)
        //{
        
        for (int i = padding; i < maxMapSize - padding; i++)
        {
            for (int j = padding; j < maxMapSize - padding; j++)
            {
                newMap[i, j] = UpdateCellState(i, j);
                
                UpdateDisplay(i, j);
            }
        }
        
        map = (bool[,]) newMap.Clone();
            
        canCont = false;
        
        //}
    }

    void UpdateDisplay(int x, int y)
    {
        Destroy(mapObjs[x, y]);

        if (newMap[x, y])
        {

            mapObjs[x, y] = Instantiate(cubePrefab, new Vector3(x * blockDistance, 0, y * blockDistance), Quaternion.identity);
            
            mapObjs[x, y].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

        }
    }

    bool UpdateCellState(int x, int y)
    {
        bool[] neighs = GetNeighbors(x, y);
        int neighCounter = 0;
        
        foreach (bool item in neighs)
        {
            if (item)
                neighCounter++;
        }

        if (map[x, y] && (neighCounter == 2 || neighCounter == 3))
        {
            //Debug.Log("Shit  for " + x + ", " + y );
            return true;
        }
        else if(!map[x, y] && (neighCounter == 3))
        {
            //Debug.Log("Woah for  " + x + ", " + y);
            return true;
        }
        else
        {
            //Debug.Log("No Shit");
            return false;
        }
    }

    bool[] GetNeighbors(int x, int y)
    {
        bool[] neighs = new bool[8];

        neighs[0] = map[x, y + 1]; // u
        neighs[1] = map[x + 1, y + 1]; // ur
        neighs[2] = map[x + 1, y]; // r 
        neighs[3] = map[x + 1, y - 1]; // dr 
        neighs[4] = map[x, y - 1]; // d 
        neighs[5] = map[x - 1, y - 1]; // dl
        neighs[6] = map[x - 1, y]; // l 
        neighs[7] = map[x - 1, y + 1]; // ul

        return neighs;
    }

    void InitDisplay()
    {
        for (int i = padding; i < maxMapSize - padding; i++)
        {
            for (int j = padding; j < maxMapSize - padding; j++)
            {
                UpdateDisplay(i, j);
            }
        }
    }
    void GenerateRandomBlocks()
    {
        Method3();
        Method2();

    }

    void Method1()
    {
        map[7, 5] = true;
        map[8, 5] = true;
        map[9, 5] = true;
        map[10, 5] = true;
        map[6, 6] = true;
        map[7, 6] = true;
        map[8, 6] = true;
        map[9, 6] = true;
        map[10, 6] = true;
        map[11, 6] = true;
        map[6, 7] = true;
        map[7, 7] = true;
        map[8, 7] = true;
        map[9, 7] = true;
        map[11, 7] = true;
        map[12, 7] = true;
        map[10, 8] = true;
        map[11, 8] = true;
    }

    void Method2()
    {
        //int a = Random.Range(maxMapSize/3, maxMapSize/2);
        int size = numberOfParticles;
        List<(int,int)> already = new List<(int, int)>();
        

        for (int i=0; i< size; i++)
        {
            int x = Random.Range(padding + 1, maxMapSize/2);
            int y = Random.Range(padding + 1, maxMapSize/2);
            while (_AlreadyExists(already, (x,y)))
            {
                x = Random.Range(padding + 1, maxMapSize / 2);
                y = Random.Range(padding + 1, maxMapSize / 2);
            }
            
            already.Add((x, y));

            //Debug.Log("X, Y: " + x + "  " + y);
            map[x, y] = true;
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
        for (int i = 0; i < 20; i++)
        {
            int k = Random.Range(0, 50);
            if (k%5 == 0)
            {
                float m = Random.Range(0f, 1f);
                if (m < presetBank.blinkerPreset.probability)
                {
                    int randX = Random.Range(padding + presetBank.blinkerPreset.radius, maxMapSize - padding - presetBank.blinkerPreset.radius);
                    int randY = Random.Range(padding + presetBank.blinkerPreset.radius, maxMapSize - padding - presetBank.blinkerPreset.radius);
                    InsertBlinker(randX, randY);
                }
            }

            else if (k % 3 ==0)
            {
                float m = Random.Range(0f, 1f);
                if (m < presetBank.toadPreset.probability)
                {
                    int randX = Random.Range(padding + presetBank.toadPreset.radius, maxMapSize - padding - presetBank.toadPreset.radius);
                    int randY = Random.Range(padding + presetBank.toadPreset.radius, maxMapSize - padding - presetBank.toadPreset.radius);
                    InsertToad(randX, randY);
                }
            }
                
        }
    }

    void InsertBlinker(int rx, int ry)
    {
        for (int x = 0; x < presetBank.blinkerPreset.radius; x++)
        {
            for (int y = 0; y < presetBank.blinkerPreset.radius; y++)
            {
                map[rx + x, ry + y] = presetBank.blinkerPreset.map[x, y];
            }
        }
    }

    void InsertToad(int rx, int ry)
    {
        for (int x = 0; x < presetBank.toadPreset.radius; x++)
        {
            for (int y = 0; y < presetBank.toadPreset.radius; y++)
            {
                map[rx + x, ry + y] = presetBank.toadPreset.map[x, y];
            }
        }
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