using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
CharacterManager

역할:
- SoldierResult(능력치) 테이블 관리
- 병종 스크립트(Infantry 등) 연결
- CharacterController가 요청할 때 중계 역할
*/

public class CharacterManager : MonoBehaviour
{
    // 상태번호 → 능력치 테이블
    private Dictionary<int, SoldierResult> resultTable;
    public static CharacterManager Instance;
    // 병종 연결 (현재 테스트: Infantry만)
    public InfantryClass infantry;
    public Tilemap groundTilemap;                              // 표시용/이동판정용 타일맵
    public Dictionary<TileBase, TerrainType> tileTerrainTable; // 타일 → 지형타입 매핑테이블
    private void Awake()
    {
        // 테이블 먼저 생성
        resultTable = new Dictionary<int, SoldierResult>();
        RegisterResults();

        // 그 다음 병종 연결
        if (infantry != null)
        {
            infantry.SetManager(this);
        }
        Instance = this;
    }

    private void RegisterResults()
    {
        resultTable[1] = new SoldierResult { attack = 100, defense = 100, moveCost = 100 };
        resultTable[2] = new SoldierResult { attack = 80, defense = 80, moveCost = 80 };
        resultTable[3] = new SoldierResult { attack = 60, defense = 60, moveCost = 60 };
    }

    public SoldierResult GetResult(int stateNumber)
    {
        if (!resultTable.ContainsKey(stateNumber))
        {
            Debug.LogError($"[CharacterManager] 없는 stateNumber 요청됨 → {stateNumber}");
            return null;
        }
        return resultTable[stateNumber];
    }

    public SoldierResult RequestUnitData(UnitType type)
    {
      

        Debug.LogWarning($"UnitType [{type}] 에 대한 유닛 데이터 없음");
        return null;
    }

    public IUnitClass GetUnitClass(UnitType type)
    {
        switch (type)
        {
            case UnitType.Infantry:
                return infantry;
                // 필요하면 Cavalry, Archer 등 추가
        }
        return null;
    }
}

[System.Serializable]
public class SoldierResult
{
    public int attack;
    public int defense;
    public int moveCost;
}
