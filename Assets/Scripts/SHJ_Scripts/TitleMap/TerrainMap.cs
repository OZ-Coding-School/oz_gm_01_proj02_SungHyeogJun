using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[System.Serializable]
public class TerrainMap : MonoBehaviour
{

    public Tilemap groundTilemap;

    public TileTerrainLink[] links;     //타일의 지형을 데이터로 연결
    private Dictionary<TileBase, TerrainData> table;
    // Start is called before the first frame update
    private void Awake()
    {
        table = new Dictionary<TileBase, TerrainData>();
        foreach (var link in links)
        {
            // 같은 타일이 이미 등록되어 있지 않다면
            if (!table.ContainsKey(link.tile))
            {
                // 타일을 키로, 지형 데이터를 값으로 등록
                table.Add(link.tile, link.terrain);
            }
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
}
