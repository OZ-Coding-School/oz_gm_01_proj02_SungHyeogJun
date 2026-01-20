using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InfantryClass : MonoBehaviour, IUnitClass
{
    public UnitType unitType => UnitType.Infantry;
    public int movementRange => 4;
    public Tilemap moveTilemap;


    // BFS 계산 결과 저장
    private HashSet<Vector3Int> moveRangePositions = new HashSet<Vector3Int>();
   
    
    public Dictionary<TerrainType, int> movementCost { get; } =
        new Dictionary<TerrainType, int>()
        {
            { TerrainType.Plain, 1 },
            { TerrainType.Forest, 2 },
            { TerrainType.Mountain, 3 },
            { TerrainType.Road, 1 },
            { TerrainType.City, 1 },
            { TerrainType.Snow, 2 },
            { TerrainType.Water, 99 },
            { TerrainType.Wall, 99 }
        };

    public HashSet<Vector3Int> ShowMoveRange(Vector3Int center)
    {
        moveRangePositions.Clear();

        TerrainMap terrainMap = CharacterManager.Instance.GetTerrainMap();
        if (terrainMap == null)
            return moveRangePositions;

        Queue<(Vector3Int pos, int dist)> open = new Queue<(Vector3Int pos, int dist)>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        open.Enqueue((center, 0));
        visited.Add(center);

        Vector3Int[] dirs = { Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down };

        while (open.Count > 0)
        {
            var (current, dist) = open.Dequeue();

            if (dist >= movementRange)
                continue;

            foreach (var d in dirs)
            {
                Vector3Int next = current + d;

                if (visited.Contains(next))
                    continue;

                TerrainData terrain = terrainMap.GetTerrain(next);
                if (terrain != null && terrain.walkable == false)
                    continue;

                visited.Add(next);
                open.Enqueue((next, dist + 1));

                moveRangePositions.Add(next);
            }
        }

        return moveRangePositions; // ← 계산 결과 반환
    }

    public HashSet<Vector3Int> ShowAttackRange(Vector3Int center)
    {
        HashSet<Vector3Int> attackPositions = new HashSet<Vector3Int>();

        // 보병 기준 공격 범위: 중심 1칸 주변 (8방향)
        Vector3Int[] dirs = {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right,
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, -1, 0)
        };

        foreach (var d in dirs)
        {
            Vector3Int pos = center + d;
            attackPositions.Add(pos);
        }

        return attackPositions;
    }
}
