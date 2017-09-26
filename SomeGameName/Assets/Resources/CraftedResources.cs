using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseCraftedResources
{
    ResourceBase additionalResource;

    protected BaseCraftedResources(CraftedResources type, Dictionary<ResourceTypes, int> requires, List<Components> buildsInto)
    {

        if (!IsValidType(type))
            throw new InvalidOperationException("The given type if crafted resource ('" + type.ToString() + "') is not valid for the casted type");
        else
        {
            Type = type;
            BuildsInto = buildsInto ?? new List<Components>();
            Requires = requires ?? new Dictionary<ResourceTypes, int>();
        }
    }

    protected BaseCraftedResources(CraftedResources type, Dictionary<ResourceTypes, int> requires, List<Components> buildsInto, ResourceBase additionalResource)
        : this(type, requires, buildsInto)
    {
        if (!IsValidAdditionalResource(additionalResource))
            throw new InvalidOperationException("Can not match '" + additionalResource.Name + "' with '" + Name);
        else
            this.additionalResource = additionalResource;
    }

    public virtual string Name
    {
        get { return Type.ToString().Split('_')[1] + (additionalResource != null ? " ('" + additionalResource.Name + "')" : string.Empty); }
    }

    protected List<Components> BuildsInto
    {
        get;
        private set;
    }

    protected Dictionary<ResourceTypes, int> Requires
    {
        get;
        private set;
    }

    public CraftedResources Type
    {
        get;
        private set;
    }

    public ResourceTypes? AdditionalResource
    {
        get
        {
            if (additionalResource == null)
                return null;
            return additionalResource.Type;
        }
    }

    public string Description
    {
        get;
        set;
    }

    protected abstract bool IsValidAdditionalResource(ResourceBase resource);

    protected abstract bool IsValidType(CraftedResources type);
}

public class Steel : BaseCraftedResources
{
    ResourceBase additionalResource;

    public Steel(CraftedResources type, Dictionary<ResourceTypes, int> requires, List<Components> buildsInto, Gauges strength, Effects resistance, Gauges shineyness)
        : base(type, requires, buildsInto)
    {
        Strength = strength;
        Resistance = resistance;
        Shineyness = shineyness;
    }

    public Steel(CraftedResources type, Dictionary<ResourceTypes, int> requires, List<Components> buildsInto, Gauges strength, Effects resistance, Gauges shineyness, ResourceBase additionalResource)
        : base(type, requires, buildsInto, additionalResource)
    {
        Strength = strength;
        Resistance = resistance;
        Shineyness = shineyness;
    }

    protected override bool IsValidAdditionalResource(ResourceBase resource)
    {
        switch (resource.Type)
        {
            case ResourceTypes.Molybdenum:
                return Type == CraftedResources.S_316;
            case ResourceTypes.Titanium:
                return Type == CraftedResources.S_316Ti;
            case ResourceTypes.Nickel:
                return Type == CraftedResources.S_430;
            case ResourceTypes.Carbon:
                return Type == CraftedResources.S_440C;
        }
        return false;
    }

    protected override bool IsValidType(CraftedResources type)
    {
        return type.ToString().Split('_')[0] == "S";
    }

    public override string Name
    {
        get { return "Steel: "; }
    }

    public Gauges Strength
    {
        get;
        private set;
    }

    public Effects Resistance
    {
        get;
        private set;
    }

    public Gauges Shineyness
    {
        get;
        private set;
    }
}

public class Battery : BaseCraftedResources
{
    ResourceBase additionalResource;

    public Battery(CraftedResources type, Dictionary<ResourceTypes, int> requires, List<Components> buildsInto, Gauges power, bool isRechargable)
        : base(type, requires, buildsInto)
    {
        Power = power;
        IsRechargable = isRechargable;
    }

    public Battery(CraftedResources type, Dictionary<ResourceTypes, int> requires, List<Components> buildsInto, ResourceBase additionalResource, Gauges power, bool isRechargable)
        : base(type, requires, buildsInto, additionalResource)
    {
        Power = power;
        IsRechargable = isRechargable;
    }

    protected override bool IsValidAdditionalResource(ResourceBase resource)
    {
        switch (resource.Type)
        {
            case ResourceTypes.Lithium:
                return Type == CraftedResources.B_Lithium;
            case ResourceTypes.Plasma:
                return Type == CraftedResources.B_Plasma;
        }
        return false;
    }

    protected override bool IsValidType(CraftedResources type)
    {
        return type.ToString().Split('_')[0] == "B";
    }

    public new string Name
    {
        get { return "Battery: " + base.Name; }
    }

    public Gauges Power
    {
        get;
        private set;
    }

    public bool IsRechargable
    {
        get;
        private set;
    }
}


public enum CraftedResources
{
    S_316,
    S_304,
    S_316Ti,
    S_430,
    S_440C,
    B_Lithium,
    B_CarbonZinc,
    B_Plasma
}