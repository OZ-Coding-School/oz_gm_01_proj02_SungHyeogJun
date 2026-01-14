using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[System.Serializable]
public class TerrainMap : MonoBehaviour
{

    public Tilemap groundTilemap;

    [Header("타일 그룹들")]
    public TileBase[] grassTiles;
    public TileBase[] forestTiles;
    public TileBase[] mountainTiles;
    public TileBase[] wallTiles;
    public TileBase[] acropolisTiles;

    // 각 Terrain 설정 파일
    [Header("Terrain 설정들")]
    public TerrainData grassTerrain;
    public TerrainData forestTerrain;
    public TerrainData mountainTerrain;
    public TerrainData wallTerrain;
    public TerrainData acropolisTerrain;

    private Dictionary<TileBase, TerrainData> table;
    // Start is called before the first frame update
    private void Awake()
    {
        table = new Dictionary<TileBase, TerrainData>();
        RegisterTiles(grassTiles, grassTerrain);
        RegisterTiles(forestTiles, forestTerrain);
        RegisterTiles(mountainTiles, mountainTerrain);
        RegisterTiles(wallTiles, wallTerrain);
        RegisterTiles(acropolisTiles, acropolisTerrain);

        ApplyWallCollider();
    }
    private void RegisterTiles(TileBase[] tiles, TerrainData data)
    {
        foreach (var tile in tiles)
        {
            if (tile == null) continue;

            if (!table.ContainsKey(tile))
                table.Add(tile, data);
        }
    }

    // 특정 셀 좌표(Vector3Int)를 주면
    // 그 위치의 지형 정보(TerrainData)를 반환하는 함수
    public TerrainData GetTerrain(Vector3Int cellPos)
    {
        // Tilemap에게 물어본다:
        // "이 셀 좌표에 깔린 타일 에셋이 뭐야?"
        TileBase tile = groundTilemap.GetTile(cellPos);

        // 타일이 없다면 (빈 칸이면)
        if (tile == null)
            return null;

        // Dictionary에서 이 타일에 해당하는 TerrainData를 찾는다
        if (table.TryGetValue(tile, out TerrainData data))
            return data;

        // 타일은 있는데, 지형 정보가 등록되지 않은 경우
        return null;
    }

    private void ApplyWallCollider()
    {
        // cellBounds : Tilemap의 전체 셀 영역을 정의하는 BoundsInt
        BoundsInt bounds = groundTilemap.cellBounds;

        // bounds.allPositionsWithin : (minX~maxX, minY~maxY) 모든 셀 좌표를 순회하는 IEnumerable<Vector3Int>
        foreach (var pos in bounds.allPositionsWithin)
        {
            // 해당 좌표의 타일 가져오기
            TileBase tile = groundTilemap.GetTile(pos);

            // 타일이 없으면 건너뛰기
            if (tile == null) continue;

            // wallTiles 배열에 tile이 포함되어 있는지 검사
            foreach (var wallTile in wallTiles)
            {
                if (tile == wallTile)
                {
                    // 벽 타일이면 Grid Collider 부여
                    groundTilemap.SetColliderType(pos, Tile.ColliderType.Grid);
                    break; // 더 검사할 필요 없음
                }
            }
        }
    }
}
