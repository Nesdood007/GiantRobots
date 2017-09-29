using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class RegionBase {

    public readonly float TerrainMaxHeight = .1f;
    public readonly float TerrainMinHeight = -.05f;
    System.Random random;
    protected bool IsCreatingHill = false;
    RatingScale currentHillHeight;

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
        
       
        
    }

    public abstract int[,] GetMap();

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

    protected int[,] Map
    {
        get;
        set;
    }

    protected void ResetMap()
    {
        Map = new int[Length, Length];
    }

    protected bool GetRandomChance(RatingScale probability)
    {
        if (probability <= 0)
            return false;

        bool[] probabilityArray = new bool[10];

        for (int i = 0; i < (int)probability; i++)
            probabilityArray[i] = true;

        return probabilityArray[random.Next() % 10];
    }

    float GetRandomHillHeight()
    {
        if ((int)HillHeight == 0)
            return 0;

        if ((int)HillHeight == -1)
        {
        }

        var probabilityArray = new RatingScale[(int)Hillyness];
        var numberOfGivenHillHeights = (int)Mathf.Floor((int)Hillyness);

        for (int i = 0; i < numberOfGivenHillHeights; i++)
            probabilityArray[i] = HillHeight;

        int modifier = 1;
        for(int i = numberOfGivenHillHeights; i < probabilityArray.Length - numberOfGivenHillHeights; i++)
        {
            var val = ((int)Hillyness + modifier);
            if (val > 10 || val < 1)
                i--;
            else
                probabilityArray[i] = (RatingScale)val;

            modifier = (modifier<0?1:-1)*(Mathf.Abs(modifier)+1);

        }

        return TerrainMaxHeight * ((int)probabilityArray[random.Next() % probabilityArray.Length]/10f);
    }
}

public class Desert : RegionBase
{
    Terrain sand;
    Terrain[] offTerrains;

    public Desert(int mapResoultion, Corners corner, Terrain sand, Terrain[] offTerrains)
        :base(mapResoultion, corner, RatingScale.One, RatingScale.Zero, RatingScale.One, RatingScale.Zero)
    {
        this.sand = sand;
        this.offTerrains = offTerrains;
    }

    public override int[,] GetMap()
    {
        ResetMap();

        //Decide if we're starting by creating a hill
        if(GetRandomChance(Hillyness))
        {
            //IsCreatingHill = 0;
        }

        return new int[1, 1];

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