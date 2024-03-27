using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
    public Image icon;
    public TMP_Text text;
    public Image status;
    public Package package;

    [SerializeField] private Sprite tick;
    [SerializeField] private Sprite cross;

    public void set(Package p)
    {
        text = GetComponentInChildren<TMP_Text>();

        text.text = p.Name;
        status.sprite = p.status ? tick : cross;
        status.color = p.status ? Color.green : Color.red;
        icon.color = p.color;
    }

    public void set(string name, Vector2 pos, Vector3 worldpos, Color color, bool s)
    {
        text = GetComponentInChildren<TMP_Text>();

        text.text = name;
        status.sprite = s ? tick : cross;
        status.color = s ? Color.green : Color.red;
        icon.color = color;
    }

    public void delete() { Destroy(gameObject); }

}
