using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMoves : MonoBehaviour
{
    [Header("타일맵 참조")]
    public Tilemap groundTilemap;    // Inspector에서 할당
    public Vector3Int currentCell;  // 캐릭터가 서 있는 타일 좌표


    private void Awake()
    {
        // 내 캐릭터의 월드 위치 → 타일 좌표
        currentCell = groundTilemap.WorldToCell(transform.position);

        Debug.Log($"캐릭터 시작 타일 좌표: {currentCell}");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentCell = groundTilemap.WorldToCell(transform.position);
    }
}
