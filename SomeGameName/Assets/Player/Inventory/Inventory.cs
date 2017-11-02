using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Inventory : MonoBehaviour {

    Dictionary<string, int> items;

    bool showInventory = false;

    Rect inventoryRect;
    public float inventoryRectWidthPercent = .25f;
    public float inventoryRectHeightPercent = .65f;
    public float inventoryRectStartXPercent = 0f;
    public float inventoryRectStartYPercent = .35f;
    public float columns = 2f;
    public float columnOffsetPercent = .05f;

    public float texturePercentage = .2f;
    public float yMargin = .01f;
    public float buttonRadius = 2f;
    Vector2 textureSize;
    Vector2 textureOffset;
    
    List<Texture2D> selectedItems;

    public GUIStyle normalButtonSkin;
    public GUIStyle activeButtonSkin;



    void Awake()
    {
        items = new Dictionary<string, int>();
        selectedItems = new List<Texture2D>();
        
    }

    // Use this for initialization
    void Start() {
        inventoryRect = new Rect(new Vector2(Screen.width * inventoryRectStartXPercent, Screen.height * inventoryRectStartYPercent), new Vector2(Screen.width * inventoryRectWidthPercent, Screen.height * inventoryRectHeightPercent));
        textureSize = new Vector2(inventoryRect.size.x * texturePercentage, inventoryRect.size.x * texturePercentage);
        textureOffset = new Vector2(inventoryRect.size.x * (1 - texturePercentage * columns) / (columns * 2), inventoryRect.size.y * yMargin);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.B))
            showInventory = !showInventory;

        if(showInventory && Input.GetMouseButtonDown(1))
        {
            string craftedItem;
            if (TryCombine(selectedItems.ToArray(), out craftedItem))
            {
                AddItem(craftedItem);
                RemoveItems(selectedItems.Select(i => i.name).ToArray(), selectedItems.Select(i => ObjectType.Resource).ToArray());

            }
            selectedItems.Clear();
        }

        items = items.Count > 1 ? items.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : items;

        if (Manager.DEBUG)
        {
            inventoryRect = new Rect(new Vector2(Screen.width * inventoryRectStartXPercent, Screen.height * inventoryRectStartYPercent), new Vector2(Screen.width * inventoryRectWidthPercent, Screen.height * inventoryRectHeightPercent));
            textureSize = new Vector2(inventoryRect.size.x * texturePercentage, inventoryRect.size.x * texturePercentage);
            textureOffset = new Vector2(inventoryRect.size.x * (1 - texturePercentage * columns) / (columns * 2), inventoryRect.size.y * .05f);
        }
    }

    private void OnGUI()
    {
        if (!showInventory)
            return;

        GUI.skin.box.wordWrap = true;
        //GUI.BeginScrollView(new Rect(inventoryRect.position, new Vector2(inventoryRect.size.x * 1.5f, inventoryRect.size.y * 1.5f)), Vector3.zero, inventoryRect);
        GUI.Box(inventoryRect, string.Empty);
        float xDisplacement = 0;
        float yDisplacement = 0;
        int nextColumn = 0;
        var textures = Manager.ResourceTextures;
        bool setHover = false;
        string hoverText = null;
        Rect hoverRect = new Rect();

        foreach (var i in items)
        {
            Vector2 currentPosition = new Vector2(inventoryRect.x + xDisplacement, inventoryRect.y + yDisplacement) + textureOffset;

            if (currentPosition.x > inventoryRect.max.y)
                break;

            if (Manager.ResourceTextures.Count == 0)
                return;

            var texture = textures.FirstOrDefault(r => r.name.ToLower() == i.Key.ToLower()); //new Vector2(inventoryRect.size.x * .4f, inventoryRect.size.y * .1f)
            if (texture == null)
            {
                texture = textures.FirstOrDefault(r => r.name.ToLower().Contains(i.Key.ToLower()));
                if (texture == null)
                    return;
            }
            GUI.Box(new Rect(currentPosition, textureSize), string.Empty, selectedItems.Any(item => item.name == texture.name) ? activeButtonSkin : normalButtonSkin);//, new Rect(new Vector2(inventoryRect.x + xDisplacement, inventoryRect.y + yDisplacement)));            

            var currButton = new Rect(new Vector2(currentPosition.x - buttonRadius, currentPosition.y - buttonRadius), new Vector2(textureSize.x + buttonRadius * 2, textureSize.y + buttonRadius * 2));

            

            if (GUI.Button(currButton, texture))
            {
                if(selectedItems.Any(item => item.name == texture.name))
                {
                    foreach (var item in selectedItems)
                    {
                        if (item.name == texture.name)
                        {                            
                            selectedItems.Remove(item);
                            break;
                        }
                    }
                }
                else
                {
                    selectedItems.Add(texture);
                }                
            }

            if (currButton.Contains(Event.current.mousePosition))
            {
                hoverText = char.ToUpper(texture.name[0]) + texture.name.Substring(1) + " (" + items[FormatKey(texture.name, ObjectType.Resource)] + ")";
                hoverRect = new Rect(Event.current.mousePosition + new Vector2(10, 0), new Vector2(100, 25));
                setHover = true;
                //setHover = true;
                //var hover = transform.Find("InventoryHoverText").GetComponent<HoverText>();
                //hover.text = char.ToUpper(texture.name[0]) + texture.name.Substring(1);
                //hover.position = Event.current.mousePosition;
                //hover.showText = true;
            }

            nextColumn++;
            if (nextColumn >= columns)
            {
                xDisplacement = 0f;
                yDisplacement += textureSize.y + textureOffset.y * 2f;
                nextColumn = 0;
            } else
            {
                xDisplacement += textureSize.x + textureOffset.x * 2f;
            }



        }
        //GUI.EndScrollView();
        if (setHover)
        {
            //var hover = transform.Find("InventoryHoverText").GetComponent<HoverText>();
            //hover.showText = false;
            GUI.Box(hoverRect, hoverText);
        }
    }

    public bool TryCombine(Texture2D[] textures, out string name)
    {
        name = string.Empty;

        if (textures.Count() == 0)
            return false;

        List<GameObject> prefabs = new List<GameObject>();
        GameObject r;
        List<CraftedResourcesType> possibleCraftedResources = null;
        var resourceTypes = new List<ResourceTypes>();
        foreach (var texture in textures)
        {
            r = Manager.GetReource(texture.name);
            if (r == null)
                return false;
            prefabs.Add(r);
            
        }

        ResourceBase tempR;

        foreach(var p in prefabs)
        {
            tempR = Manager.GetInactiveCompoent<ResourceBase>(p);
            resourceTypes.Add(tempR.type);
            if (possibleCraftedResources == null)
            {
                possibleCraftedResources = ResourceBase.GetBuildsInto(tempR.type);
            }
            else
            {
                var tempPossibleCraftedResources = new List<CraftedResourcesType>();
                foreach (var currentR in ResourceBase.GetBuildsInto(tempR.type))
                {
                    if (possibleCraftedResources.Contains(currentR))
                        tempPossibleCraftedResources.Add(currentR);
                }
                if (tempPossibleCraftedResources.Count == 0)
                    return false;
                possibleCraftedResources = tempPossibleCraftedResources;
            }
        }

        CraftedResourcesType? material = null;


        foreach (var currentCraftedResource in possibleCraftedResources)
        {
            var currentRequirements = CraftedResources.GetRequirements(currentCraftedResource);
            bool isCorrectMaterial = true;
            foreach (var requirment in currentRequirements)
            {                
                if (!resourceTypes.Contains(requirment))
                {
                    isCorrectMaterial = false;
                    break;
                }
            }
            if (isCorrectMaterial)
            {
                material = currentCraftedResource;
                break;
            }
        }
        

        if (material == null)
            return false;

        name = material.Value.ToString();

        return true;
    }

    public bool RemoveItem(string name, ObjectType type)
    {
        var key = FormatKey(name, type);

        if (!InventoryContainsKey(key))
            return false;

        if (items[key] == 1)
            items.Remove(key);
        else
            items[key]--;

        return true;
    }

    public bool RemoveItems(string[] names, ObjectType[] types)
    {
        if (names.Length != types.Length)
            return false;

        bool success;
        for (int i = 0; i < names.Length; i++)
        {
            success = RemoveItem(names[i], types[i]);
            if (!success)
                return false;
        }
        return true;
    }

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag == ObjectType.Resource.ToString())
        {
            if (InventoryContainsKey(col.gameObject.name))
            {
                var key = FormatKey(col.gameObject.name, ObjectType.Resource);
                items[key]++;
            }
            else
            {
                items.Add(FormatKey(col.gameObject.name, ObjectType.Resource), 1);
            }

        }
        Debug.Log(InventoryToString());
    }

    public void AddItem(string name)
    {
        if (InventoryContainsKey(name))
        {
            var key = FormatKey(name, ObjectType.Resource);
            items[key]++;
        }
        else
        {
            items.Add(FormatKey(name, ObjectType.Resource), 1);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (Time.timeSinceLevelLoad < 2)
            return;

        GameObject gameObj;
        if (col.gameObject.name.Contains("Afterburner"))
            gameObj = col.gameObject.transform.parent.gameObject;
        else
            gameObj = col.gameObject;
        if (gameObj.tag == ObjectType.Resource.ToString())
        {
            AddItem(gameObj.name);

        }
        Debug.Log(InventoryToString());
    }

    string FormatKey(string key, ObjectType type)
    {
        switch (type)
        {
            case ObjectType.Resource:
                var i = key.IndexOfAny(new char[] { ' ', '(' });
                var k = i <= 0 ? key : key.Substring(0, i);
                return char.ToUpper(k[0]) + k.Substring(1);
        }
        return key;
    }

    public bool InventoryContainsKey(string key)
    {

        foreach (var k in items.Keys)
        {
            if (k.Contains(key) || key.Contains(k))
                return true;
        }
        return false;

    }

    public string InventoryToString()
    {
        var s = "";
        foreach (var i in items)
            s += i.Key + ": " + i.Value + ", ";
        if (s == string.Empty)
            return "";
        return s.Substring(0, s.Length - 2);
    }

    //class InventoryItem
    //{
    //    public InventoryItem (Texture2D texture, int row, int column)
    //    {
    //        Texture = texture;
    //        Row = row;
    //        Column = column;
    //    }

    //    public int Row
    //    {
    //        get;
    //        private set;
    //    }

    //    public int Column
    //    {
    //        get;
    //        private set;
    //    }

    //    public Texture2D Texture
    //    {
    //        get;
    //        private set;
    //    }
    //}
}
