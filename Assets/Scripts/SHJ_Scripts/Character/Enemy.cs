using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TMPro.Examples.ObjectSpin;

// 캐릭터(유닛)를 직접 조작하는 컨트롤러 클래스
// 선택, 이동, 공격 범위 표시를 담당
public class Enemy : CharacterClass, IWalkable
{
    public Faction faction = Faction.Enemy;

    private bool _isWalkable = false;   // 내부 필드

    public bool IsWalkable => _isWalkable;   // 인터페이스 구현
    public UnitType classUnitType;
    // 유닛의 병종 타입
    // CharacterManager에서 병종별 데이터(IUnitClass)를 가져오기 위한 키

    public IUnitClass currentClass;
    // 현재 유닛이 사용하는 병종 데이터
    // 이동 거리, 공격 범위 계산에 사용됨

    [SerializeField] private CharacterManager manager;
    // 병종 능력, 지형 데이터 등을 관리하는 싱글톤 매니저

    [SerializeField] private Grid grid;
    // 월드 좌표를 타일 좌표로 변환하기 위한 Grid

    private Dictionary<Vector3Int, Color> originalColors = new Dictionary<Vector3Int, Color>();
    // 이동 범위 표시 전에 타일이 가지고 있던 원래 색을 저장
    // 이동 표시 해제 시 원상 복구용

    public Tilemap groundTilemap;
    // 실제 바닥 타일맵
    // 이동 범위 색조 변경과 공격 범위 타일 교체에 사용

    private Vector3Int prevCell;
    // 이전 셀 좌표
    // 현재 코드에서는 사용되지 않지만 남아 있음

    private Vector3 prevPosition;
    // 이동 취소 시 원래 위치로 되돌리기 위한 좌표 저장

    public HashSet<Vector3Int> movableCellsBFS = new HashSet<Vector3Int>();
    // BFS로 계산된 이동 가능 셀 집합
    // CharacterMoves에서 계산된 결과를 참조

    private CharacterMoves moves;
    // 이동 가능 범위를 계산하는 컴포넌트

    public TileBase attackOutlineTile;
    // 공격 범위를 표시하기 위한 테두리 전용 타일

    private HashSet<Vector3Int> attackCells = new HashSet<Vector3Int>();
    // 현재 공격 범위로 표시된 셀들
    // 공격 범위 제거 시 사용
    public Camera mainCamera;
    public GameObject commandUI;
    [SerializeField] private Vector2 uiOffset = new Vector2(100f, 50f);
   

    public ActionType currentAction = ActionType.None;
    protected override void Awake()
    {
        // 부모 클래스(CharacterClass)의 초기화 로직 실행
        base.Awake();
    }

    private void Start()
    {
        // CharacterManager 싱글톤 인스턴스 참조
        manager = CharacterManager.Instance;

        // 병종 타입을 기준으로 병종 데이터 획득
        currentClass = manager.GetUnitClass(classUnitType);

        // 이동 계산 컴포넌트 가져오기
        moves = GetComponent<CharacterMoves>();

        // 이동 컴포넌트가 없으면 정상 동작 불가
        if (moves == null)
            Debug.LogError("CharacterMoves가 붙어있지 않습니다!");

        // 공격 범위 표시용 테두리 타일 생성
        // 내부는 투명, 외곽만 색이 있음
        attackOutlineTile = CreateBorderTile(32, Color.red);
    }

    private void Update()
    {
        EnemyAI();
    }


    public void EnemyAI()
    {

    }


    public void OnSelected()
    {
        Vector3Int cell = groundTilemap.WorldToCell(transform.position);

        if (moves == null) return;

        moves.CalculateMoveRange(cell, currentClass.movementRange);

        // 기존 이동 색 제거
        ClearMoveRangeHighlight();

        // 공격 범위 계산 후 attackCells에 저장
        HashSet<Vector3Int> attackRange = currentClass.ShowAttackRange(cell);
        attackCells = attackRange; // ← 반드시 여기서 미리 채워야 함

        // 공격 범위 표시 (테두리만)
        HighlightAttackRangeOnTilemap(attackRange);

        // 이동 범위 표시 (공격 범위 위는 제외)
        HighlightMoveRangeOnTilemap(moves.movableCellsBFS);

        ChangeState(UnitState.Selected);
    }

    public void OnMove(Vector3Int targetCell)
    {
        // 이동 컴포넌트가 없으면 처리 중단
        if (moves == null)
        {
            Debug.LogError("CharacterMoves 연결 안됨");
            return;
        }

        // 이동 가능 범위가 아니면 처리 중단
        if (!moves.movableCellsBFS.Contains(targetCell))
        {
            Debug.Log($"[OnMove] 이동 불가 타일: {targetCell}");
            return;
        }

        // 상태를 이동 상태로 변경
        ChangeState(UnitState.Move);

        // 이동 취소 대비 현재 위치 저장
        prevPosition = transform.position;

        // 타일 중앙 좌표로 이동
        transform.position = groundTilemap.GetCellCenterWorld(targetCell);

        Debug.Log($"[OnMove] 이동 완료: {targetCell}");

        // 이동 및 공격 하이라이트 제거
        HideHighlights();

        // 이동 후 커맨드 상태로 전환
        OnCommand();
    }

    public void OnCommand()
    {
        ChangeState(UnitState.Command); // ← 여기서 Command로 바뀜
        Debug.Log("애니메이션 진행 끝: OnCommand() 상태!");

        if (commandUI != null)
            commandUI.SetActive(true);

       

    }

    public void OnAction()
    {
        ChangeState(UnitState.Action);
        Debug.Log("OnAction() 상태!");

        if (commandUI != null)
        {
            commandUI.gameObject.SetActive(false);
            Debug.Log("Comment UI 숨김");
        }

        switch (currentAction)
        {
            case ActionType.Attack:
                // 공격 범위 표시 (붉은 필드)
                Vector3Int cell = groundTilemap.WorldToCell(transform.position);
                HashSet<Vector3Int> attackRange = currentClass.ShowAttackRange(cell);
                HighlightAttackRangeOnTilemap(attackRange);
                break;

            case ActionType.Skill:
                // 스킬 범위 표시 (필요시)
                break;

            case ActionType.Item:
                // 아이템 대상 표시 (필요시)
                break;
        }
    }
    private void OnAttack(CharacterController target)
    {
        Debug.Log($"{name}이 {target.name}을 공격합니다!");
        // 여기에 실제 공격 처리 로직 추가 (데미지 계산, 애니메이션 등)
        ChangeState(UnitState.Done);  // 공격 후 상태 변경 예시
    }
    public void OnEnd()
    {
        // 턴 종료 상태
        ChangeState(UnitState.End);
    }

    public void OnDeselected()
    {
        // 선택 해제 후 대기 상태
        ChangeState(UnitState.Idle);
    }

    public void HighlightMoveRangeOnTilemap(HashSet<Vector3Int> positions)
    {
        if (groundTilemap == null) return;

        foreach (var pos in positions)
        {
            // 공격 범위 타일 위는 색 변경 안함
            if (attackCells.Contains(pos)) continue;

            TileBase tile = groundTilemap.GetTile(pos);
            if (tile == null) continue;

            Color originalColor = groundTilemap.GetColor(pos);
            if (!originalColors.ContainsKey(pos))
                originalColors[pos] = originalColor;

            groundTilemap.SetTileFlags(pos, TileFlags.None);
            groundTilemap.SetColor(pos, new Color(0f, 0.5f, 1f, 0.5f));
        }
    }

    public void ClearMoveRangeHighlight()
    {
        // 이동 범위 표시 제거
        if (groundTilemap == null) return;

        foreach (var pos in originalColors.Keys)
        {
            // 저장해둔 원래 색으로 복구
            groundTilemap.SetColor(pos, originalColors[pos]);
            Debug.Log($"[ClearHighlight] 좌표 색 복원: {pos}");
        }

        // 저장 데이터 초기화
        originalColors.Clear();
    }

    public bool GetCellWalkable(Vector3Int cell)
    {
        // 지형 체크
        if (manager == null || manager.GetTerrainMap() == null) return false;
        var terrainData = manager.GetTerrainMap().GetTerrain(cell);
        if (terrainData == null || terrainData.walkable == false) return false;

        // 다른 유닛 체크
        Collider2D hit = Physics2D.OverlapPoint(groundTilemap.GetCellCenterWorld(cell));
        if (hit != null && hit.GetComponent<CharacterController>() != this)
            return false; // 다른 유닛이 있으면 이동 불가

        return true;
    }

    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();
    // 공격 범위 표시 전의 원래 타일 저장용

    public void HighlightAttackRangeOnTilemap(HashSet<Vector3Int> positions)
    {
        if (groundTilemap == null) return;

        attackCells = positions;

        foreach (var pos in positions)
        {
            TileBase originalTile = groundTilemap.GetTile(pos);
            if (originalTile == null) continue;

            if (!originalTiles.ContainsKey(pos))
                originalTiles[pos] = originalTile;

            groundTilemap.SetTileFlags(pos, TileFlags.None);

            // 공격 범위 테두리 타일로 교체
            groundTilemap.SetTile(pos, attackOutlineTile);

            // **항상 흰색 유지**
            groundTilemap.SetColor(pos, Color.white);
        }
    }

    public void ClearAttackRangeHighlight()
    {
        foreach (var pos in attackCells)
        {
            // 저장된 원래 타일로 복구
            if (originalTiles.ContainsKey(pos))
                groundTilemap.SetTile(pos, originalTiles[pos]);
        }

        originalTiles.Clear();
        attackCells.Clear();
    }

    private TileBase CreateBorderTile(int size, Color borderColor)
    {
        // 픽셀 단위 텍스처 생성
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;

        // 투명 색상 정의
        Color transparent = new Color(0, 0, 0, 0);

        // 외곽 픽셀만 색을 채움
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool border = (x < 2 || y < 2 || x > size - 3 || y > size - 3);
                tex.SetPixel(x, y, border ? borderColor : transparent);
            }
        }

        // 텍스처 변경 적용
        tex.Apply();

        // 텍스처를 스프라이트로 변환
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);

        // 타일 객체 생성
        Tile tile = ScriptableObject.CreateInstance<Tile>();

        // 스프라이트 할당
        tile.sprite = sprite;

        // 기본 색조 유지
        tile.color = Color.white;

        return tile;
    }

    private void HideHighlights()
    {
        // 이동 범위와 공격 범위 표시를 모두 제거
        ClearMoveRangeHighlight();
        ClearAttackRangeHighlight();
    }

    private void ShowHighlights()
    {
        // 이동 범위와 공격 범위를 다시 표시
        HighlightMoveRangeOnTilemap(moves.movableCellsBFS);
        HighlightAttackRangeOnTilemap(attackCells);
    }
   


}
