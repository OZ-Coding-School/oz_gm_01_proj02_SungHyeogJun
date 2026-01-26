using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StatsData", menuName = "Stats")]
public class PlayerStats : ScriptableObject
{
    public int _hp;                 //체력
    public int _str;                //공격력
    public int _vit;                //방어력
    public float _hit;              //명중률
    public float _block;            //방어
    public float _critical;         //크르티컬데미지
    public float _criticalhit;     //크르티컬확률


}
