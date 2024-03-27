using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{
    public string Name;
    public Vector2 pos;
    public Vector3 worldpos;
    public Color color;
    public bool status;
    

    public void set(string Name, Vector2 pos, Vector3 worldpos, Color color, bool status)
    {
        this.Name = Name;
        this.pos = pos;
        this.worldpos = worldpos;
        this.color = color;
        this.status = status;

        gameObject.name = Name;
        GetComponent<SpriteRenderer>().color = color;
    }
}
