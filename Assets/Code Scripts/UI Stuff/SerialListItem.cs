using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SerialListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public void add(bool io, string cmd, string txt)
    {
        DateTime t = System.DateTime.Now;
        string time = string.Format("{0}:{1}:{2}:{3}", t.Hour.ToString(), t.Minute.ToString(), t.Second.ToString(), t.Millisecond.ToString());
        string iostr = io ? "<-" : "->";
        text.text = string.Format("{0} {1} {2}{3}", iostr, time, cmd, txt);
    }
}
