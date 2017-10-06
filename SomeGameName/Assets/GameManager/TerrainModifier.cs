using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TerrainModifier : MonoBehaviour
{

    GenerateHeightMap heightMapGenerator;
    Terrain terrain;
    public Texture2D desertSand;
    public Texture2D grassLand;
    void Awake()
    {
        terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
        var length = terrain.terrainData.heightmapResolution;
        //heightMapGenerator = new GenerateHeightMap(length, length);

        var s = new SplatPrototype();
        s.texture = desertSand;
        var g = new SplatPrototype();
        g.texture = grassLand;

        terrain.terrainData.splatPrototypes = new SplatPrototype[2] { s, g };

        var regions = new List<RegionBase>() { new Desert(length, Corners.BottomLeft), new Desert(length, Corners.TopLeft), new Desert(length, Corners.TopRight), new Desert(length, Corners.BottomRight) };
        var map = new float[length, length];

        var currentMap = regions.First((r) => r.Corner == Corners.TopLeft).GetMap();

        Copy(RotateArrayLeft(FlipDiagonal(currentMap)), ref map, Corners.TopRight);
        Copy(RotateArrayLeft(currentMap), ref map, Corners.TopLeft);
        Copy(RotateArrayRight(currentMap), ref map, Corners.BottomRight);
        Copy(currentMap, ref map, Corners.BottomLeft);

        terrain.terrainData.SetHeights(0, 0, map);

        var terrainMap = regions.First().GetTerrainMap(terrain.terrainData);
        regions.First().SetGrasslands(ref terrainMap);

        //TopLeft
        terrain.terrainData.SetAlphamaps(0, 0, terrainMap);

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    float[,] RotateArrayRight(float[,] array)
    {
        var length = array.GetLength(0);
        var newArray = new float[length, length];

        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < length; i++)
            {
                newArray[i, length - j-1] = array[j, i];
            }
        }
        return newArray;
    }

    float[,] RotateArrayLeft(float[,] array)
    {
        var length = array.GetLength(0);
        var newArray = new float[length, length];

        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < length; i++)
            {
                newArray[length - i - 1, j] = array[j, i];
            }
        }
        return newArray;
    }

    //float[,] FlipHorizontaly(float [,] )

    float[,] FlipDiagonal(float[,] array)
    {
        var length = array.GetLength(0);
        var newArray = new float[length, length];

        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < length; i++)
            {
                newArray[length - i - 1, length - j - 1] = array[j, i];
            }
        }
        return newArray;
    }

    void Copy(float[,] source, ref float[,] destination, Corners corner)
    {
        var length = source.GetLength(0);

        int startJ, starti;
        if (corner == Corners.TopRight)
        {
            startJ = 0;
            starti = 0;
        }
        else if (corner == Corners.BottomRight)
        {
            startJ = length;
            starti = 0;
        }
        else if (corner == Corners.TopLeft)
        {
            startJ = 0;
            starti = length;
        }
        else
        {
            startJ = length;
            starti = length;
        }
        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < length; i++)
            {
                destination[j + startJ, i + starti] = source[j, i];
            }
        }

    }


}
