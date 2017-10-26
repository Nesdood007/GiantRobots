using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Manager : MonoBehaviour {

    Dictionary<Corners, Regions> mapAlignment;
    public static List<GameObject> Players;
    public static List<GameObject> ResourcePrefabs;
    public static List<GameObject> EnemyPrefabs;
    public static readonly int NumberOfCommonEnemiesAtOneTime = 5;
    public static List<GameObject> Enemies;

    System.Random rand;
    Vector3 currentPosition;

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

                
                
            }

            SpawnEnemies();

            foreach (var obj in GameObject.FindGameObjectsWithTag("Enemy"))
                obj.SendMessage("SetWanderingBounds", tData);
        }
    }

    private void Awake()
    {
        SetProperties();
        if(GameIsGoing)
        {
            foreach(var p in Players)
            {
                GameObject.Destroy(p);
            }
            
        }
    }

    // Use this for initialization
    void Start () {

            
    }
	
	// Update is called once per frame
	void Update () {

        UpdateProperties();
    }

    void SetProperties()
    {
        rand = new System.Random();
        UpdateProperties();
        ResourcePrefabs = Resources.LoadAll<GameObject>("Objects/").ToList();
        EnemyPrefabs = Resources.LoadAll<GameObject>("Enemies/Prefabs").ToList();
    }

    void UpdateProperties()
    {
        Players = GameObject.FindGameObjectsWithTag("Player").ToList();
        Enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        GameIsGoing = Players.Count > 0;        
    }

    public void SpawnEnemies()
    {
        if (Enemies == null)
        {
            SetProperties();

            foreach (var e in Enemies)
            {
                GameObject.Destroy(e);
            }
        }
        //var usedRegions = new List<Regions>();
        foreach (var region in RegionPositions.Keys)
        {
            for(int i = 0; i < NumberOfCommonEnemiesAtOneTime; i++)
            {
                var enemy = GetCommonEnemy(region);
                if(enemy != null)
                {
                    Instantiate(enemy, GetNextPosition(), Quaternion.Euler(Vector3.zero));
                }

            }

        }
    }

    public Vector3 GetNextPosition()
    {
        if (currentPosition == null)
        {
            currentPosition = Vector3.zero;
            return currentPosition;
        }

        var i = rand.Next() % 100 * 10;

        if (currentPosition.x < 499)
        {
            currentPosition = new Vector3(currentPosition.x + i, 0, currentPosition.z);
        } else
        {
            currentPosition = new Vector3(0, 0, currentPosition.z+i);
        }


        return currentPosition;
    }

    GameObject GetCommonEnemy(Regions region)
    {
        var regionEnemies = new List<GameObject>();
        foreach (var e in EnemyPrefabs)
        {
            var properties = (e.transform.GetChild(0).GetComponentsInParent(typeof(EnemySetupProps), true)[0] as EnemySetupProps);
            if (properties.primaryRegion == region && properties.rarity == Rarity.Common)
            {
                regionEnemies.Add(e);
            }

        }

        return regionEnemies.Count == 0 ? null : regionEnemies[rand.Next() % regionEnemies.Count];
    }

    //public static GameObject GetResource(string name)
    //{

    //}
}
