using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterClass : MonoBehaviour
{
    [SerializeField] private Faction faction = Faction.Enemy;
    public Faction Faction => faction;

    [SerializeField] private Animator animator;
    protected UnitState state = UnitState.Idle;
    public UnitState CurrentState => state;
    protected virtual void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public virtual void ChangeState(UnitState newState)
    {
        state = newState;

        switch (state)
        {
            case UnitState.Idle:
                OnIdle();
                break;
            case UnitState.Selected:
                OnSelected();
                break;
            case UnitState.Command:
                OnCommand();
                break;
            case UnitState.Move:
                OnMove();
                break;
            case UnitState.Action:
                OnAction();
                break;
            case UnitState.End:
                OnEnd();
                break;
        }
    }

    protected virtual void OnIdle()
    {
        if (animator != null)
            animator.speed = 1f;
    }

    protected virtual void OnSelected()
    {
        if (animator != null)
            animator.speed = 0f;

        ShowFactionColor();
    }

    protected virtual void OnCommand()
    {
        // 아직 내용 없음 (UI 띄울 예정)
    }

    protected virtual void OnMove()
    {
        // 이동 애니 시작 준비
    }

    protected virtual void OnAction()
    {
        // 공격/책략 등 실행 전 준비
    }

    protected virtual void OnEnd()
    {
        // 종료 연출 (삼조전 검은 처리)
    }

    public virtual void OnDeselected()
    {
        ChangeState(UnitState.Idle);
    }

    public virtual void ShowFactionColor()
    {
        // 너거 기존 코드 그대로
    }
}
