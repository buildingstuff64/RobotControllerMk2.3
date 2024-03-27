using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public enum Instructions { F, R, L, T, P, S, C, E}
public class RobotController : MonoBehaviour
{

    [SerializeField] private GridManager gm;
    [SerializeField] private PackageManager pm;
    [SerializeField] private SerialManager sm;
    [SerializeField] private Pathfinding pathfinding;

    List<Instructions> instructions = new List<Instructions>();
    public List<PathInstruction> Ipath = new List<PathInstruction>();
    [SerializeField] private List<GameObject> prefabs;
    private GameObject lastSpawn;
    public Package nextPackage;
    private int currentINSTRUCTION = 0;

    public void StartRobotControl()
    {
        StartCoroutine(startControl());
        
    }
    public IEnumerator startControl()
    {
        List<Package> packages = pm.packages;
        foreach (Package p in packages)
        {
            Debug.Log($"{pathfinding.currentPos} : {pathfinding.lastDirection}");
            sm.lastMsg = null;
            pathfinding.toPackage(p);
            doInstruction(0);
            while (Ipath.Count > 0)
            {
                if (sm.lastMsg != null)
                {
                    if (sm.lastMsg.Length>4)
                    {
                        if (sm.lastMsg.Substring(0, 4) == "NEXT")
                        {
                            currentINSTRUCTION = int.Parse(sm.lastMsg.Remove(0, 4));
                            sm.lastMsg = null;
                            doInstruction(currentINSTRUCTION);
                            continue;
                        } 
                    }
                    if (sm.lastMsg.Length>7)
                    {
                        if (sm.lastMsg.Substring(0, 7) == "REROUTE")
                        {
                            int i = int.Parse(sm.lastMsg.Remove(0, 7));
                            sm.lastMsg = null;
                            reroutetopackage(Ipath[i], p);
                            Debug.Log("REROUTED");
                            yield return null;
                        }  
                    }
                    if (sm.lastMsg == "FIN") break;
                }
                yield return new WaitForSeconds(0.01f);
            }
            Debug.Log("Send Serial");
            p.gameObject.transform.GetComponent<SpriteRenderer>().enabled = false;
            yield return null;
            sm.SendSerial("C#get");
            bool returnedColor = false;
            string color = "red";
            float timer = 3f;
            while (!returnedColor)
            {
                if (sm.lastMsg != null)
                {
                    if (sm.lastMsg.Substring(0, 2) == "C#" && sm.lastMsg.Length > 2)
                    {
                        color = sm.lastMsg.Remove(0, 2);
                        returnedColor = true;
                        sm.lastMsg = null;
                        break;
                    }
                }
                if (timer < 0) returnedColor = true;
                timer -= 0.01f;
                if (timer == 1.9 || timer == 2 || timer == 1) { sm.SendSerial("C#get"); }
                yield return new WaitForSeconds(0.01f);
            }

            Debug.Log("test to this point");
            p.color = getColorFromString(color);
            Ipath.Clear();
            pathfinding.toBase(color);
            doInstruction(0);
            while (Ipath.Count > 0)
            {
                if (sm.lastMsg != null)
                {
                    if (sm.lastMsg.Length > 4)
                    {
                        if (sm.lastMsg.Substring(0, 4) == "NEXT")
                        {
                            currentINSTRUCTION = int.Parse(sm.lastMsg.Remove(0, 4));
                            sm.lastMsg = null;
                            doInstruction(currentINSTRUCTION);
                            continue;
                        }
                    }
                    if (sm.lastMsg.Length > 7)
                    {
                        if (sm.lastMsg.Substring(0, 7) == "REROUTE")
                        {
                            int i = int.Parse(sm.lastMsg.Remove(0, 7));
                            sm.lastMsg = null;
                            Debug.Log(i);
                            reroutetobase(Ipath[i], color);
                            yield return null;
                        } 
                    }
                    if (sm.lastMsg == "FIN") break;
                }
                yield return new WaitForSeconds(0.01f);
            }
            pm.packageDropoff(p);
            Ipath.Clear();
            sm.lastMsg = null;
            Debug.Log("package finished");
            yield return new WaitForSeconds(1f);
        }
        pathfinding.gotoPos(Vector2Int.zero);

    }

    public void reroutetopackage(PathInstruction ci, Package p)
    {
        Ipath.Clear();
        pathfinding.currentPos = ci.node.pos;
        pathfinding.lastDirection= ci.direction;
        Vector2Int ob = pathfinding.currentPos + Vector2Int.RoundToInt(ci.direction);
        pathfinding.obsticals.Add(ob);
        pathfinding.toPackage(p);
        currentINSTRUCTION = 0;
        doInstruction(currentINSTRUCTION);
        pathfinding.obsticals.Remove(ob);
    }

    public void reroutetobase(PathInstruction ci, string color)
    {
        Ipath.Clear();
        pathfinding.currentPos = ci.node.pos;
        pathfinding.lastDirection = ci.direction;
        Vector2Int ob = pathfinding.currentPos + Vector2Int.RoundToInt(ci.direction);
        pathfinding.obsticals.Add(ob);
        pathfinding.toBase(color);
        currentINSTRUCTION = 0;
        doInstruction(currentINSTRUCTION);
        pathfinding.obsticals.Remove(ob);
    }


    public void addPathInstruction(PathNode n, string i, Vector2 v)
    {
        foreach (char c in i)
        {
            Ipath.Add(new PathInstruction(n, charToInstruction(c), v));
        }
    } 

    Instructions charToInstruction(char s)
    {
        switch (s)
        {
            case 'F':
                return Instructions.F;
            case 'R':
                return Instructions.R;
            case 'L':
                return Instructions.L;
            case 'T':
                return Instructions.T;
            case 'P':
                return Instructions.P;
            case 'E':
                return Instructions.E;
        }
        return Instructions.S;
    }

    List<Instructions> stringToInstructions(string i)
    {
        List<Instructions> I = new List<Instructions>();
        foreach (char c in i) 
        {
            I.Add(charToInstruction(c));
        }
        return I;
    }

    void doInstruction(int i)
    {
        if (Ipath.Count < 0) return;
        if (Ipath[i] == null) return;
        try
        {
            Vector3 worldpos = gm.getNodeFromPos(Ipath[i].node.pos.x, Ipath[i].node.pos.y).worldpos;
            if (Ipath[i].instruction == Instructions.F) spawnPrefab(0, worldpos, Ipath[i].direction);
            if (Ipath[i].instruction == Instructions.L) spawnPrefab(1, worldpos, Ipath[i].direction);
            if (Ipath[i].instruction == Instructions.R) spawnPrefab(2, worldpos, Ipath[i].direction);
            if (Ipath[i].instruction == Instructions.T) spawnPrefab(3, worldpos, Ipath[i].direction);
            if (Ipath[i].instruction == Instructions.P || Ipath[i].instruction == Instructions.E)
            {
                spawnPrefab(4, worldpos, Ipath[i].direction);
                pm.packagePickup(nextPackage);

            }
        }
        catch (System.Exception)
        {

            throw;
        }

    }

    void spawnPrefab(int i, Vector3 pos, Vector2 direction)
    {
        int r = 0;
        if (direction == Vector2.up) r = 0;
        if (direction == Vector2.left) r = 90;
        if (direction == Vector2.down) r = 180;
        if (direction == Vector2.right) r = 270;

        Destroy(lastSpawn);
        lastSpawn = Instantiate(prefabs[i], pos, Quaternion.Euler(0, 0, r));
    }

    private Color getColorFromString(string str)
    {
        switch (str)
        {
            case "red":
                return Color.red;
            case "green":
                return Color.green;
            case "yellow":
                return Color.yellow;
            case "orange":
                return new Color(1.0f, 0.64f, 0.0f);
            default:
                return Color.grey;
        }
    }

    public void quitApp()
    {
        Application.Quit();
    }

}
public class PathInstruction
{
    public PathNode node;
    public Instructions instruction;
    public Vector2 direction;

    public PathInstruction(PathNode node, Instructions instruction, Vector2 v)
    {
        this.node = node;
        this.instruction = instruction;
        this.direction = v;
    }

    void setDirection(Vector2 direction) { this.direction = direction; }

    public override string ToString()
    {
        return $"pos:{node.pos} dir:{direction} instruction:{instruction.ToString()}";
    }
}
