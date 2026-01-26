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

    [SerializeField] private PlayerStats[] playerStats;
    // 상태번호 → 능력치 테이블

    public static CharacterManager Instance;
    // 병종 연결 (현재 테스트: Infantry만)
    public TerrainMap terrainMap;

    private InfantryClass infantry;
    private void Awake()
    {

        Instance = this;
        infantry = FindObjectOfType<InfantryClass>();
    }

    public TerrainMap GetTerrainMap()
    {
        return terrainMap;
    }

    public IUnitClass GetUnitClass(UnitType type)
    {
        return infantry; // 지금은 단일이라 이걸로 끝
    }
}


