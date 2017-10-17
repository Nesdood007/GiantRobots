using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Manager : MonoBehaviour {

    Dictionary<Corners, Regions> mapAlignment;
    public static List<GameObject> Players;

    public bool GameIsGoing
    {
        get;
        private set;
    }

    public Dictionary<Regions, Rect> RegionPositions
    {
        get;
        private set;
    }

    public Dictionary<Corners, Regions> MapAlignment
    {
        get { return mapAlignment; }
        set
        {
            if (value.Count != 4)
                throw new InvalidOperationException();
            mapAlignment = value;

            //var pos = GameObject.FindGameObjectWithTag("Terrain").transform.position;
            var terrain = GameObject.FindGameObjectWithTag("Terrain");
            var tData= terrain.GetComponent<Terrain>().terrainData;
            var bounds = new Bounds();
            bounds.SetMinMax(Vector3.zero, new Vector2(tData.size.x, tData.size.z));

            bounds.SetMinMax(terrain.transform.TransformPoint(bounds.min), terrain.transform.TransformPoint(bounds.max));

            RegionPositions = new Dictionary<Regions, Rect>();
            Rect temp;
            foreach (var key in value.Keys)
            {
                
                switch(key)
                {
                    case Corners.BottomLeft:
                        temp = new Rect(new Vector2(bounds.min.x + bounds.extents.x, bounds.min.y + bounds.extents.y), new Vector2(bounds.extents.x, bounds.extents.y));
                        break;
                    case Corners.BottomRight:
                        temp = new Rect(new Vector2(bounds.min.x, bounds.min.y + bounds.extents.y), new Vector2(bounds.extents.x, bounds.extents.y));
                        break;
                    case Corners.TopLeft:
                        temp = new Rect(new Vector2(bounds.min.x + bounds.extents.x, bounds.min.y), new Vector2(bounds.extents.x, bounds.extents.y));
                        break;
                    default:
                        temp = new Rect(bounds.min, new Vector2(bounds.extents.x, bounds.extents.y));
                        break;
                }
                if(!RegionPositions.Keys.Contains(value[key]))
                    RegionPositions.Add(value[key], temp);

                foreach(var obj in GameObject.FindGameObjectsWithTag("Enemy"))
                    obj.SendMessage("SetWanderingBounds", tData);
            }
        }
    }

	// Use this for initialization
	void Start () {

            
    }
	
	// Update is called once per frame
	void Update () {
        Players = GameObject.FindGameObjectsWithTag("Player").ToList();
        GameIsGoing = Players.Count > 0;

    }
}
