using System.Collections.Generic;
using UnityEngine;

public interface IUnitClass
{
    UnitType unitType { get; }
    int movementRange { get; }
    Dictionary<TerrainType, int> movementCost { get; }

    /// <summary>
    /// 중심점을 기준으로 이동 가능한 범위 계산
    /// InfantryClass에서는 BFS 구현
    /// </summary>
    HashSet<Vector3Int> ShowMoveRange(Vector3Int center);

    /// <summary>
    /// 계산된 이동 범위를 Tilemap에 색으로 표시
    /// InfantryClass에서는 HighlightMoveRange 구현
    /// </summary>

    HashSet<Vector3Int> ShowAttackRange(Vector3Int center);

}

public interface IWalkable
{
    bool IsWalkable { get; }
}

