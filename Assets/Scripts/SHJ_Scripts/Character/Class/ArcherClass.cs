using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherClass : MonoBehaviour
{
    public Dictionary<TerrainType, int> MoveCostTable { get; private set; }

    private void Awake()
    {
        MoveCostTable = new Dictionary<TerrainType, int>();

        RegisterArcherCosts();
    }

    private void RegisterArcherCosts()
    {
        MoveCostTable[TerrainType.Plain] = 1;
        MoveCostTable[TerrainType.Forest] = 1;
        MoveCostTable[TerrainType.Mountain] = 1;
        MoveCostTable[TerrainType.Road] = 1;
        MoveCostTable[TerrainType.City] = 1;
        MoveCostTable[TerrainType.Water] = 1;
        MoveCostTable[TerrainType.Snow] = 1;
        MoveCostTable[TerrainType.Wall] = 999;
    }


}
