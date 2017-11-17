﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.ObjectModel;

public class Manager : MonoBehaviour {

    Dictionary<Corners, Regions> mapAlignment;
    Dictionary<Corners, Regions> prevMapAlignment;
    public static bool DEBUG = true;
    public static List<GameObject> Players;
    public static List<GameObject> ResourcePrefabs;
    public static List<Texture2D> ResourceTextures;
    public static List<GameObject> CommonResourcePrefabs;
    public static List<GameObject> RareResourcePrefabs;
    public static List<GameObject> CraftedResourcePrefabs;
    public static List<GameObject> EnemyPrefabs;
    public static List<GameObject> BasePrefabs;
    public static List<GameObject> Bases;
    public static List<Material> SkyBoxes;
    public static readonly int NumberOfCommonEnemiesAtOneTime = 5;
    public static List<GameObject> Enemies;
    public static List<GameObject> Trees;
    public static List<Team> Teams;
    public static States CurrentState = States.Setup;
    static System.Random rand;
    Vector3 currentPosition;
    TerrainData tData;


    public static T GetInactiveCompoent<T>(GameObject gameobject)
        where T : Component
    {
        var c = gameobject.transform.GetChild(0).GetComponentsInParent(typeof(T), true);
        return c.Count() == 0 ? null : c[0] as T;
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

            AssignRegionPositions();
        }
    }

    void AssignRegionPositions()
    {
        var terrain = GameObject.FindGameObjectWithTag("Terrain");
        tData = terrain.GetComponent<Terrain>().terrainData;

        //Create a new bounds to represent our terrain
        var bounds = new Bounds();
        bounds.SetMinMax(Vector3.zero, new Vector2(tData.size.x, tData.size.z));
        bounds.SetMinMax(terrain.transform.TransformPoint(bounds.min), terrain.transform.TransformPoint(bounds.max));

        RegionPositions = new Dictionary<Regions, Rect>();
        Rect temp;
        foreach (var key in MapAlignment.Keys)
        {

            switch (key)
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

            //During testing, there might be multiples of regions
            if (!RegionPositions.Keys.Contains(MapAlignment[key]))
                RegionPositions.Add(MapAlignment[key], temp);
        }
    }

    private void Awake()
    {

        Players = GameObject.FindGameObjectsWithTag("Player").ToList();

        //Clear the scene of any lingering test players
        if (CurrentState != States.GameIsGoing)
        {
            foreach (var p in Players)
            {
                GameObject.Destroy(p);
            }

        }

        Players = GameObject.FindGameObjectsWithTag("Player").ToList();

        SetProperties();

        //Create Walls
        SpawnManager.SpawnWalls();

        //Create Trees
        SpawnManager.SpawnTrees(RegionPositions);

        //SpawnBases; Choose the spawn regions randomly
        SpawnManager.SpawnBases(RegionPositions);

        //Spawn Enemies
        SpawnManager.SpawnEnemies(RegionPositions, NumberOfCommonEnemiesAtOneTime);

        //Assign A SkyBox
        AssignSkybox();

        foreach (var b in Bases)
            GameObject.Destroy(b.GetComponent<BoxCollider>());

        foreach (var obj in GameObject.FindGameObjectsWithTag("Enemy"))
            obj.SendMessage("SetWanderingBounds", tData);
    }

    // Use this for initialization
    void Start() {


    }

    // Update is called once per frame
    void Update() {

        UpdateProperties();
    }

    void SetProperties()
    {
        rand = new System.Random();
        UpdateProperties();

        ResourcePrefabs = Resources.LoadAll<GameObject>("Objects/").ToList();
        ResourceBase component;
        CraftedResources component2;
        CommonResourcePrefabs = ResourcePrefabs.Where((r) => (component = GetInactiveCompoent<ResourceBase>(r)) != null && component.rarity == Rarity.Common).ToList();
        RareResourcePrefabs = ResourcePrefabs.Where((r) => (component = GetInactiveCompoent<ResourceBase>(r)) != null && component.rarity == Rarity.Rare).ToList();
        CraftedResourcePrefabs = ResourcePrefabs.Where((r) => (component2 = GetInactiveCompoent<CraftedResources>(r)) != null).ToList();
        EnemyPrefabs = Resources.LoadAll<GameObject>("Enemies/Prefabs").ToList();
        ResourceTextures = Resources.LoadAll<Texture2D>("Textures/").ToList();
        BasePrefabs = Resources.LoadAll<GameObject>("Buildings/Bases/").Where(obj => obj.name.ToLower().Contains("base_")).ToList();
        Bases = new List<GameObject>();
        SkyBoxes = Resources.LoadAll<Material>("SkyBoxes/").ToList();
        Teams = new List<global::Team>();
        Trees = Resources.LoadAll<GameObject>("Trees/").ToList();
    }

    void UpdateProperties()
    {
        Players = GameObject.FindGameObjectsWithTag("Player").ToList();
        Enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        if (Players.Count > 0 && CurrentState == States.Setup)
            CurrentState = States.GameIsGoing;
        else if (CurrentState == States.GameIsGoing && Players.Count == 0)
            CurrentState = States.GameIsOver;
    }

    void AssignSkybox()
    {
        RenderSettings.skybox = SkyBoxes.ToArray()[rand.Next() % SkyBoxes.Count];
    }    

    public static Teams AssignPlayerToTeam(GameObject player, out Vector3 startingPosition, out GameObject assignedBase)
    {
        Teams teamName;
        Team team;
        if (Teams.First().Players.Count <= Teams.Last().Players.Count)
        {
            team = Teams.First();
            team.AddPlayer(player);
        }
        else
        {
            team = Teams.Last();
            team.AddPlayer(player);
        }

        teamName = team.Name;

        var spawnPoint = team.GetUnusedSpawnPoint(out assignedBase);
        if (spawnPoint == null)
            throw new ArgumentOutOfRangeException();

        startingPosition = spawnPoint.Value;

        return teamName;
    }      

    public static GameObject GetResource(Regions region, float probabilityOfRareDrop = 0)
    {
        var areSpawningRare = probabilityOfRareDrop != 0 && (probabilityOfRareDrop > 1 || rand.Next() % 100 + 1 <= probabilityOfRareDrop * 100);
        var validResources = new List<GameObject>();
        foreach(var r in areSpawningRare? RareResourcePrefabs : CommonResourcePrefabs)
        {
            var properties = GetInactiveCompoent<ResourceBase>(r);
            if (properties == null)
                continue;
            if (properties.primaryRegion == region)
                validResources.Add(r);
        }

        return validResources.Count == 0 ? null : validResources[rand.Next() % validResources.Count];
    }

    public static GameObject GetReource(string name)
    {
        return ResourcePrefabs.FirstOrDefault(r => r.name.ToLower().Contains(name.ToLower()));        
    }
}

public enum Teams
{
    Alpha,
    Beta
}

public class Team
{
    List<GameObject> players;
    GameObject baseObj;
    List<string> usedSpawnPoints;

    public Team(Teams name, GameObject baseObj)
    {
        Name = name;
        this.baseObj = baseObj;
        players = new List<GameObject>();
        usedSpawnPoints = new List<string>();
    }

    public Teams Name
    {
        get;
        private set;
    }

    public ReadOnlyCollection<GameObject> Players
    {
        get { return players.AsReadOnly(); }
    }

    public void AddPlayer(GameObject player)
    {
        players.Add(player);
    }

    public Vector3 GetBasePosition()
    {
        return baseObj.transform.position;
    }

    public Vector3? GetUnusedSpawnPoint(out GameObject spawnBase)
    {
        spawnBase = null;
        var spawnPointsParent = baseObj.transform.Find("SpawnPoints");
        for(int i = 0; i < baseObj.transform.Find("SpawnPoints").childCount; i++ )
        {
            var point = spawnPointsParent.GetChild(i);
            if (!usedSpawnPoints.Contains(point.gameObject.name))
            {
                usedSpawnPoints.Add(point.name);
                spawnBase = baseObj;
                return point.position;
            }
        }

        return null;
    }

    public bool PointIsInBase(Vector3 point)
    {
        return baseObj.GetComponent<BoxCollider>().bounds.Contains(point);
    }
}

public enum States
{
    GameIsGoing,
    Setup,
    GameIsOver
}


