using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UI : MonoBehaviour {

    Texture2D full;
    Texture2D empty;
    Texture2D background;

    public bool DEBUG;
    public float yOutterOffsetPercentage = .05f;
    public float xOutterOffsetPercentage = .05f;
    public float width = .1f;
    public float height = .05f;
    public float yInnerOffsetPercentage = .02f;
    public float xInnerOffsetPercentage = .02f;

    Rect backgroundRect;
    Rect fullRect;
    Rect emptyRect;
    Combat combat;
    Vector2 originalSize;

    float Health
    {
        get { return combat.Health; }
    }

    float PercentHealth
    {
        get { return combat.PercentHealth; }
    }

    void SetRectangles()
    {
        backgroundRect = new Rect(new Vector2(Screen.width * xOutterOffsetPercentage, Screen.height * yOutterOffsetPercentage), new Vector2(Screen.width * width, Screen.height * height));
        originalSize = new Vector2(backgroundRect.size.x * (1 - xInnerOffsetPercentage) - 2 * Screen.width * xInnerOffsetPercentage, backgroundRect.size.y * (1 - yInnerOffsetPercentage) - 2* Screen.height * yInnerOffsetPercentage);
        fullRect = new Rect(new Vector2(backgroundRect.position.x + Screen.width * xInnerOffsetPercentage, backgroundRect.position.y + Screen.height * yInnerOffsetPercentage), originalSize);
        emptyRect = new Rect(new Vector2(backgroundRect.position.x + Screen.width * xInnerOffsetPercentage, backgroundRect.position.y + Screen.height * yInnerOffsetPercentage), originalSize);
    }

    void Awake()
    {
        var textures = Resources.LoadAll("Health").Where((o) => o.GetType().FullName.Contains("Texture2D"));
        foreach(var t in textures)
        {
            if (t.name.Contains("full"))
                full = t as Texture2D;
            else if (t.name.Contains("empty"))
                empty = t as Texture2D;
            else if (t.name.Contains("black"))
                background = t as Texture2D;
        }
    }

    // Use this for initialization
    void Start () {

        SetRectangles();
    }
	
	// Update is called once per frame
	void Update () {
		if(combat == null)
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 0)
                return;
            foreach(var p in players)
            {
                if (p.GetComponent<Movement>().IsLocalPlayer)
                {
                    combat = p.GetComponent<Combat>();
                    break;
                }
            }
        }
        if (combat == null)
            return;
        fullRect.size = new Vector2(PercentHealth * originalSize.x, originalSize.y);
        
        if(DEBUG)        
            SetRectangles();
        
    }

    void OnGUI()
    {
        if (combat == null)
            return;
        GUI.DrawTexture(backgroundRect, background);
        GUI.DrawTexture(emptyRect, empty);
        GUI.DrawTexture(fullRect, full);
    }
}
