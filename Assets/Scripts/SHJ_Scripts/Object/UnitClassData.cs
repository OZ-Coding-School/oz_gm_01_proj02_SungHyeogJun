using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitClassData", menuName = "Game/UnitClassData")]
public class UnitClassData : MonoBehaviour
{
   
   
    
    public string className;     // 보병, 기병 등
    public int move;             // 이동력
    public int range;            // 공격 사거리
    public int maxHP;
    public int attack;
    public int defense;

    // 나중에 지형 패널티 테이블 들어올 자리
    
}
