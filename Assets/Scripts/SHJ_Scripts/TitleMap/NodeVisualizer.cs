using UnityEngine;

// Scene 뷰에서 Node 좌표 확인용
public class NodeVisualizer : MonoBehaviour
{
    public TerrainMap terrainMap; // TerrainMap 참조
    public Color walkableColor = Color.green;   // 이동 가능
    public Color unwalkableColor = Color.red;   // 이동 불가
    public float gizmoSize = 0.5f;             // 표시 크기

    private void OnDrawGizmos()
    {
        if (terrainMap == null || terrainMap.groundTilemap == null) return;

        BoundsInt bounds = terrainMap.groundTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            TerrainData data = terrainMap.GetTerrain(pos);
            if (data == null) continue;

            // 이동 가능 여부에 따라 색상 변경
            Gizmos.color = data.walkable ? walkableColor : unwalkableColor;

            // 실제 좌표를 Tilemap 기준으로 변환
            Vector3 worldPos = terrainMap.groundTilemap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0);
            Gizmos.DrawCube(worldPos, Vector3.one * gizmoSize);
        }
    }
}
