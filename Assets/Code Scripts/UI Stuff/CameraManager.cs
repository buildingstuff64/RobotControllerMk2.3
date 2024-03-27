using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GridManager gm;
    public Camera cam;

    float ypos;
    float xposleft;
    float xposcenter;
    void Start()
    {
        cam.GetComponent<Camera>();
        ypos = ((gm.grid_spacing_y * gm.grid_size[1] + 2) / 2) - gm.grid_spacing_y / 2;
        xposcenter = ((gm.grid_spacing_x * gm.grid_size[1] + 2) / 2) - gm.grid_spacing_x / 2;
        xposleft = xposcenter + (1920 / 1080 * 1920 / 670);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveCamera(bool lc)
    {
        float px = lc ? xposleft : xposcenter;
        Vector3 pos = new Vector3(px, ypos, -10);
        transform.position = pos;
        cam.orthographicSize = ((gm.grid_spacing_y / 2) * gm.grid_size[1] + 2)+.5f;
        
    }
}
