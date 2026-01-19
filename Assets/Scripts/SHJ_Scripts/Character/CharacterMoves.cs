using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMoves : MonoBehaviour
{
    private CharacterController controller;

    // 이동 가능한 타일 좌표(BFS 결과 저장)
    public HashSet<Vector3Int> movableCellsBFS = new HashSet<Vector3Int>();

    private void Start()
    {
        // 같은 GameObject에 붙어있는 CharacterController 자동 연결
        controller = GetComponent<CharacterController>();
        if (controller == null)
            Debug.LogError("CharacterController가 붙어있지 않습니다!");
    }

    /// <summary>
    /// CharacterController에게 요청해서 셀 이동 가능 여부 반환
    /// </summary>
    public bool IsCellWalkable(Vector3Int cell)
    {
        if (controller == null) return false;

        // controller의 GetCellWalkable 호출
        return controller.GetCellWalkable(cell);
    }

    /// <summary>
    /// BFS를 사용해서 시작 지점에서 이동 가능한 모든 셀 계산
    /// movableCellsBFS에 결과 저장
    /// </summary>
    public void CalculateMoveRange(Vector3Int start, int movementRange)
    {
        movableCellsBFS.Clear();

        Queue<(Vector3Int pos, int dist)> open = new Queue<(Vector3Int pos, int dist)>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        open.Enqueue((start, 0));
        visited.Add(start);

        // 4방향 이동
        Vector3Int[] dirs = { Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down };

        while (open.Count > 0)
        {
            var (current, dist) = open.Dequeue();

            if (dist >= movementRange)
                continue;

            foreach (var d in dirs)
            {
                Vector3Int next = current + d;

                if (visited.Contains(next))
                    continue;

                if (!IsCellWalkable(next))
                {
                    Debug.Log($"[BFS] 통과 불가: {next}");
                    continue;
                }

                visited.Add(next);
                open.Enqueue((next, dist + 1));

                movableCellsBFS.Add(next);
                Debug.Log($"[BFS] 이동 가능 추가: {next}");
            }
        }

        Debug.Log($"[BFS] 총 이동 가능 셀 개수: {movableCellsBFS.Count}");
    }
}
