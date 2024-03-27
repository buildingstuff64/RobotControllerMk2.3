using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SerialList : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    public Transform content;
    public List<SerialList> items = new List<SerialList>();
    [SerializeField] private TMP_Text textfield;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TMP_Text dropdownselected;
    [SerializeField] private ScrollRect scrollRect;


    public void addItem(bool io, string dropdown, string txt)
    {
        GameObject g = Instantiate(itemPrefab);
        g.transform.SetParent(content);
        g.transform.position = Vector3.zero;
        SerialListItem sl = g.GetComponent<SerialListItem>();
        sl.add(io, dropdown, txt);
        scrollRect.velocity = new Vector2(0f, 1000f);
    }

    public string getDropdown()
    {
        return dropdownselected.text;
    }

    public string getTextField()
    {
        return textfield.text;
    }

    public void clearall()
    {
        foreach (GameObject g in content.GetComponentsInChildren<GameObject>())
        {
            Destroy(g);
        }
    }
}
