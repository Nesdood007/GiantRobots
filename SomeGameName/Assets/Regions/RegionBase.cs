﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class RegionBase
{

    public static readonly float TerrainMaxHeight = .05f;
    public static readonly float TerrainMinHeight = -.05f;
    System.Random random;
    protected bool IsCreatingHill = false;
    RatingScale currentHillHeight;
    public readonly float sin45Deg = Mathf.Abs(Mathf.Sin(Mathf.PI/4f));
    public static readonly float GrasslandFadeRadius = 50f;

    public RegionBase(int mapResoultion, Corners corner, RatingScale maxHeight, RatingScale minHeight, RatingScale hillHeight, RatingScale hillyness)
    {
        Length = mapResoultion / 2;
        ResetMap();
        Corner = corner;
        MaxHeight = maxHeight;
        MinHeight = minHeight;
        Hillyness = hillyness;
        HillHeight = hillHeight;
        random = new System.Random();
        GrassLandsRadius = Length / 4;
        MaxNumberOfMountainsAtOneTime = 5;
    }

    protected int MaxNumberOfMountainsAtOneTime
    {
        get;
        private set;
    }

    protected List<Mountain> Mountains
    {
        get;
        set;
    }

    public abstract float[,] GetMap();

    public abstract float[,,] GetTerrainMap(TerrainData data);

    public int Length
    {
        get;
        private set;
    }

    public Corners Corner
    {
        get;
        private set;
    }

    public RatingScale MaxHeight
    {
        get;
        private set;
    }

    public RatingScale MinHeight
    {
        get;
        private set;
    }

    public RatingScale Hillyness
    {
        get;
        private set;
    }

    public RatingScale HillHeight
    {
        get;
        private set;
    }

    protected float[,] Map
    {
        get;
        set;
    }

    protected int NumberOfHills
    {
        get;
        set;
    }

    public float GrassLandsRadius
    {
        get;
        private set;
    }

    protected void ResetMap()
    {
        Map = new float[Length, Length];
        NumberOfHills = 0;
        Mountains = new List<Mountain>();
    }

    protected bool GetRandomChance(RatingScale probability)
    {
        if (probability <= 0)
            return false;

        return random.Next()%(int)((1 / (float)(int)probability) * 100000)< (int)probability;     
    }

    protected int GetRandomHillHeight()
    {
        if ((int)HillHeight == 0)
            return 0;

        if ((int)HillHeight == -1)
        {
            //TODO
        }

        var probabilityArray = new RatingScale[10];
        var numberOfGivenHillHeights = (int)Mathf.Floor((int)Hillyness/2f);

        for (int i = 0; i < numberOfGivenHillHeights; i++)
            probabilityArray[i] = HillHeight;

        int modifier = 0;
        for (int i = numberOfGivenHillHeights; i < probabilityArray.Length - numberOfGivenHillHeights; i++)
        {
            var val = ((int)Hillyness + modifier);
            if (val > 10 || val < 1)
            {
                modifier = 0;
                val = ((int)Hillyness + modifier);
            }
            probabilityArray[i] = (RatingScale)val;

            
             modifier = (modifier > 0 ? -1 : 1) * (Mathf.Abs(modifier) + 1);

        }

        return ((int)probabilityArray[random.Next() % 10]);
    }

    public bool IsInGrasslands(int x, int y)
    {
        return x * x + y * y <= GrassLandsRadius* GrassLandsRadius;
    }

    public bool TextureIsInGrasslands(int x, int y)
    {
        return Mathf.Pow(Length/2f - x , 2) + Mathf.Pow(Length / 2f - y, 2) <= GrassLandsRadius * GrassLandsRadius;
    }

    protected float GetMaxHeight(List<float> heights)
    {
        float max = -1;
        foreach (var h in heights)
            if (h > max)
                max = h;
        return max;
    }

    public float PercentAwayFromGrassland(int x, int y)
    {
        return ((x * x + y * y) - (GrassLandsRadius* GrassLandsRadius)) / (GrasslandFadeRadius* GrasslandFadeRadius);
    }

    public void SetGrasslands(ref float[,,] map)
    {
        var length = map.GetLength(0);

        for(int j = 0; j < length; j++)
        {
            for(int i = 0; i < length; i++)
            {
                if(TextureIsInGrasslands(i, j))
                {
                    map[i, j, 0] = 0;
                    map[i, j, 1] = 1f;
                }
            }
        }
    }
}

public class Desert : RegionBase
{
    Texture sand;
    Texture[] offTerrains;

    public Desert(int mapResoultion, Corners corner)
       : base(mapResoultion, corner, RatingScale.One, RatingScale.Zero, RatingScale.One, RatingScale.One)
    {
       
    }

    public Desert(int mapResoultion, Corners corner, Texture sand, Texture[] offTerrains)
        : base(mapResoultion, corner, RatingScale.One, RatingScale.Zero, RatingScale.One, RatingScale.Five)
    {
        this.sand = sand;
        this.offTerrains = offTerrains;
    }

    public override float[,] GetMap()
    {
        ResetMap();
        bool canCreateMountain;
        List<float> heights = new List<float>();

        Debug.Log(Length);

        for (int j = 0; j < Length; j++)
        {
            for (int i = 0; i < Length; i++)
            {
                if (IsInGrasslands(i, j))
                {                  
                    Map[j, i] = 0f;
                    continue;
                }
                canCreateMountain = true;

                foreach (var m in Mountains)
                    canCreateMountain &= m.CanCreateNewMountain(i, j);

                if(canCreateMountain && GetRandomChance(Hillyness) && Mountains.Count <= MaxNumberOfMountainsAtOneTime)
                {
                    Debug.Log("Creating Mountain");
                    var radius = 0f;
                    float h = GetRandomHillHeight();
                    while (radius == 0)
                        radius = GetRandomHillHeight() * 100;
                    h =  (1 / (11 - h));

                    var angle = h / radius;

                    //radius = radius * Length * Length;
                    var centerX = radius + i;
                    var centerY = radius + j;
                    Mountains.Add(new Mountain(radius, (int)centerX, (int)centerY, h * RegionBase.TerrainMaxHeight));
                }

                heights.Clear();
                var tempMountain = new Mountain[Mountains.Count];
                Mountains.CopyTo(tempMountain);

                foreach (var m in tempMountain)
                {
                    if (m.IsInMountain(i, j))
                        heights.Add(m.HeightAtPosition(i, j));
                    else if (m.CanRemoveMountain(i, j))
                        Mountains.Remove(m);
                }

                Map[j, i] = heights.Count == 0 ? 0f : GetMaxHeight(heights);
            }
        }
        return Map;

    }

    public override float[,,] GetTerrainMap(TerrainData data)
    {
        var map = new float[data.alphamapWidth, data.alphamapHeight, 2];
        for (int j = 0; j < data.alphamapHeight; j++)
        {
            for (int i = 0; i < data.alphamapWidth; i++)
            {
                //if(IsInGrasslands(i, j))
                //{
                //    map[i, j, 0] = 0;
                //    map[i, j, 1] = 1f;
                //    continue;
                //}
                var fractionAway = PercentAwayFromGrassland(i, j);
                if(fractionAway > 1f)
                {
                    map[i, j, 0] = 1f;
                    map[i, j, 1] = 0;
                }
                else
                {
                    map[i, j, 0] = 1f- fractionAway;
                    map[i, j, 1] = fractionAway;
                }
                //var normX = (float)(i * 1.0 / (data.alphamapWidth - 1));
                //var normY = (float)(j * 1.0 / (data.alphamapHeight - 1));

                //// Get the steepness value at the normalized coordinate.
                //var angle = data.GetSteepness(normX, normY);

                //// Steepness is given as an angle, 0..90 degrees. Divide
                //// by 90 to get an alpha blending value in the range 0..1.
                //var frac = 1;
                //map[i, j, 0] = frac;
                //map[i, j, 1] = 1 - frac;
            }
        }

        return map;
    }


}


public class Mountain
{
    
    public readonly float DefaultNewMountainDepthPercentage = .33f;

    public Mountain(float radius, int centerX, int centerY, float height)
    {
        Radius = radius;
        CenterX = centerX;
        CenterY = centerY;
        NewMountainDepthPercentage = DefaultNewMountainDepthPercentage;
        Height = height;
    }

    public Mountain(float radius, int centerX, int centerY, float height, float newMountainDepthPercentage)
        :this(radius, centerX, centerY, height)
    {
        NewMountainDepthPercentage = newMountainDepthPercentage;
    }

    //The point must be the percentage below the max height to start a new mountain
    public float NewMountainDepthPercentage
    {
        get;
        private set;
    }

    public float Height
    {
        get;
        private set;
    }

    public float Radius
    {
        get;
        private set;
    }

    public int CenterX
    {
        get;
        private set;
    }

    public int CenterY
    {
        get;
        private set;
    }

    public bool IsInMountain(int x, int y)
    {
        var h = HeightAtPosition(x, y);
        return h > 0 && h <= Radius * Radius * Height;
    }

    public float HeightAtPosition(int x, int y)
    {
        var xPart = x - CenterX;
        var yPart = y - CenterY;
        return (1-((float)(xPart * xPart + yPart* yPart)/(Radius * Radius))) * Height;
    }

    //To create a new mountain, the point must be outside of R * NewMountainDepthPercentage
    public bool CanCreateNewMountain(int x, int y)
    {
        return HeightAtPosition(x, y) < Radius * Radius * Height;// * (1-NewMountainDepthPercentage);
    }

    public bool CanRemoveMountain(int x, int y)
    {
        return x > CenterX + Radius && y > CenterY + Radius;
    }

}

public enum Corners
{
    TopRight = 3,
    TopLeft = 2,
    BottomRight = 1,
    BottomLeft = 0
}

public enum HillHeight
{
    Tall = 3,
    Medium = 2,
    Small = 1,
    None = 0,
    Sea = -1
}

public enum RatingScale
{
    Ten = 10,
    Nine = 9,
    Eight = 8,
    Seven = 7,
    Six = 6,
    Five = 5,
    Four = 4,
    Three = 3,
    Two = 2,
    One = 1,
    Zero = 0,
    Negative = -1
}