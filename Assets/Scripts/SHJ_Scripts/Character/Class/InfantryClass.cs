using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InfantryClass : MonoBehaviour, IUnitClass
{
    public int movementRange { get; private set; } = 4;
    public Dictionary<TerrainType, int> CostTable { get; private set; }

    [SerializeField] private TileBase[] _distanceTiles;   // ← 실제 데이터 보관

    public TileBase[] distanceTiles => _distanceTiles;

    public Color highlightColor = Color.cyan;

    public UnitType unitType { get; private set; } = UnitType.Infantry;

    private TerrainMap terrainMap;
    private CharacterManager manager;
    private Tilemap groundTilemap;

    private Dictionary<Vector3Int, Color> originalColors = new Dictionary<Vector3Int, Color>();

    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();
    // ← 인터페이스 구현

    private void Awake()
    {
        CostTable = new Dictionary<TerrainType, int>();
        RegisterCosts();

        terrainMap = FindObjectOfType<TerrainMap>();

        manager = FindObjectOfType<CharacterManager>();
        if (manager != null)
        {
            manager.infantry = this;

            //타일맵 연결 여기서 해줌
            groundTilemap = manager.groundTilemap;
        }
    }

    private void RegisterCosts()
    {
        CostTable[TerrainType.Plain] = 1;
        CostTable[TerrainType.Forest] = 2;
        CostTable[TerrainType.Mountain] = 999;
        CostTable[TerrainType.Road] = 1;
        CostTable[TerrainType.City] = 1;
        CostTable[TerrainType.Water] = 999;
        CostTable[TerrainType.Snow] = 2;
        CostTable[TerrainType.Wall] = 999;
    }

    public void ShowDummyRange(Vector3Int startCell)
    {
        if (groundTilemap == null)
            return;

        ClearHighlight();

        for (int dx = -movementRange; dx <= movementRange; dx++)
        {
            for (int dy = -movementRange; dy <= movementRange; dy++)
            {
                int distance = Mathf.Abs(dx) + Mathf.Abs(dy);
                if (distance > movementRange)
                    continue;

                Vector3Int cell = startCell + new Vector3Int(dx, dy, 0);

                if (!groundTilemap.HasTile(cell))
                    continue;

                // 원본 타일 저장
                if (!originalTiles.ContainsKey(cell))
                    originalTiles[cell] = groundTilemap.GetTile(cell);

                // distance tile 적용
                if (distance > 0 && distance - 1 < distanceTiles.Length)
                {
                    groundTilemap.SetTileFlags(cell, TileFlags.None);
                    groundTilemap.SetTile(cell, distanceTiles[distance - 1]);
                }

                // 원본 색 저장
                if (!originalColors.ContainsKey(cell))
                    originalColors[cell] = groundTilemap.GetColor(cell);

                groundTilemap.SetTileFlags(cell, TileFlags.None);
                groundTilemap.SetColor(cell, highlightColor);
            }
        }
    }

    public void ClearHighlight()
    {
        if (groundTilemap == null) return;

        // 색 복구
        foreach (var kvp in originalColors)
        {
            groundTilemap.SetTileFlags(kvp.Key, TileFlags.None);
            groundTilemap.SetColor(kvp.Key, kvp.Value);
        }
        originalColors.Clear();

        // 타일 복구
        foreach (var kvp in originalTiles)
        {
            groundTilemap.SetTileFlags(kvp.Key, TileFlags.None);
            groundTilemap.SetTile(kvp.Key, kvp.Value);
        }
        originalTiles.Clear();
    }

    public void SetManager(CharacterManager mgr)
    {
        manager = mgr;
        groundTilemap = manager.groundTilemap; //SetManager 통해서도 연결 가능
    }

   
}
