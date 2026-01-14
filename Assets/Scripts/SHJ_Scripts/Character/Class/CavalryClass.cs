using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavalryClass : MonoBehaviour
{
    public Dictionary<TerrainType, int> CostTable { get; private set; }

    private TerrainMap terrainMap;

    private void Awake()
    {
        CostTable = new Dictionary<TerrainType, int>();
        RegisterArcherCosts();

        terrainMap = FindObjectOfType<TerrainMap>();
    }

    private void RegisterArcherCosts()
    {
        CostTable[TerrainType.Plain] = 1;
        CostTable[TerrainType.Forest] = 1;
        CostTable[TerrainType.Mountain] = 1;
        CostTable[TerrainType.Road] = 1;
        CostTable[TerrainType.City] = 1;
        CostTable[TerrainType.Water] = 1;
        CostTable[TerrainType.Snow] = 1;
        CostTable[TerrainType.Wall] = 999;
    }
}
