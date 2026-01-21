using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Comment : MonoBehaviour
{
    public Button attackButton;
    public Button skillButton;
    public Button itemButton;

    private CharacterController character;

    public void SetCharacter(CharacterController ctrl)
    {
        character = ctrl;
        Debug.Log("버튼 연결");
        if (attackButton != null)
            attackButton.onClick.AddListener(() => OnAttackButton());
        if (skillButton != null)
            skillButton.onClick.AddListener(() => OnSkillButton());
        if (itemButton != null)
            itemButton.onClick.AddListener(() => OnItemButton());
    }

    private void OnAttackButton()
    {
        if (character != null) // 조건 제거
        {
            character.currentAction = ActionType.Attack;
            character.OnAction();
            Debug.Log("Comment → Attack 버튼 클릭 → OnAction 호출");
        }
    }

    private void OnSkillButton()
    {
        if (character != null) // 조건 제거
        {
            character.currentAction = ActionType.Skill;
            character.OnAction();
            Debug.Log("Comment → Skill 버튼 클릭 → OnAction 호출");
        }
    }

    private void OnItemButton()
    {
        if (character != null) // 조건 제거
        {
            character.currentAction = ActionType.Item;
            character.OnAction();
            Debug.Log("Comment → Item 버튼 클릭 → OnAction 호출");
        }
    }
}
