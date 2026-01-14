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

    [SerializeField] private Tilemap groundTilemap;
    // Grid 없을 때 대응 타일맵 직접 사용
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
        if (Input.GetMouseButtonDown(0)) // 왼쪽 클릭
        {
            CheckClick();
        }
    }

    private void CheckClick()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit && hit.transform.gameObject == gameObject)
        {
            Debug.Log("클릭");
            OnSelected();
        }
    }
    public void OnSelected()          // 유닛이 선택됨 이동표시 → UI 커맨드 입력 전 단계
    {
        // 유닛의 상태를 "선택됨"으로 변경
        ChangeState(UnitState.Selected);

        // 병종 클래스 요청 (예: InfantryClass)
        var unit = manager.GetUnitClass(classUnitType);
        if (unit == null)
        {
            Debug.LogWarning($"병종 {classUnitType} 클래스를 찾을 수 없음");
            return;
        }

        // 현재 유닛의 타일 좌표 계산
        Vector3Int cell = (grid != null)
            ? grid.WorldToCell(transform.position)
            : groundTilemap.WorldToCell(transform.position);

        // 이동범위 표시 요청 (병종 클래스가 처리)
        unit.ShowDummyRange(cell);
    }

    public void OnCommand()           // 커맨드 메뉴 진입 (이동/공격/책략 선택 단계)
    {
        ChangeState(UnitState.Command);
    }

    public void OnMove()              // 실제 이동 시작 상태 (조조전 이동 경로 표시 → 이동 처리)
    {
        ChangeState(UnitState.Move);
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
        if (manager == null)
        {
            Debug.LogWarning($"{name} : CharacterManager 없음!");
            return;
        }

        var result = manager.RequestUnitData(classUnitType);
        if (result != null)
        {
            Debug.Log($"[{classUnitType}] Atk:{result.attack}, Def:{result.defense}, Move:{result.moveCost}");
        }
    }
}

