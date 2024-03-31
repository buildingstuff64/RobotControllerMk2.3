using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BaseLocations : MonoBehaviour
{
    [SerializeField] private string savepath;
    public Dictionary<string, Vector2> locations = new Dictionary<string, Vector2>();

    [System.Serializable]
    public class SerializableList<T>
    {
        public List<T> list;
    }

    [Serializable]
    public class Location 
    {
        public string color;
        public Vector2 position;
    }

    [SerializeField] private SerializableList<Location> Locations;

    private void Awake()
    {
        foreach (Location location in Locations.list)
        {
            locations.Add(location.color, location.position);
        }
    }

    private void Start()
    {
        savepath = Application.persistentDataPath + "/BaseLocations.json";
        if (!File.Exists(savepath))
        {
            saveBaseLocations();
        }
    }

    public void saveBaseLocations()
    {
        savepath = Application.persistentDataPath + "/BaseLocations.json";
        string txt = JsonUtility.ToJson(Locations, true);
        File.WriteAllText(savepath, txt);
    }

    public void getBaseLocations()
    {
        if (File.Exists(savepath))
        {
            string data = File.ReadAllText(savepath);
            Locations = JsonUtility.FromJson<SerializableList<Location>>(data);

            foreach (Location location in Locations.list)
            {
                locations.Add(location.color, location.position);
            }
        }
        else
        {
            saveBaseLocations();
        }

    }

}
