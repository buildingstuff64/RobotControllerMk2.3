using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ScrollList : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    public Transform content;
    private List<ListItem> items = new List<ListItem>();
    public void addItemPackage(Package p)
    {
        GameObject g = Instantiate(itemPrefab);
        g.transform.SetParent(content);
        g.transform.position = Vector3.zero;
        ListItem item = g.GetComponent<ListItem>();
        item.set(p);
        items.Add(item);
    }

    public void removeItem(Package p)
    {
        ListItem l = getListItem(p);
        if (l == null) return;
        items.Remove(l);
        l.delete();
    }

    ListItem getListItem(Package p)
    {
        foreach (ListItem item in items)
        {
            if (item.text.text == p.Name) return item;
        }
        return null;
    }

    public void updatePackage(Package p, string name, Vector2 pos, Vector3 worldpos, Color col, bool status)
    {
        ListItem item = getListItem(p);
        item.set(name, pos, worldpos, col, status);
    }

}
