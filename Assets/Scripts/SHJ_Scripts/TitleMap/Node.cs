using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    /// <summary>
    /// ¼¿ ÁÂÇ¥ (Tilemap ±âÁØ ÁÂÇ¥)
    /// </summary>
    public Vector3Int cell;


    public Node(Vector3Int cell)
    {
        this.cell = cell;
       
    }
}
