using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject empty_;
    public int maxMapSize;

    private bool[,] map;
    private bool[,] newMap;

    private GameObject[,] mapObjs;

    private int padding = 1;

    private float margin = 2;
    private float scale = 5;

    private float blockDistance;

    private bool canCont;

    void Start()
    {
        canCont = false;

        blockDistance = scale + margin;

        mapObjs = new GameObject[maxMapSize, maxMapSize];
        newMap = new bool[maxMapSize, maxMapSize];
        map = new bool[maxMapSize, maxMapSize];

        map[5, 5] = true;
        map[6, 5] = true;
        map[7, 5] = true;

        //newMap = map;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            canCont = !canCont;
        
        if (canCont)
        {
            for (int i = padding; i < maxMapSize - padding; i++)
            {
                for (int j = padding; j < maxMapSize - padding; j++)
                {
                    newMap[i, j] = UpdateCellState(i, j);
                    UpdateDisplay(i, j);
                }
            }
            map = newMap;
            
            canCont = false;
        }
    }

    void UpdateDisplay(int x, int y)
    {
        if (newMap[x, y])
        {
            mapObjs[x, y] = Instantiate(cubePrefab, new Vector3(x * blockDistance, 0, y * blockDistance), Quaternion.identity);
        }
        else
        {
            Destroy(mapObjs[x, y]);
            mapObjs[x, y] = empty_;
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
            Debug.Log("Shit  " + x + ", " + y );
            return true;
        }
        else if(!map[x, y] && (neighCounter == 3))
        {
            Debug.Log("Woah  " + x + ", " + y);
            return true;
        }
        else
        {
            Debug.Log("No Shit");
            return false;
        }


    }
}
