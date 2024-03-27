using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private SerialManager serialManager;
    [SerializeField] private ScrollList packagelist;
    [SerializeField] private SerialList seriallist;
    [SerializeField] private TMP_Text CommPortText;
    [SerializeField] private TMP_Dropdown CommPortDropdown;
    [SerializeField] private GameObject SerialConnect;


    public void serialIn(string txt)
    {
        seriallist.addItem(true, "", txt);
    }

    public void serialOut(string txt)
    {
        seriallist.addItem(false, seriallist.getDropdown(), txt);
    }

    public void clearSerial()
    {
        seriallist.clearall();
    }

    public void SendSerial()
    {
        string str = seriallist.getDropdown() + seriallist.getTextField();
        serialManager.SendSerial(str);
        serialOut(seriallist.getTextField());

    }

    public void addPackage(Package p)
    {
        packagelist.addItemPackage(p);
    }

    public void removePackage(Package p)
    {
        packagelist.removeItem(p);
    }

    public void updatePackage(Package p, string name, Vector2 pos, Vector3 worldpos, Color col, bool status)
    {
        packagelist.updatePackage(p, name, pos, worldpos, col, status);
    }

    public string getComm()
    {
        return CommPortText.text;
    }

    public void setCommPortOptions(List<string> txt)
    {
        CommPortDropdown.AddOptions(txt);
    }

    public void hideConnect(bool s)
    {
        SerialConnect.SetActive(s);
    }

}
