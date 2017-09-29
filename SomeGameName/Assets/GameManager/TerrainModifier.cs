using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainModifier : MonoBehaviour {

    GenerateHeightMap heightMapGenerator;
    Terrain terrain;
    void Awake()
    {
        terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
        var length = terrain.terrainData.heightmapResolution;
        heightMapGenerator = new GenerateHeightMap(length, length);
        
        var map = new Desert(length * 4, Corners.BottomLeft).GetMap();
       terrain.terrainData.SetHeights(0, 0, map);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
