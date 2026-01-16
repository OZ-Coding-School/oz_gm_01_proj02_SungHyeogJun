using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IUnitClass
{
    UnitType unitType { get; }
    int movementRange { get; }
    Dictionary<TerrainType, int> CostTable { get; }
    TileBase[] distanceTiles { get; }

    void ShowDummyRange(Vector3Int startCell);
    void ClearHighlight();
}
