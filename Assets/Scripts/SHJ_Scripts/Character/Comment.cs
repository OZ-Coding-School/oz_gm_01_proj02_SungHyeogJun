using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Comment : MonoBehaviour
{
    public Button attackButton;
    public Button skillButton;
    public Button itemButton;

    private CharacterController character;

    public void SetCharacter(CharacterController ctrl)
    {
        character = ctrl;

        if (attackButton != null)
            attackButton.onClick.AddListener(() => AttackAction());
        if (skillButton != null)
            skillButton.onClick.AddListener(() => SkillAction());
        if (itemButton != null)
            itemButton.onClick.AddListener(() => ItemAction());
    }

    private void AttackAction()
    {
        if (character != null)
        {
            character.ChangeState(UnitState.Action);
            Debug.Log("공격 버튼 클릭 → 상태: Action");
            // TODO: 공격 실행 로직 추가 가능
        }
    }

    private void SkillAction()
    {
        if (character != null)
        {
            character.ChangeState(UnitState.Action);
            Debug.Log("스킬 버튼 클릭 → 상태: Action");
            // TODO: 스킬 실행 로직 추가 가능
        }
    }

    private void ItemAction()
    {
        if (character != null)
        {
            character.ChangeState(UnitState.Action);
            Debug.Log("아이템 버튼 클릭 → 상태: Action");
            // TODO: 아이템 사용 로직 추가 가능
        }
    }
}
