using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMoves : MonoBehaviour
{
    [Header("필수 컴포넌트")]
    public Tilemap tilemap;                // ← 타일 좌표 변환용 Tilemap (Inspector에서 할당)

    [Header("이동 관련 값")]
    public int moveRange = 4;              // 이동 가능 칸 수 (조조전 기준: 보병 4)
    public float moveSpeed = 4f;           // 캐릭터 타일 간 이동 속도

    [Header("애니메이션")]
    private Animator animator;             // ← 4방향 전환용 Animator

    private Vector3Int startCell;          // 현재 캐릭터 위치 (타일 좌표)
    private Vector3Int targetCell;         // 이동 목표 (타일 좌표)

    private Vector3Int[] dirs = new Vector3Int[]   // 4방향 탐색용 벡터
    {
        new Vector3Int(1, 0, 0),    // 우
        new Vector3Int(-1, 0, 0),   // 좌
        new Vector3Int(0, 1, 0),    // 상
        new Vector3Int(0, -1, 0)    // 하
    };

    private void Awake()
    {
        //추가: Animator 캐싱
        animator = GetComponent<Animator>();
        //없으면 오류 체크
        if (animator == null)
            Debug.LogWarning("Animator 없음! 방향 애니 적용 불가");
    }

    private void Start()
    {
        // Start에서 현재 유닛의 월드 좌표 → 타일 좌표 변환
        startCell = tilemap.WorldToCell(transform.position);
    }

    public void MoveToCell(Vector3Int clickedCell)
    {
        // 클릭한 셀 저장
        targetCell = clickedCell;

        // 조조전 기준의 맨해튼 거리 계산
        int distance = Mathf.Abs(targetCell.x - startCell.x) + Mathf.Abs(targetCell.y - startCell.y);

        // 이동력이 부족하면 취소
        if (distance > moveRange)
        {
            Debug.Log("이동 불가: 이동력이 부족함");
            return;
        }

        //추가: 이동 코루틴 실행 (이제 즉시 순간이동 X)
        StartCoroutine(MoveAlongPath());
    }

    IEnumerator MoveAlongPath()
    {
        // 경로 생성 (조조전은 단순 최단 직선이므로 x→y 순 처리)
        List<Vector3Int> path = new List<Vector3Int>();

        Vector3Int cur = startCell;

        // X 이동
        while (cur.x != targetCell.x)
        {
            if (cur.x < targetCell.x) cur += new Vector3Int(1, 0, 0);
            else cur += new Vector3Int(-1, 0, 0);

            path.Add(cur);
        }

        // Y 이동
        while (cur.y != targetCell.y)
        {
            if (cur.y < targetCell.y) cur += new Vector3Int(0, 1, 0);
            else cur += new Vector3Int(0, -1, 0);

            path.Add(cur);
        }

        // 순서대로 이동
        foreach (var step in path)
        {
            Vector3 worldTarget = tilemap.GetCellCenterWorld(step);

            //추가: 방향에 따른 애니메이션 변경 처리
            SetDirectionAnimation(step - startCell);

            //추가: 선형 이동
            while (Vector3.Distance(transform.position, worldTarget) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    worldTarget,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            // 완료 후 현재 좌표 갱신
            startCell = step;
        }

        // 이동 끝 → Idle
        if (animator != null)
            animator.SetFloat("MoveX", 0);
        if (animator != null)
            animator.SetFloat("MoveY", 0);
    }

    // 4방향 애니메이션 처리
    void SetDirectionAnimation(Vector3Int dir)
    {
        if (animator == null) return;

        // Animator 파라미터 기준:
        // MoveX: -1 좌, +1 우
        // MoveY: -1 하, +1 상
        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);
    }
}
