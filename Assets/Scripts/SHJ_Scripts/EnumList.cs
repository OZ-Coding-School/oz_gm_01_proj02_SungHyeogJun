using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CharacterEnum
{
    None,
}

public enum ActionType
{
    None,
    Attack,
    Skill,
    Item
}
public enum UnitType
{
    Infantry,   // 보병
    Cavalry,    // 기병
    Archer,     // 궁병 (예시)
    Bandit,     // 산적 / 의적 계열
    YellowScarves,   //황건적
    Strategist

}

public enum TerrainType
{

    None = 0,
    Plain = 1 << 0,       // 평지
    Forest = 1 << 1,      // 숲
    Mountain = 1 << 2,    // 산
    Road = 1 << 3,        // 길s
    City = 1 << 4,        // 성/진
    Snow = 1 << 5,        // 설원 등
    Water = 1 << 6,       // 강/호수
    Wall = 1 << 7         // 벽
    
}
public enum UnitState
{
    Idle,
    Selected,
    Move,
    Action,
    Done,
    Command,
    End
}

public enum Faction
{
    Ally,
    Friendly,
    Enemy
}

public enum FieldType
{
    None = 0,
    Field1 = 1,
    Field2 = 2,
    Field3 = 3
}
