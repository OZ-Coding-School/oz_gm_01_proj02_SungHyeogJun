using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterController : CharacterClass
{
    [SerializeField] private UnitType classUnitType;
    // 유닛의 병종 타입 (보병/기병/궁병 등) → 병종 능력/지형 이동/공격 방식 차이

    [SerializeField] private CharacterManager manager;
    // 병종 능력 데이터 요청/관리 담당 (Atk/Def/Move 등)

    [SerializeField] private Grid grid;
    // 타일 좌표 변환용 Grid
    

    // 병종 데이터 가져오기용
    private int movementRange;                     // 이동 범위
    private Dictionary<TerrainType, int> costTable; // 지형별 이동 비용
    [SerializeField] private Tilemap groundTilemap;                 // 타일 표시용 / 이동판정용
    // Grid 없을 때 대응 타일맵 직접 사용
    private TileBase[] distanceTiles;
    private List<Vector3Int> movableCells;
    private bool waitingMoveClick = false;

    private Vector3Int prevCell;
    private Vector3 prevPosition;

    protected override void Awake()   // 초기 준비 단계 (Animator/Manager 연결)
    {
        base.Awake();
       
    }
    private void Start()
    {
        // CharacterManager 연결 (싱글톤)
        if (manager == null)
            manager = CharacterManager.Instance;

        if (manager == null)
        {
            Debug.LogWarning($"{name} : CharacterManager 없음!");
            return;
        }
        RequestMyData();
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
        if (state != UnitState.Selected) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int clickedCell = groundTilemap.WorldToCell(worldPos);

        if (movableCells != null && movableCells.Contains(clickedCell))
        {
            prevCell = groundTilemap.WorldToCell(transform.position);
            prevPosition = transform.position;
            manager.GetUnitClass(classUnitType).ClearHighlight();

            transform.position = groundTilemap.GetCellCenterWorld(clickedCell);
            ChangeState(UnitState.Action); // 이동 완료
            HideMoveTiles();
            OnMove();
        }

    }
    private void CheckCancelInput() //상태취소
    {
        if (!Input.GetMouseButtonDown(1)) return; // 오른쪽 클릭

        switch (state)
        {
            case UnitState.Selected:
                Debug.Log("취소: 선택 해제");
                manager.GetUnitClass(classUnitType).ClearHighlight();
                
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
        if (movableCells == null) return; //추가!

        var unit = manager.GetUnitClass(classUnitType);
        if (unit == null) return;

        foreach (var cell in movableCells)
        {
            groundTilemap.SetColor(cell, Color.white);
        }

        movableCells = null; // 중요! 이동 후 null로
    }
    public void OnSelected()          // 유닛이 선택됨 이동표시 → UI 커맨드 입력 전 단계
    {
        HideMoveTiles(); // 이전 잔여 타일 숨기기
        ChangeState(UnitState.Selected);

        var unit = manager.GetUnitClass(classUnitType);
        if (unit == null) return;

        // 현재 좌표
        Vector3Int currentCell = (grid != null)
            ? grid.WorldToCell(transform.position)
            : groundTilemap.WorldToCell(transform.position);

        // 이동 범위 표시만 함
        unit.ShowDummyRange(currentCell);

        // 이동 가능한 셀 배열 계산 (추후 이동용)
        movableCells = new List<Vector3Int>();
        for (int dx = -unit.movementRange; dx <= unit.movementRange; dx++)
        {
            for (int dy = -unit.movementRange; dy <= unit.movementRange; dy++)
            {
                int distance = Mathf.Abs(dx) + Mathf.Abs(dy);
                if (distance == 0 || distance > unit.movementRange) continue;

                Vector3Int cell = currentCell + new Vector3Int(dx, dy, 0);
                movableCells.Add(cell);
            }
        }

        Debug.Log($"이동 범위 표시 완료 (총 {movableCells.Count}개)");
    }

    public void OnMove()              // 실제 이동 시작 상태 (조조전 이동 경로 표시 → 이동 처리)
    {
        ChangeState(UnitState.Move);
        Debug.Log($"애니메이션진행:  OnMove()상태!");
        OnCommand();
        waitingMoveClick = true; // 추가: 타일 클릭 대기 상태 진입
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

    private void RequestMyData()      // 병종 능력 요청 (Atk/Def/Move 등 불러오기)
    {
        var unit = manager.GetUnitClass(classUnitType);
        if (unit == null)
        {
            Debug.LogWarning($"병종 {classUnitType} 클래스를 찾을 수 없음");
            return;
        }

        movementRange = unit.movementRange;
        costTable = unit.CostTable;
        groundTilemap = manager.groundTilemap;

        // 배열 연결
        distanceTiles = ((InfantryClass)unit).distanceTiles;
    }
}

