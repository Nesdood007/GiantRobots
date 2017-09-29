using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class RegionBase
{

    public readonly float TerrainMaxHeight = .1f;
    public readonly float TerrainMinHeight = -.05f;
    System.Random random;
    protected bool IsCreatingHill = false;
    RatingScale currentHillHeight;
    public readonly float sin45Deg = Mathf.Sin(45);

    public RegionBase(int mapResoultion, Corners corner, RatingScale maxHeight, RatingScale minHeight, RatingScale hillHeight, RatingScale hillyness)
    {
        Length = mapResoultion / 4;
        ResetMap();
        Corner = corner;
        MaxHeight = maxHeight;
        MinHeight = minHeight;
        Hillyness = hillyness;
        HillHeight = hillHeight;
        random = new System.Random();
        GrassLandsRadius = Length / 4;
    }

    protected List<Mountain> Mountains
    {
        get;
        set;
    }

    public abstract float[,] GetMap();

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

        bool[] probabilityArray = new bool[10];

        for (int i = 0; i < (int)probability / (NumberOfHills == 0 ? 1 : NumberOfHills); i++)
            probabilityArray[i] = true;

        return probabilityArray[random.Next() % 10];
    }

    protected float GetRandomHillHeight()
    {
        if ((int)HillHeight == 0)
            return 0;

        if ((int)HillHeight == -1)
        {
            //TODO
        }

        var probabilityArray = new RatingScale[(int)Hillyness];
        var numberOfGivenHillHeights = (int)Mathf.Floor((int)Hillyness);

        for (int i = 0; i < numberOfGivenHillHeights; i++)
            probabilityArray[i] = HillHeight;

        int modifier = 1;
        for (int i = numberOfGivenHillHeights; i < probabilityArray.Length - numberOfGivenHillHeights; i++)
        {
            var val = ((int)Hillyness + modifier);
            if (val > 10 || val < 1)
                i--;
            else
                probabilityArray[i] = (RatingScale)val;

            modifier = (modifier < 0 ? 1 : -1) * (Mathf.Abs(modifier) + 1);

        }

        return TerrainMaxHeight * ((int)probabilityArray[random.Next() % probabilityArray.Length] / 10f);
    }

    public bool IsInGrasslands(int x, int y)
    {
        return x * x + y * y <= GrassLandsRadius;
    }

    protected float GetMaxHeight(List<float> heights)
    {
        float max = -1;
        foreach (var h in heights)
            if (h > max)
                max = h;
        return max;
    }
}

public class Desert : RegionBase
{
    Terrain sand;
    Terrain[] offTerrains;

    public Desert(int mapResoultion, Corners corner)
       : base(mapResoultion, corner, RatingScale.One, RatingScale.Zero, RatingScale.One, RatingScale.Zero)
    {
       
    }

    public Desert(int mapResoultion, Corners corner, Terrain sand, Terrain[] offTerrains)
        : base(mapResoultion, corner, RatingScale.One, RatingScale.Zero, RatingScale.One, RatingScale.Zero)
    {
        this.sand = sand;
        this.offTerrains = offTerrains;
    }

    public override float[,] GetMap()
    {
        ResetMap();
        bool canCreateMountain;
        List<float> heights = new List<float>();

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
                    canCreateMountain = canCreateMountain & m.CanCreateNewMountain(i, j);

                if(canCreateMountain && GetRandomChance(Hillyness))
                {
                    var radius = GetRandomHillHeight();
                    var centerX = sin45Deg * radius + i;
                    var centerY = sin45Deg * radius + j;
                    Mountains.Add(new Mountain(radius, (int)centerX, (int)centerY));
                }
                heights.Clear();
                foreach (var m in Mountains)
                {
                    if (m.IsInMountain(i, j))
                        heights.Add(m.HeightAtPosition(i, j));

                }

                Map[j, i] = heights.Count == 0 ? 0f : GetMaxHeight(heights);
            }
        }



        return new float[1, 1];

    }

    
}


public class Mountain
{
    
    public readonly float DefaultNewMountainDepthPercentage = .33f;

    public Mountain(float radius, int centerX, int centerY)
    {
        Radius = radius;
        CenterX = centerX;
        CenterY = centerY;
        NewMountainDepthPercentage = DefaultNewMountainDepthPercentage;
    }

    public Mountain(float radius, int centerX, int centerY, float newMountainDepthPercentage)
        :this(radius, centerX, centerY)
    {
        NewMountainDepthPercentage = newMountainDepthPercentage;
    }

    //The point must be the percentage below the max height to start a new mountain
    public float NewMountainDepthPercentage
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
        return HeightAtPosition (x,y) <= Radius * Radius;
    }

    public float HeightAtPosition(int x, int y)
    {
        return Mathf.Pow(x - CenterX, 2) + Mathf.Pow(y - CenterY, 2);
    }

    //To create a new mountain, the point must be outside of R * NewMountainDepthPercentage
    public bool CanCreateNewMountain(int x, int y)
    {
        return HeightAtPosition(x, y) > Radius * Radius * NewMountainDepthPercentage;
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