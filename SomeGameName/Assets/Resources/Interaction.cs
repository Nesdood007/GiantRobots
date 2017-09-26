using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour {

    GameObject player;
    ResourceSpawn respawnComponent;
    // Use this for initialization
    void Start () {
        respawnComponent = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ResourceSpawn>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision col)
    {
        Debug.Log(col.gameObject.tag);
        if(col.gameObject.tag == "Player")
        {           
            if(tag == "Resource")
            {
                respawnComponent.Respawn<ResourceBase>(this.gameObject.GetComponent<ResourceBase>());
            }
            Destroy(gameObject);
        }
    }
}
