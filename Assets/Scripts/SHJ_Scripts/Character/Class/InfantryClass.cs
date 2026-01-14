using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InfantryClass : MonoBehaviour, IUnitClass
{
    public Tilemap groundTilemap;
    public Color highlightColor = Color.green;

    private Dictionary<Vector3Int, Color> originalColors = new Dictionary<Vector3Int, Color>();

    public int movementRange { get; private set; } = 4;
    public Dictionary<TerrainType, int> CostTable { get; private set; }

    private TerrainMap terrainMap;
    private CharacterManager manager;

    public UnitType unitType { get; private set; } = UnitType.Infantry;
    public FieldType currentField = FieldType.None;

    public int stateNumber = 1; // 결과 테이블 인덱스

    private void Awake()
    {
        CostTable = new Dictionary<TerrainType, int>();
        RegisterCosts();

        terrainMap = FindObjectOfType<TerrainMap>();

        manager = FindObjectOfType<CharacterManager>();
        if (manager != null)
        {
            manager.infantry = this;
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
        ClearHighlight();
        for (int dx = -movementRange; dx <= movementRange; dx++)
        {
            for (int dy = -movementRange; dy <= movementRange; dy++)
            {
                if (Mathf.Abs(dx) + Mathf.Abs(dy) <= movementRange)
                {
                    Vector3Int cell = startCell + new Vector3Int(dx, dy, 0);

                    if (!originalColors.ContainsKey(cell))
                        originalColors[cell] = groundTilemap.GetColor(cell);

                    groundTilemap.SetTileFlags(cell, TileFlags.None);
                    groundTilemap.SetColor(cell, highlightColor);
                }
            }
        }
    }

    public void ClearHighlight()
    {
        foreach (var kvp in originalColors)
            groundTilemap.SetColor(kvp.Key, kvp.Value);

        originalColors.Clear();
    }

    public void SetManager(CharacterManager mgr)
    {
        manager = mgr;
    }

    public SoldierResult GetCurrentResult()
    {
        if (manager != null)
            return manager.GetResult(stateNumber);

        Debug.LogWarning("CharacterManager 없음!");
        return null;
    }

    //
    public SoldierResult RequestResult(CharacterManager mgr)
    {
        // 매니저에게 결과 반환 요청 (중계 역할)
        return mgr.GetResult(stateNumber);
    }
}
