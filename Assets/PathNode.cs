using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    public bool walkable;
    public Vector2Int pos;

    public PathNode prev;

    public int gCost;
    public int hCost;

    public PathNode(bool _walkable, Vector2Int pos)
    {
        this.walkable = _walkable;
        this.pos = pos;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Vector3 vector3
    {
        get { return new Vector3(pos.x, pos.y, 0); }
    }
}
