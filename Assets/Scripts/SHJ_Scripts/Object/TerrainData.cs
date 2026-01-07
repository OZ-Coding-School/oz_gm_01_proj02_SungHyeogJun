using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class TerrainData : ScriptableObject
{
    public bool walkable;
    public int moveCost;
}
