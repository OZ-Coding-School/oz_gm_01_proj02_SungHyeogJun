using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterController : CharacterClass
{
    [SerializeField] private UnitType classUnitType;
    private IUnitClass currentClass;
    // 유닛의 병종 타입 (보병/기병/궁병 등) → 병종 능력/지형 이동/공격 방식 차이

    [SerializeField] private CharacterManager manager;
    // 병종 능력 데이터 요청/관리 담당 (Atk/Def/Move 등)
    [SerializeField] private Grid grid;
    // 타일 좌표 변환용 Grid
    private Dictionary<Vector3Int, Color> originalColors = new Dictionary<Vector3Int, Color>();

    public Tilemap groundTilemap; // 타일 표시용 / 이동판정용

    private Vector3Int prevCell;
    private Vector3 prevPosition;

    public HashSet<Vector3Int> movableCellsBFS = new HashSet<Vector3Int>();

    private CharacterMoves moves;  // CharacterMoves 연결용
    protected override void Awake()   // 초기 준비 단계 (Animator/Manager 연결)
    {
        base.Awake();

    }
    private void Start()
    {

        manager = CharacterManager.Instance;

        currentClass = manager.GetUnitClass(classUnitType);
        moves = GetComponent<CharacterMoves>();
        if (moves == null)
            Debug.LogError("CharacterMoves가 붙어있지 않습니다!");

    }
    private void Update()
    {
        CheckSelectInput();
        CheckMoveInput();
        CheckCancelInput();
    }

    private void CheckSelectInput() // 추가: 유닛 선택 전용
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit && hit.transform.gameObject == gameObject)
        {
            Debug.Log("클릭: 유닛 선택");
            OnSelected();
        }
    }

    private void CheckMoveInput() // 추가: 이동 타일 클릭 전용
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (state != UnitState.Selected) return; // 선택 상태에서만

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int clickedCell = groundTilemap.WorldToCell(worldPoint);

        // 클릭 좌표를 타일 중앙 좌표로 보정 (BFS 좌표와 정확히 맞추기 위해)
        clickedCell = new Vector3Int(clickedCell.x, clickedCell.y, 0);

        Debug.Log($"[CheckMoveInput] 클릭된 셀: {clickedCell}");

        if (moves == null)
        {
            Debug.LogError("[CheckMoveInput] CharacterMoves가 연결되어 있지 않음");
            return;
        }

        if (!moves.movableCellsBFS.Contains(clickedCell))
        {
            Debug.Log($"[CheckMoveInput] BFS 계산 결과에 없는 타일: {clickedCell}");
            return;
        }

        Debug.Log($"[CheckMoveInput] 이동 처리 시작: {clickedCell}");
        OnMove(clickedCell);

    }
    private void CheckCancelInput() //상태취소
    {
        if (!Input.GetMouseButtonDown(1)) return; // 오른쪽 클릭

        switch (state)
        {
            case UnitState.Selected:
                Debug.Log("취소: 선택 해제");
                //manager.GetUnitClass(classUnitType).ClearHighlight();

                HideMoveTiles();
                break;

            case UnitState.Move:
                Debug.Log("취소: 이동 선택 취소");
                OnSelected(); // 이동 표시만 다시
                break;

            case UnitState.Command:
                Debug.Log("취소: 커맨드 취소 → 이동 선택으로");
                // 위치 롤백
                transform.position = prevPosition;

                // 상태 복구
                ChangeState(UnitState.Selected);
                OnSelected();
                break;

            default:
                break;
        }
    }
    private void HideMoveTiles()
    {

    }
    public void OnSelected()          // 유닛이 선택됨 이동표시 → UI 커맨드 입력 전 단계
    {
        Debug.Log("OnSelected 상태");

        Vector3Int cell = groundTilemap.WorldToCell(transform.position);

        if (moves == null)
        {
            Debug.LogError("CharacterMoves 연결 안됨");
            return;
        }

        // BFS 계산해서 이동 가능 좌표 저장
        moves.CalculateMoveRange(cell, currentClass.movementRange);

        // 현재 유닛의 BFS 이동 가능 좌표를 가져와서 표시
        HighlightMoveRangeOnTilemap(moves.movableCellsBFS);

        ChangeState(UnitState.Selected); // 선택 상태

    }

    public void OnMove(Vector3Int targetCell)
    {
        if (moves == null)
        {
            Debug.LogError("CharacterMoves 연결 안됨");
            return;
        }

        if (!moves.movableCellsBFS.Contains(targetCell))
        {
            Debug.Log($"[OnMove] 이동 불가 타일: {targetCell}");
            return;
        }

        ChangeState(UnitState.Move);

        prevPosition = transform.position;
        transform.position = groundTilemap.GetCellCenterWorld(targetCell); // 타일 중심으로 이동

        Debug.Log($"[OnMove] 이동 완료: {targetCell}");

        OnCommand(); // 이동 후 커맨드 상태로 전환
    }

    public void OnCommand()           // 커맨드 메뉴 진입 (이동/공격/책략 선택 단계)
    {
        ChangeState(UnitState.Command);
        Debug.Log($"애니메이션진행끝:  OnCommand()상태!");

    }



    public void OnAction()            // 공격/책략 실행 상태 (실제 액션 처리)
    {
        ChangeState(UnitState.Action);
    }

    public void OnEnd()               // 턴 종료 상태 (삼조전 스타일로 ‘암전/끝난 유닛 표시’)
    {
        ChangeState(UnitState.End);
    }

    public void OnDeselected()        // 선택 해제 → 다시 Idle (기본 대기 상태)
    {
        ChangeState(UnitState.Idle);
    }

    //private void RequestMyData()      // 병종 능력 요청 (Atk/Def/Move 등 불러오기)
    //{
    //}

    public void HighlightMoveRangeOnTilemap(HashSet<Vector3Int> positions)
    {
        if (groundTilemap == null) return;

        foreach (var pos in positions)
        {
            TileBase tile = groundTilemap.GetTile(pos);
            if (tile == null) continue;

            // 타일 색 변경
            Color originalColor = groundTilemap.GetColor(pos);
            if (!originalColors.ContainsKey(pos))
                originalColors[pos] = originalColor;

            // 타일 전체에 색칠 (Tilemap Flag를 없애야 적용 가능)
            groundTilemap.SetTileFlags(pos, TileFlags.None);
            groundTilemap.SetColor(pos, new Color(0f, 0.5f, 1f, 0.5f));
        }
    }

    public void ClearMoveRangeHighlight()
    {
        if (groundTilemap == null) return;

        foreach (var pos in originalColors.Keys)
        {
            groundTilemap.SetColor(pos, originalColors[pos]);
            Debug.Log($"[ClearHighlight] 좌표 색 복원: {pos}");
        }

        originalColors.Clear();
    }

    public bool GetCellWalkable(Vector3Int cell)
    {
        if (manager == null || manager.GetTerrainMap() == null) return false;

        var terrainData = manager.GetTerrainMap().GetTerrain(cell);
        if (terrainData == null) return false;

        return terrainData.walkable;
    }
}

