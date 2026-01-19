using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    /// <summary>
    /// 셀 좌표 (Tilemap 기준 좌표)
    /// </summary>
    public Vector3Int cell;      // 이 노드가 위치한 타일 좌표
    public Node parent;          // 경로 추적용 부모 노드
    public int gCost;            // 시작 노드에서 현재 노드까지의 비용
    public int hCost;            // 현재 노드에서 목표 노드까지의 예상 비용
    public int fCost => gCost + hCost; // 총 비용 = g + h
    /// <summary>
    /// 생성자: 노드 좌표만 지정
    /// </summary>
    /// <param name="cell">타일 좌표</param>
    public Node(Vector3Int cell)
    {
        this.cell = cell;
        parent = null;
        gCost = 0;
        hCost = 0;
    }
}
