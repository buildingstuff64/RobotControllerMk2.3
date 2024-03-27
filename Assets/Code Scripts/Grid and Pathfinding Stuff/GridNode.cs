using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    public GameObject HorzontalLine;
    public GameObject VerticalLine;
    public Vector2 pos;
    public Vector3 worldpos;

    public void changeColor(Color col)
    {
        HorzontalLine.GetComponent<SpriteRenderer>().color = col;
        VerticalLine.GetComponent<SpriteRenderer>().color = col;
    }

}
