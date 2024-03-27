using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PackageManager : MonoBehaviour
{
    [SerializeField] private UIManager UI;
    [SerializeField] private Camera _camera;
    [SerializeField] private Pathfinding pathfinding;
    private bool isPlacePackage = true;
    private bool isPlaceObs = false;
    [SerializeField] private GameObject packagePrefab;
    [SerializeField] private GameObject obsPrefab;
    private GameObject packageCursor;
    private GameObject obsCursor;
    public List<Package> packages = new List<Package>();
    private List<GameObject> obGm = new List<GameObject>();
    public List<Package> donePackages = new List<Package>();

    void Start()
    {
        packageCursor = Instantiate(packagePrefab);
        obsCursor = Instantiate(obsPrefab);
    }

    // Update is called once per frame
    void Update()
    {

        if (isPlacePackage)
        {
            if (!packageCursor.activeSelf) { packageCursor.SetActive(true); }
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            packageCursor.transform.position = pos;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                Physics.OverlapSphere(hit.transform.position, 0.1f);
                GridNode gn = hit.collider.gameObject.GetComponent<GridNode>();
                packageCursor.transform.position = gn.transform.position;

                if (gn == null) return;

                if (Input.GetMouseButtonDown(0))
                {
                    placePackage(gn);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    deletePackage(gn.pos);
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    Package p = getPackageFromPos(gn.pos);
                    updatePackage(p.pos, "New Name", p.pos, p.worldpos, Color.red, true);
                }
            }
        }
        else
        {
            packageCursor.SetActive(false);
        }

        if (isPlaceObs)
        {
            if (!obsCursor.activeSelf) { obsCursor.SetActive(true); }
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            obsCursor.transform.position = pos;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                Physics.OverlapSphere(hit.transform.position, 0.1f);
                GridNode gn = hit.collider.gameObject.GetComponent<GridNode>();
                obsCursor.transform.position = gn.transform.position;

                if (gn == null) return;

                if (Input.GetMouseButtonDown(0))
                {
                    placeObs(gn);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    deleteObs(gn);
                }
            }
        }
        else
        {
            obsCursor.SetActive(false);
        }
    }

    void placePackage(GridNode gn)
    {
        foreach (Package item in packages)
        {
            if (item.pos == gn.pos) return;
        }


        GameObject obj = Instantiate(packagePrefab, gn.worldpos, Quaternion.identity);
        obj.transform.SetParent(transform);
        obj.AddComponent<Package>();
        Package p = obj.GetComponent<Package>();
        p.set(string.Format("Package {0}", gn.pos.ToString()), gn.pos, gn.worldpos, Color.grey, false);
        packages.Add(p);

        UI.addPackage(p);
        
    }

    void deletePackage(Vector2 pos)
    {
        Package p = getPackageFromPos(pos);
        if (p == null) return;
        UI.removePackage(p);
        packages.Remove(p);
        Destroy(p.gameObject);
        return;
    }

    void updatePackage(Vector2 p, string name, Vector2 pos, Vector3 worldpos, Color col, bool s)
    {
        Package package = getPackageFromPos(p);

        UI.updatePackage(package, name, pos, worldpos, col, s);
        package.set(name, pos, worldpos, col, s);
    }

    public void packagePickup(Package package)
    {
        Package p = getPackage(package);
        p.GetComponent<SpriteRenderer>().enabled = false;
        UI.updatePackage(p, p.Name, p.pos, p.worldpos, p.color, false);
    }

    public void packageDropoff(Package package)
    {
        Package p = getPackage(package);
        p.GetComponent<SpriteRenderer>().enabled = false;
        p.set(p.name, p.pos, p.worldpos, p.color, true);
        UI.updatePackage(p, p.Name, p.pos, p.worldpos, p.color, true);
    }

    public Package getPackageFromPos(Vector2 pos)
    {
        foreach (Package item in packages)
        {
            if(item.pos == pos) return item;
        }
        return null;
    }

    public Package getPackage(Package p)
    {
        foreach(Package pack in packages)
        {
            if (p == pack) return pack;
        }
        return null;
    }

    public void togglePlacePackage()
    {
        isPlacePackage = isPlacePackage ? false : true;
    }

    public void tooglePlaceObs()
    {
        isPlaceObs = isPlaceObs ? false : true;
    }
    void placeObs(GridNode gn)
    {
        GameObject obj = Instantiate(obsPrefab, gn.worldpos, Quaternion.identity);
        obj.transform.SetParent(transform);
        pathfinding.obsticals.Add(new Vector2Int((int)obj.transform.position.x, (int)obj.transform.position.y));
        obGm.Add(obj);

    }

    void deleteObs(GridNode gn)
    {
        foreach (Vector2Int v in pathfinding.obsticals)
        {
            if (gn.pos == v) pathfinding.obsticals.Remove(v);
        }
        foreach (GameObject gm in obGm)
        {
            if (gm.transform.position == gn.worldpos) Destroy(gm);
        }
    }

}
