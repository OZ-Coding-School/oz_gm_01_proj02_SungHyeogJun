using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TerrainMap : MonoBehaviour
{
    // 타일맵 참조: 지형 타일이 깔린 Tilemap
    public Tilemap groundTilemap;

    [Header("타일 그룹들")]
    public TileBase[] grassTiles;     // 풀 지형
    public TileBase[] forestTiles;    // 숲 지형
    public TileBase[] mountainTiles;  // 산 지형
    public TileBase[] wallTiles;      // 벽 지형
    public TileBase[] acropolisTiles; // 성채 지형

    [Header("Terrain 설정들")]
    public TerrainData grassTerrain;     // 풀 지형 속성
    public TerrainData forestTerrain;    // 숲 지형 속성
    public TerrainData mountainTerrain;  // 산 지형 속성
    public TerrainData wallTerrain;      // 벽 지형 속성
    public TerrainData acropolisTerrain; // 성채 지형 속성

    // 타일과 TerrainData 매핑용 딕셔너리
    private Dictionary<TileBase, TerrainData> table;

    // Node 2차원 배열: 각 타일 좌표(Node.cell)만 저장
    private Node[,] nodes;

    private void Awake()
    {
        // 1. 타일과 TerrainData 연결
        table = new Dictionary<TileBase, TerrainData>();
        RegisterTiles(grassTiles, grassTerrain);
        RegisterTiles(forestTiles, forestTerrain);
        RegisterTiles(mountainTiles, mountainTerrain);
        RegisterTiles(wallTiles, wallTerrain);
        RegisterTiles(acropolisTiles, acropolisTerrain);

        // 2. 벽 타일에 Grid Collider 적용
        ApplyWallCollider();

        // 3. Node 생성: 좌표만 저장
        CreateNodes();

        // 4. TerrainData.walkable=false인 타일에 이동 불가 처리
        ApplyWalkableFlags();
    }

    /// <summary>
    /// 각 TileBase 배열과 해당 TerrainData를 딕셔너리에 등록
    /// </summary>
    private void RegisterTiles(TileBase[] tiles, TerrainData data)
    {
        foreach (var tile in tiles)
        {
            if (tile == null) continue;

            if (!table.ContainsKey(tile))
                table.Add(tile, data);
        }
    }

    /// <summary>
    /// 주어진 좌표의 TerrainData 반환
    /// Node에서 직접 walkable을 가지고 있지 않고 여기서 참조
    /// </summary>
    public TerrainData GetTerrain(Vector3Int cellPos)
    {
        TileBase tile = groundTilemap.GetTile(cellPos);

        if (tile == null) return null;

        if (table.TryGetValue(tile, out TerrainData data))
            return data;

        return null;
    }

    /// <summary>
    /// wallTiles 배열에 포함된 타일에는 Grid Collider 적용
    /// 기존의 벽 기능 유지
    /// </summary>
    private void ApplyWallCollider()
    {
        BoundsInt bounds = groundTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = groundTilemap.GetTile(pos);
            if (tile == null) continue;

            foreach (var wallTile in wallTiles)
            {
                if (tile == wallTile)
                {
                    groundTilemap.SetColliderType(pos, Tile.ColliderType.Grid);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Tilemap 전체 셀 좌표를 Node로 생성
    /// Node는 좌표(cell)만 가지며 이동 가능 여부는 TerrainData에서 판단
    /// </summary>
    private void CreateNodes()
    {
        BoundsInt bounds = groundTilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;

        nodes = new Node[width, height];
        Vector3Int offset = bounds.min;

        foreach (var pos in bounds.allPositionsWithin)
        {
            Vector3Int index = pos - offset;

            // Node 생성: 좌표만 저장
            nodes[index.x, index.y] = new Node(pos);
        }
    }

    /// <summary>
    /// 각 Node 좌표에 해당하는 타일의 TerrainData.walkable을 확인
    /// false인 경우 이동 불가 처리(Grid Collider 적용)
    /// </summary>
    private void ApplyWalkableFlags()
    {
        BoundsInt bounds = groundTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            TerrainData data = GetTerrain(pos);

            if (data != null && data.walkable == false)
            {
                // 이동 불가 타일이면 Collider 설정
                groundTilemap.SetColliderType(pos, Tile.ColliderType.Grid);
            }
        }
    }
}
