using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int[] grid_size = new[] { 4, 8 };
    public float grid_spacing_x = 5;
    public float grid_spacing_y = 2;

    public GameObject[,] grid;
    public GameObject gridnode;
    public GameObject basearea;
    public CameraManager cameraManager;
    void Start()
    {
        CreateGrid();
        cameraManager.MoveCamera(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateSize(int X, int Y)
    {
        Array.Clear(grid, 0, grid.Length);
        grid_size = new int[] { X, Y };
        CreateGrid();
        cameraManager.MoveCamera(true);
    }

    void CreateGrid()
    {
        grid = new GameObject[grid_size[0], grid_size[1] + 1];
        for(int  x = 0; x < grid_size[0]; x++)
        {
            for (int y = 0; y < grid_size[1] + 1; y++)
            {
                if (y == grid_size[1])
                {
                    Vector3 pos = new Vector3(x * grid_spacing_x, y * grid_spacing_y, 0);
                    grid[x, y] = (GameObject)Instantiate(basearea, pos, Quaternion.identity);
                    GridNode gn = grid[x, y].GetComponent<GridNode>();
                    gn.HorzontalLine.transform.localScale = new Vector3(grid_spacing_x/2, 0.2f, 1);
                    gn.VerticalLine.transform.localScale = new Vector3(0.2f, grid_spacing_y/2, 1);
                    gn.VerticalLine.transform.localPosition = new Vector3(0, -grid_spacing_y / 4, 0);
                    gn.pos = new Vector2(x, y);
                    gn.worldpos = pos;
                    grid[x, y].name = string.Format("({0},{1})", x, y);
                    grid[x, y].transform.parent = transform;

                }
                else
                {
                    Vector3 pos = new Vector3(x * grid_spacing_x, y * grid_spacing_y, 0);
                    grid[x, y] = (GameObject)Instantiate(gridnode, pos, Quaternion.identity);
                    GridNode gn = grid[x, y].GetComponent<GridNode>();
                    gn.HorzontalLine.transform.localScale = new Vector3(grid_spacing_x, 0.2f, 1);
                    gn.VerticalLine.transform.localScale = new Vector3(0.2f, grid_spacing_y, 1);
                    gn.pos = new Vector2(x, y);
                    gn.worldpos = pos;
                    grid[x, y].name = string.Format("({0},{1})", x, y);
                    grid[x, y].transform.parent = transform;
                }
                
            }
        }
    }

    public GridNode getNodeFromPos(int x, int y)
    {
        return grid[x, y].GetComponent<GridNode>();
    }

    public void changegridcolors(List<PathNode> p)
    {
        if (p == null || p.Count == 0) { return; }
        foreach (PathNode item in p)
        {
            getNodeFromPos(item.pos.x, item.pos.y).changeColor(Color.red);
        }
    }

    public void clearColors()
    {
        for (int x = 0; x < grid_size[0]; x++)
        {
            for (int y = 0; y < grid_size[1]+1; y++)
            {
                getNodeFromPos(x, y).changeColor(Color.white);
            }
        }
    }
}
