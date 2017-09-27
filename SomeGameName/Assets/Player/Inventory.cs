using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    Dictionary<string, int> items;

    void Awake()
    {
        items = new Dictionary<string, int>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
        for(int i = 0; i < names.Length; i++)
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
            if (InventoryContainsKey(gameObj.name))
            {
                var key = FormatKey(gameObj.name, ObjectType.Resource);
                items[key]++;
            }
            else
            {
                items.Add(FormatKey(gameObj.name, ObjectType.Resource), 1);
            }

        }
        Debug.Log(InventoryToString());
    }

    string FormatKey(string key, ObjectType type)
    {
        switch(type)
        {
            case ObjectType.Resource:
                var i = key.IndexOfAny(new char[] { ' ', '(' });
                return i <= 0 ? key : key.Substring(0, i);
        }
        return key;
    }

    public bool InventoryContainsKey(string key)
    {
       
        foreach(var k in items.Keys)
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
            s += i.Key + ": " + i.Value +", ";
        if (s == string.Empty)
            return "";
        return s.Substring(0, s.Length-2);
    }
}
