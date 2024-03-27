using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Pathfinding : MonoBehaviour
{
    private PathNode[,] grid;
    [SerializeField] private GridManager gm;
    [SerializeField] private PackageManager pm;
    [SerializeField] private SerialManager sm;
    [SerializeField] private RobotController rc;
    [SerializeField] private BaseLocations BaseLocations;
    private List<PathNode> finalPath;
    public Transform robotTransform;
    public Vector2 lastDirection = Vector2.up;
    private string IS = "I#";
    public Vector2Int currentPos = new Vector2Int(0,0);
    private Package ignorePackage;
    public List<Vector2Int> obsticals = new List<Vector2Int>();

    public void TestFunction()
    {
        StartCoroutine(rc.startControl());

    }

    public void toPackage(Package package)
    {
        Package p = pm.getPackage(package);
        ignorePackage = p;      
        rc.nextPackage = p;
        List<PathNode> path = findpathtopackage(currentPos, Vector2Int.FloorToInt(p.pos));
        gm.clearColors();
        gm.changegridcolors(path);
        string str = encodeInstruction(createInstruction(path, true));
        currentPos = Vector2Int.FloorToInt(p.pos);
        Debug.Log(str);
        sm.SendInstruction(str);
    }

    public void toBase(string color)
    {
        Vector2 v = BaseLocations.locations[color];
        List<Vector2> t = BaseLocations.locations.Values.ToList();
        t.Remove(v);
        List<PathNode> path = findpathtobase(currentPos, Vector2Int.FloorToInt(v), t);
        gm.clearColors();
        gm.changegridcolors(path);
        string str = encodeInstruction(createInstruction(path, true));
        currentPos = Vector2Int.FloorToInt(v);
        Debug.Log(str);
        sm.SendInstruction(str);
    }

    public void gotoPos(Vector2Int pos)
    {
        List<PathNode> path = findpath(currentPos, pos, null);
        string str = encodeInstruction(createInstruction(path, false));
        currentPos = pos;
        sm.SendInstruction(str);
    }

    void createGrid(List<Vector2Int> temp)
    {
        grid = new PathNode[gm.grid_size[0], gm.grid_size[1] + 1];
        for (int x = 0;x < gm.grid_size[0]; x++)
        {
            for(int y = 0;y < gm.grid_size[1] + 1; y++)
            {
                grid[x, y] = new PathNode(true, new Vector2Int(x, y));
                Package p = pm.getPackageFromPos(grid[x, y].pos);
                grid[x, y].walkable = (p == null) || (p == ignorePackage) ? true : false;
                if (p != null)
                {
                    if (p != ignorePackage)
                    {
                        grid[x, y].walkable = p.status ? true : false; 
                    }
                }
                foreach  (Vector2Int v in obsticals)
                {
                    if (v == new Vector2Int(x, y)) grid[x, y].walkable = false;
                }
                if (temp == null) continue;
                foreach (Vector2Int item in temp)
                {
                    if (item.Equals(new Vector2Int(x, y))) grid[x, y].walkable = false;
                }
            }
        }
        Debug.Log("grid created");
    }

    public List<PathNode> getNeighbours(PathNode node)
    {
        List<PathNode> neighbours = new List<PathNode>();


        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1) continue;

                int checkX = node.pos.x + x;
                int checky = node.pos.y + y;       

                if (checkX >= 0 && checkX < gm.grid_size[0] && checky >= 0 && checky < gm.grid_size[1] + 1)
                {
                    neighbours.Add(grid[checkX, checky]);
                }

            }
        }

        return neighbours;
    }

    public List<PathNode> findpathtopackage(Vector2Int start,  Vector2Int end)
    {
        return findpath(start, end, null);
    }

    public List<PathNode>findpathtobase(Vector2Int start, Vector2Int end, List<Vector2> temp)
    {
        List<Vector2Int>  v = new List<Vector2Int>();
        foreach (Vector2 item in temp) v.Add(Vector2Int.FloorToInt(item));
        return findpath(start, end, v);
    }

    public List<PathNode> findpath(Vector2Int start, Vector2Int end, List<Vector2Int> p)
    {
        createGrid(p);

        PathNode startNode = grid[start.x, start.y];
        PathNode endNode = grid[end.x, end.y];

        List<PathNode> open = new List<PathNode>();
        HashSet<PathNode> closed = new HashSet<PathNode>();
        open.Add(startNode);

        while(open.Count > 0)
        {
            PathNode node = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].fCost < node.fCost || open[i].fCost == node.fCost)
                {
                    if (open[i].hCost < node.hCost) node = open[i];
                }
            }

            open.Remove(node);
            closed.Add(node);

            if (node == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            foreach (PathNode neighbour in getNeighbours(node))
            {
                if (!neighbour.walkable || closed.Contains(neighbour)) continue;

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if(newCostToNeighbour < neighbour.gCost || !open.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.prev = node;

                    if (!open.Contains(neighbour)) open.Add(neighbour);
                }
            }
        }
        return null;

    }

    List<PathNode> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;
        

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.prev;
        }
        path.Add(startNode);
        path.Reverse();
        return path;
    }

    int GetDistance(PathNode nodeA, PathNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.pos.x - nodeB.pos.x);
        int dstY = Mathf.Abs(nodeA.pos.y - nodeB.pos.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    string createInstruction(List<PathNode> path, bool package)
    {

        string str = "";
        List<Vector2> p = new List<Vector2>();
        foreach (PathNode node in path) p.Add(node.pos);
        Vector2 previousDirection = lastDirection;

        for (int i = 0; i < p.Count - 1; i++)
        {
            string s = "";
            Vector2 direction = p[i + 1] - p[i];
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Calculate the relative angle
            float relativeAngle = angle - Mathf.Atan2(previousDirection.y, previousDirection.x) * Mathf.Rad2Deg;

            switch (clampAngle(relativeAngle))
            {
                case 90:
                    s += "L"; break;
                case -90:
                    s += "R"; break;
                case 180:
                case -180:
                    s += "T"; break;
            }
            s += "F";

            rc.addPathInstruction(path[i], s, direction);
            previousDirection = direction;
            str += s;
        }
        lastDirection = previousDirection;
        if (package)
        {
            rc.addPathInstruction(path[path.Count - 1], "P", lastDirection);
        }
        if (str == null || str.Length == 0) return "";
        str = str.Remove(str.Length - 1);
        return str += "PE";
    }

    string encodeInstruction(string code)
    {
        return Regex.Replace(code, @"F{2,}", m => "F" + m.Value.Length);
    }


    float clampAngle(float f)
    {
        if (f == 360 || f == -360) f = 0;
        if (f < -180)
        {
            f += 360;
        }
        if (f > 180)
        {
            f -= 360;
        }
        return f;
    }

}
