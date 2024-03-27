using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLocations : MonoBehaviour
{
    public Dictionary<string, Vector2> locations = new Dictionary<string, Vector2>();

    [Serializable]
    public struct Location 
    {
        public string color;
        public Vector2 position;
    }

    public List<Location> Locations;

    private void Awake()
    {
        foreach (Location location in Locations)
        {
            locations.Add(location.color, location.position);
        }
    }



}
