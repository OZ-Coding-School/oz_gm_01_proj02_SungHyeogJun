using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitClass
{
    UnitType unitType { get; }
    int movementRange { get; }
    Dictionary<TerrainType, int> CostTable { get; }

    SoldierResult GetCurrentResult();
    SoldierResult RequestResult(CharacterManager mgr);

    void ShowDummyRange(Vector3Int startCell);
    void ClearHighlight();
}
