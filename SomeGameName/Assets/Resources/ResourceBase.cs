using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceBase : MonoBehaviour{

    public readonly int PrimaryRegionAbundance = 75;
    public readonly int OffRegionAbundance = 45;
    public ResourceTypes type;
    public Rarity rarity;
    List<CraftedResources> buildsInto;
    public Regions primaryRegion;
    public Regions currentRegion;
    public int baseSpawnRate = 30;
    Dictionary<Regions, int> abundance;

    void Start()
    {
        Type = type;
        BuildsInto = new List<CraftedResources>();
        Rarity = rarity;
        BuildsInto = buildsInto;
        PrimaryRegion = primaryRegion;
        CurrentRegion = currentRegion;
    }

    /// <summary>
    /// For each region, provides/returns an integer value that represents
    /// the abundance of the resource in that particular region. If you do
    /// not provide a region or the abundance is not between 1 and 100,
    /// the defaults will be filled in. 
    /// This is independent of the rarity of the object.
    /// </summary>
    protected Dictionary<Regions, int> Abundance
    {
        get
        {
            if(abundance == null)
            {
                abundance = new Dictionary<Regions, int>();
                foreach (var region in Enum.GetValues(typeof(Regions)))
                {
                    var r = (Regions)region;
                    abundance.Add(r, r == PrimaryRegion ? PrimaryRegionAbundance : OffRegionAbundance);
                }
            }
            return abundance;
        }

        set
        {
            abundance = value;

            foreach(var region in Enum.GetValues(typeof(Regions)))
            {
                var r = (Regions)region;
                if (!abundance.ContainsKey(r))
                {
                    abundance.Add(r, r == PrimaryRegion ? PrimaryRegionAbundance : OffRegionAbundance);
                } else if(abundance[r] < 1 || abundance[r] > 100)
                {
                    abundance[r] = r == PrimaryRegion ? PrimaryRegionAbundance : OffRegionAbundance;
                }
                
            }
        }
    }

    /// <summary>
    /// This is the number of seconds that we should wait before spawining
    /// an item of this type. 
    /// It is dependent on the rarity and region.
    /// </summary>
    public int GetSpawnRate(Regions region)
    {
        return (int)Math.Ceiling((double) (baseSpawnRate * (1 / (((int)Rarity) *.01)) * (region == PrimaryRegion?1:2))); 
    }

    public string Name
    {
        get { return Type.ToString(); }
    }

    public Rarity Rarity
    {
        get;
        private set;
    }

    protected List<CraftedResources> BuildsInto
    {
        get;
        private set;
    }

    public ResourceTypes Type
    {
        get;
        private set;
    }

    public Regions PrimaryRegion
    {
        get;
        private set;
    }

    public Regions CurrentRegion
    {
        get;
        private set;
    }

    public Regions GetRandomSpawnRegion()
    {
        var rA = Enum.GetValues(typeof(Regions));
        var rL = new List<Regions>();
        foreach(var r in rA)
        {
            var rCast = (Regions)r;
            if(rCast != PrimaryRegion)
            {
                rL.Add(rCast);
                rL.Add(PrimaryRegion);
            }
        }
        return (Regions)rL.ToArray().GetValue((new System.Random()).Next() % rL.Count);
    }

    /// <summary>
    ///  Returns an integer value between 1 and 100 that represents how abundant this
    ///  resource is in the given region based on it's rarity.
    /// </summary>
    /// <param name="region"></param>
    /// <returns></returns>
    public int GetAbundance(Regions region)
    {
        return (int) (Abundance[region] * ((int)Rarity/100f));
    }

    public bool CanBuildInto(CraftedResources item)
    {
        return BuildsInto.Contains(item);
    }
}

public enum ResourceTypes
{
    Molybdenum,
    Titanium,
    Nickel,
    Carbon,
    Iron,
    Chromite,
    Magnesium,
    Coal,
    Cobalt,
    Lithium,
    Plasma,
    Silicon
}

public enum Rarity
{
    Common = 100,
    Uncommon = 50,
    Rare = 25
}

public enum Regions
{
    Mountains,
    Swamps,
    Coast,
    Desert,
    Forest
}

public enum Gauges
{
    Low,
    Medium,
    High
}

public enum Effects
{
    Corrosion, 
    Heat,
    Cold
}

public enum ObjectType
{
    Resource,
    CraftedResource,
    Component
}