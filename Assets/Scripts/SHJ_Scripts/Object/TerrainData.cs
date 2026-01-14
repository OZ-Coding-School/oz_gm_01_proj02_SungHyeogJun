using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TerrainData", menuName = "SRPG/Terrain")]
public class TerrainData : ScriptableObject
{
    public bool walkable;
    public TerrainType type;
}
